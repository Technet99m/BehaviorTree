using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private TextAsset _treeTemplate;

        private void Start()
        {
            var actions = new Dictionary<string, IAction>();
            actions.Add("Yep", new YepAction());
            var tree = BTBuilder.BuildTree(_treeTemplate.text, actions);    
            
            Debug.Log(tree.Evaluate());
        }
    }

    public class YepAction : IAction
    {
        public NodeState Execute(Dictionary<string, object> context)
        {
            Debug.Log("Yep");
            return NodeState.Success;
        }
    }
}
