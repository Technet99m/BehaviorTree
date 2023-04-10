using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public class SequentialNode : Node
    {
        public enum SequenceMode
        {
            All,
            Any,
        }

        private List<Node> _children = new List<Node>();
        private SequenceMode _mode;
        private bool _runnigIsSuccess = false;

        public SequentialNode(Dictionary<string, object> context, List<Node> children, SequenceMode mode, bool runningIsSucces) : base(context)
        {
            _children = children;
            foreach(var child in _children)
                Attach(child);
            _mode = mode;
            _runnigIsSuccess = runningIsSucces;
        }

        public override NodeState Evaluate()
        {
            if (_children.Count == 0)
            {
                return NodeState.Failure;
            }

            bool hadSucces = false;
            bool hadRunning = false;

            foreach (var child in _children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success:
                        hadSucces = true;
                        if (_mode == SequenceMode.Any)
                            return NodeState.Success;
                        break;
                    case NodeState.Failure:
                        if (_mode == SequenceMode.All)
                            return NodeState.Failure;
                        break;
                    case NodeState.Running:
                        hadRunning = true;
                        if(_runnigIsSuccess && _mode == SequenceMode.Any)
                            return NodeState.Running;
                        if(!_runnigIsSuccess && _mode == SequenceMode.All)
                            return NodeState.Running;
                        break;
                }
            }

            if (_mode == SequenceMode.All)
            {
                return hadSucces ? NodeState.Success : NodeState.Running;
            }
            else
            {
                return hadRunning ? NodeState.Running : NodeState.Failure;
            }
        }
           
    }
}
