using System;

namespace Frame.OS.WPF.Events
{
    public class EventSubscription<TPayload> : IEventSubscription
    {
        private readonly IDelegateReference _ActionReference;
        private readonly IDelegateReference _FilterReference;

        public EventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
        {
            if (actionReference == null)
                throw new ArgumentNullException("actionReference");
            if (!(actionReference.Target is Action<TPayload>))
                throw new ArgumentException(String.Format("接口IDelegateReference的目标对象的数据类型必须是{0}.",
                    typeof(Action<TPayload>).FullName), "actionReference");

            if (filterReference == null)
                throw new ArgumentNullException("filterReference");
            if (!(filterReference.Target is Predicate<TPayload>))
                throw new ArgumentException(String.Format("接口IDelegateReference的目标对象的数据类型必须是{0}.",
                    typeof(Predicate<TPayload>).FullName), "filterReference");

            this._ActionReference = actionReference;
            this._FilterReference = filterReference;
        }

        public Action<TPayload> Action
        {
            get { return (Action<TPayload>)this._ActionReference.Target; }
        }

        public Predicate<TPayload> Filter
        {
            get { return (Predicate<TPayload>)this._FilterReference.Target; }
        }

        public virtual void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            if (action == null) 
                throw new System.ArgumentNullException("action");

            action(argument);
        }

        #region 实现接口IEventSubscription

        public SubscriptionToken SubscriptionToken { get; set; }

        public virtual Action<object[]> GetExecutionStrategy()
        {
            Action<TPayload> action = this.Action;
            Predicate<TPayload> filter = this.Filter;
            if (null != action && null != filter)
            {
                return arguments =>
                {
                    TPayload argument = default(TPayload);
                    if (null != arguments && arguments.Length > 0 && null != arguments[0])
                        argument = (TPayload)arguments[0];
                    if (filter(argument))
                        InvokeAction(action, argument);
                };
            }

            return null;
        }

        #endregion
    }
}
