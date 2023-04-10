using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using LitJson;

namespace BTEditor
{
    public class ActionNode : BTNode
    {   
        private TextField _actionNameField;

        public ActionNode(BTGraphView graphView) : base(graphView, "Action Node", BTNodeType.Action)
        {
            _actionNameField = new TextField();
            topContainer.style.backgroundColor = Color.green;
            mainContainer.Add(_actionNameField);
            Update();
        }

        public override BTNode Clone()
        {
            var clone = new ActionNode(_graphView);
            clone._actionNameField.value = _actionNameField.value;
            return clone;
        }

        public override JsonData Export()
        {
            var data = base.Export();
            data["actionName"] = _actionNameField.value;
            return data;
        }

        public void SetAction(string actionName)
        {
            _actionNameField.value = actionName;
        }
    }
}
