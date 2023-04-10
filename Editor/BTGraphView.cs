using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

namespace BTEditor
{
    public class BTGraphView : GraphView
    {
        private EntryNode _entryPointNode;
        private BTSearchWindow _searchWindow;

        public BTNode EntryPointNode => _entryPointNode;


        public BTGraphView(BTGraph graphWindow)
        {
            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            _entryPointNode = CreateEntryPointNode();

            _searchWindow = ScriptableObject.CreateInstance<BTSearchWindow>();
            _searchWindow.Configure(graphWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);      
        }

        public void LoadFromFile(string filepath)
        {
            RemoveElement(_entryPointNode);
            _entryPointNode = BTIO.Import(this, filepath);
        }

        public void CreateNode(BTNode node, Vector2 position)
        {
            node.SetPosition(new Rect(position, new Vector2(100, 150)));
            AddElement(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node && port.portType == startPort.portType && port.direction != startPort.direction)
                    compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        private EntryNode CreateEntryPointNode()
        {
            var node = new EntryNode(this);
            node.SetPosition(new Rect(100, 200, 100, 150));
            AddElement(node);
            return node;
        }
    }
}
