using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using LitJson;

namespace BTEditor
{
    public class BTNode : Node
    {
        public string GUID;
        public BTNodeType type {get; private set;}

        protected BTGraphView _graphView;

        public Port InputPort => inputContainer[0] as Port;
        public virtual bool isUnique => true;

        public BTNode(BTGraphView graphView, string title, BTNodeType type) : this(graphView, title, type, true) {  }

        public BTNode(BTGraphView graphView, string title, BTNodeType type, bool inclueInputPort)
        {
            this.title = title;
            this.type = type;
            _graphView = graphView;
            GUID = System.Guid.NewGuid().ToString();
            var contextMenuManipulator =  new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Delete", (e) => Delete(), (e => isUnique ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled));
                evt.menu.AppendAction("Duplicate", (e) => Duplicate(), (e => isUnique ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled));
            });
            this.AddManipulator(contextMenuManipulator);
            
            if(!inclueInputPort)
                return;
            GeneratePort("Input", Direction.Input);
        }

        public void Update()
        {
            RefreshExpandedState();
            RefreshPorts();
        }

        public Port GeneratePort(string name, Direction portDirection)
        {
            var port = InstantiatePort(Orientation.Horizontal, portDirection, Port.Capacity.Single, typeof(BTNode));
            port.portName = name;
            if(portDirection == Direction.Input)
            {
                inputContainer.Add(port);
            }
            else
            {
                outputContainer.Add(port);
            }
            return port;
        }

        public void Delete()
        {
            var inputPorts = inputContainer.Query<Port>().ToList();
            var outputPorts = outputContainer.Query<Port>().ToList();
            var portsToRemove = inputPorts.Concat(outputPorts);
            
            foreach(var port in portsToRemove)
            {
                var connections = port.connections.ToList();
                foreach (var edge in connections)
                {
                    edge.output.Disconnect(edge);
                    edge.input.Disconnect(edge);
                    edge.parent.Remove(edge);
                }
            }
            
            _graphView.RemoveElement(this);
        }

        public void Duplicate()
        {
            var node = Clone();
            _graphView.CreateNode(node, GetPosition().position + new Vector2(100, 100));
        }

        public virtual JsonData Export()
        {
            var jsonData = new JsonData();
            jsonData["GUID"] = GUID;
            jsonData["type"] = type.ToString();
            jsonData["position"] = new JsonData();
            jsonData["position"]["x"] = GetPosition().position.x;
            jsonData["position"]["y"] = GetPosition().position.y;
            return jsonData;
        }

        public virtual BTNode Clone()
        {
            return new BTNode(_graphView, title, type);
        } 
    }

    public enum BTNodeType
    {
        Entry,
        Action,
        Negation,
        Sequential,
    }
}
