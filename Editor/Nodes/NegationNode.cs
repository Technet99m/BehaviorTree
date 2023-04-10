using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using LitJson;
namespace BTEditor
{
    public class NegationNode : BTNode
    {
        private Port _nextPort;

        public NegationNode(BTGraphView graphView) : base(graphView, "Negation Node", BTNodeType.Negation)
        {
            topContainer.style.backgroundColor = Color.red;
            _nextPort = GeneratePort("Output", Direction.Output);
            Update();
        }

        public override BTNode Clone()
        {
            return new NegationNode(_graphView);
        }

        public override JsonData Export()
        {
            var data = base.Export();
            if (_nextPort.connections.Count() == 0)
                return data;
            var portDestination = _nextPort.connections.First().input.node as BTNode;
            data["child"] = portDestination.GUID;
            return data;
        }

        public void SetNext(BTNode next)
        {
            _nextPort.ConnectTo(next.InputPort);

            var edge = new Edge()
            {
                output = _nextPort,
                input = next.InputPort
            };
            _graphView.Add(edge);
        }
    }
}
