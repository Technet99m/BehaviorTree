using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BTRuntime
{
    public class ActionNode : Node
    {
        private IAction _action;

        public ActionNode(Dictionary<string,object> context, IAction action) : base(context)
        {
            _action = action;
        }

        public override NodeState Evaluate()
        {
            return _action.Execute(Context);
        }
    }
}

