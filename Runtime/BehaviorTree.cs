using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public class BehaviorTree
    {
        private readonly Node Root;
        public readonly Dictionary<string,object> Context;

        public BehaviorTree(Node root, Dictionary<string, object> context)
        {
            Root = root;
            Context = context;
        }

        public NodeState Evaluate()
        {
            return Root.Evaluate();
        }
    }
}
