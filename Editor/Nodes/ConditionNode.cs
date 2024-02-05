using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using LitJson;

namespace BTEditor
{
    public class ConditionNode : BTNode
    {
        private TextField _conditionNameField;

        private Port _successPort;

        public ConditionNode(BTGraphView graphView) : base(graphView, "Conditional Node", BTNodeType.Condition)
        {
            _conditionNameField = new TextField();
            topContainer.style.backgroundColor = Color.yellow;
            mainContainer.Add(_conditionNameField);

            _successPort = GeneratePort("Success", Direction.Output);

            Update();
        }

        public override JsonData Export()
        {
            var data = base.Export();
            data["condition"] = _conditionNameField.value;
            if (_successPort.connections.Count() != 0)
            {
                var successDestination = _successPort.connections.First().input.node as BTNode;
                data["success"] = successDestination.GUID;
            }
            return data;
        }

        public override BTNode Clone()
        {
            var clone = new ConditionNode(_graphView);
            return clone;
        }


        public void SetData(BTNode success, string conditionName)
        {
            if (success != null)
            {
                var edge = _successPort.ConnectTo(success.InputPort);
                _graphView.Add(edge);
            }
            _conditionNameField.value = conditionName;
        }
    }
}
