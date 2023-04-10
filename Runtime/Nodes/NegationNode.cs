using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public class NegationNode : Node
    {
        private Node _next;

        public NegationNode(Dictionary<string, object> context, Node next) : base(context)
        {
            _next = next;
        }

        public override NodeState Evaluate()
        {
            switch (_next.Evaluate())
            {
                case NodeState.Success:
                    return NodeState.Failure;
                case NodeState.Failure:
                    return NodeState.Success;
                default:
                    return NodeState.Running;
            }
        } 
    }
}