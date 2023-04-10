using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTRuntime
{
    public interface IAction 
    {
        public NodeState Execute(Dictionary<string, object> context);
    }
}
  
