using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BTRuntime
{
    public class ConditionNode : Node
    {
        private IAction _condition;

        private Node _success;

        public ConditionNode(Dictionary<string,object> context, IAction condition, Node success) : base(context)
        {
            _condition = condition;
            _success = success;
        }

        public override NodeState Evaluate()
        {
            var condition = _condition.Execute(Context);
            
            if (condition == NodeState.Success)
            {
                return _success.Evaluate();
            }

            return condition;
        }
    }
}
