using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public class ConvertRunning : Node
    {
        private Node _next;
        private bool _toSuccess;

        public ConvertRunning(Dictionary<string, object> context, bool toSuccess, Node next) : base(context)
        {
            _next = next;
            _toSuccess = toSuccess;
        }

        public override NodeState Evaluate()
        {
            var result = _next.Evaluate();
            if(result != NodeState.Running)
                return result;
            return _toSuccess ? NodeState.Success : NodeState.Failure;
        } 
    }
}