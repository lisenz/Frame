using System;
using System.Linq;
using System.Collections.Generic;

namespace Frame.OS
{
    internal class WeakDelegatesManager
    {
        private readonly List<DelegateReference> _Listeners = new List<DelegateReference>();

        public void AddListener(Delegate listener)
        {
            this._Listeners.Add(new DelegateReference(listener, false));
        }

        public void RemoveListener(Delegate listener)
        {
            this._Listeners.RemoveAll(reference =>
            {
                Delegate target = reference.Target;
                return listener.Equals(target) || target == null;
            });
        }

        public void Raise(params object[] args)
        {
            this._Listeners.RemoveAll(listener => listener.Target == null);

            foreach (Delegate handler in this._Listeners.ToList()
                .Select(listener => listener.Target)
                .Where(listener => listener != null))
            {
                handler.DynamicInvoke(args);
            }
        }
    }
}
