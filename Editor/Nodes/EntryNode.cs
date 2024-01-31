using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using LitJson;

namespace BTEditor
{
    public class EntryNode : BTNode
    {
        public override bool isUnique => false;
        private Port _entryPort;

        public EntryNode(BTGraphView graphView) : base(graphView, "Entry Point", BTNodeType.Entry, false)
        {
            _entryPort = GeneratePort("Entry", Direction.Output);
            topContainer.style.backgroundColor = Color.yellow;
            Update();
        }

        public override JsonData Export()
        {
            var data = base.Export();
            if (_entryPort.connections.Count() > 0)
                data["child"] = (_entryPort.connections.First().input.node as BTNode).GUID;
            return data;
        }

        public void ConnectTo(BTNode node)
        {
            var edge = _entryPort.ConnectTo(node.InputPort);
            _graphView.Add(edge);
        }
    }
}
