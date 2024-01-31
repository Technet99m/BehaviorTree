using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using LitJson;

namespace BTEditor
{
    public class SequentialNode : BTNode
    {
        private int _outputPortCount = 0;
        private List<Port> _outputPorts;
        private DropdownField _modeSelector;
        private Toggle _runningIsSuccess;

        public SequentialNode(BTGraphView graphView) : base(graphView, "Sequential Node", BTNodeType.Sequential)
        {
            _outputPorts = new List<Port>();
            AddExtraOutputPort();

            _modeSelector = new DropdownField("Mode", new List<string> {"All", "Any"}, 0);
            _runningIsSuccess = new Toggle("Running is Success");
            
            mainContainer.Add(_modeSelector);
            mainContainer.Add(_runningIsSuccess);

            mainContainer.Add(new Button(() => AddExtraOutputPort()) {text = "Add Output Port"});
            mainContainer.Add(new Button(() => RemoveOutputPort()) {text = "Remove Output Port"});
            topContainer.style.backgroundColor = Color.blue;
            
            
            Update();
        }

        public override JsonData Export()
        {
            var data = base.Export();
            data["mode"] = _modeSelector.value;
            data["runningIsSuccess"] = _runningIsSuccess.value;
            data["children"] = JsonData.EmptyArray;
            foreach (var port in _outputPorts)
            {
                if(port.connections.Count() == 0)
                    continue;
                var portDestination = port.connections.First().input.node as BTNode;
                data["children"].Add(portDestination.GUID);
            }
            return data;
        }

        public override BTNode Clone()
        {
            var clone = new SequentialNode(_graphView);
            clone._modeSelector.value = _modeSelector.value;
            for (int i = 0; i < _outputPortCount; i++)
                clone.AddExtraOutputPort();
            return clone;
        }

        public void SetMode(string mode, bool runningIsSuccess)
        {
            _modeSelector.value = mode;
            _runningIsSuccess.value = runningIsSuccess;
        }

        public void FillConnections(List<BTNode> connections)
        {
            while (_outputPortCount > 0)
                RemoveOutputPort();
            foreach (var connection in connections)
            {
                var port = AddExtraOutputPort();
                var edge = port.ConnectTo(connection.InputPort);

                _graphView.Add(edge);
            }
        }

        private Port AddExtraOutputPort()
        {
            var outputPort = GeneratePort("Output " + _outputPortCount, Direction.Output);
            _outputPortCount++;
            outputContainer.Add(outputPort);
            _outputPorts.Add(outputPort);
            Update();
            return outputPort;
        }

        private void RemoveOutputPort()
        {
            if (_outputPortCount == 0)
                return;
            
            _outputPortCount--;
            var portToRemove = _outputPorts[_outputPortCount];
            //remove allConnections
            var connections = portToRemove.connections.ToList();
            foreach (var edge in connections)
            {
                edge.output.Disconnect(edge);
                edge.input.Disconnect(edge);
                edge.parent.Remove(edge);
            }
            outputContainer.Remove(portToRemove);
            _outputPorts.RemoveAt(_outputPortCount);
            Update();
        }
    }
}
