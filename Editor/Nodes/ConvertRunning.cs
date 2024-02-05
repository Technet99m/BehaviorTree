using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using LitJson;
namespace BTEditor
{
    public class ConvertRunning : BTNode
    {
        private Port _nextPort;
        private DropdownField _modeSelector;

        public ConvertRunning(BTGraphView graphView) : base(graphView, "Convert Running Node", BTNodeType.ConvertRunning)
        {
            _modeSelector = new DropdownField("To", new List<string> {"Success", "Failure"}, 0);
            mainContainer.Add(_modeSelector);

            topContainer.style.backgroundColor = Color.magenta;
            _nextPort = GeneratePort("Output", Direction.Output);
            Update();
        }

        public override BTNode Clone()
        {
            var clone = new ConvertRunning(_graphView);
            clone._modeSelector.value = _modeSelector.value;
            return clone;
        }

        public override JsonData Export()
        {
            var data = base.Export();
            if (_nextPort.connections.Count() == 0)
                return data;
            var portDestination = _nextPort.connections.First().input.node as BTNode;
            data["child"] = portDestination.GUID;
            data["toSuccess"] = _modeSelector.value == "Success";
            return data;
        }

        public void SetData(BTNode next, bool toSuccess)
        {
            var edge = _nextPort.ConnectTo(next.InputPort);
            _graphView.Add(edge);
        }
    }
}