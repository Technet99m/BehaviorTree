using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace BTEditor
{
    public class BTGraphView : GraphView
    {
        private EntryNode _entryPointNode;
        private BTSearchWindow _searchWindow;
        private EditorWindow _window;

        private Port _pendingConnectionPort;

        public BTNode EntryPointNode => _entryPointNode;


        public BTGraphView(BTGraph graphWindow)
        {
            _window = graphWindow;

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
            nodeCreationRequest = ShowSearchWindow;                     
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
            if(_pendingConnectionPort != null)
            {
                AddElement(_pendingConnectionPort.ConnectTo(node.InputPort));
                _pendingConnectionPort = null;
            }
        }

        public void ShowSearchWindowAndConnect(Vector2 position, Port portToConnect)
        {
            var mousePosition = position + _window.position.position;

            _pendingConnectionPort = portToConnect;
            SearchWindow.Open(new SearchWindowContext(mousePosition), _searchWindow);
        }

        public void ShowSearchWindow(NodeCreationContext context)
        {
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
            _pendingConnectionPort = null;
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
