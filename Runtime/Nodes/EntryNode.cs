using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public class EntryNode : Node
    {
        private Node _next;

        public EntryNode(Dictionary<string,object> context, Node next) : base(context)
        {
            _next = next;
            Attach(_next);
        }

        public override NodeState Evaluate()
        {
            return _next.Evaluate();
        }
    }
}

