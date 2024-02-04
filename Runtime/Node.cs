using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public abstract class Node
    {
        public readonly Dictionary<string, object> Context;
        protected Node Parent;

        public Node( Dictionary<string, object> context)
        {
            Context = context;
        }

        public void Attach(Node child)
        {
            child.Parent = this;
        }

        public abstract NodeState Evaluate();
    }

    public enum NodeType
    {
        Entry,
        Action,
        Negation,
        Sequential,
        Condition
    }
}
