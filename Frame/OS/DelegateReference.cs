using System;
using System.Reflection;

namespace Frame.OS
{
    public class DelegateReference : IDelegateReference
    {
        private readonly Delegate _Delegate;
        private readonly WeakReference _WeakReference;
        private readonly MethodInfo _Method;
        private readonly Type _DelegateType;

        public DelegateReference(Delegate fdelegate, bool keepReferenceAlive)
        {
            if (fdelegate == null)
                throw new ArgumentNullException("delegate");

            if (keepReferenceAlive)
            {
                this._Delegate = fdelegate;
            }
            else
            {
                this._WeakReference = new WeakReference(fdelegate.Target);
                this._Method = fdelegate.Method;
                this._DelegateType = fdelegate.GetType();
            }
        }

        public Delegate Target
        {
            get
            {
                if (this._Delegate != null)
                {
                    return this._Delegate;
                }
                else
                {
                    return TryGetDelegate();
                }
            }
        }

        private Delegate TryGetDelegate()
        {
            if (this._Method.IsStatic)
            {
                return Delegate.CreateDelegate(this._DelegateType, null, this._Method);
            }
            object target = this._WeakReference.Target;
            if (target != null)
            {
                return Delegate.CreateDelegate(this._DelegateType, target, this._Method);
            }
            return null;
        }
    }
}
