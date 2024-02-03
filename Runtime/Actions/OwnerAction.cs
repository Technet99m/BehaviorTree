using System.Collections.Generic;

namespace BTRuntime
{
    public abstract class OwnerAction<T> : IAction
    {
        protected T owner;

        public abstract NodeState Execute(Dictionary<string, object> context);

        public OwnerAction(T owner)
        {
            this.owner = owner;
        }
    }
}
