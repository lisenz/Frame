using System;
using System.Windows.Threading;

namespace Frame.OS.WPF.Events
{
    public class DispatcherEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        private readonly IDispatcherFacade _Dispatcher;

        public DispatcherEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, IDispatcherFacade dispatcher)
            : base(actionReference, filterReference)
        {
            this._Dispatcher = dispatcher;
        }

        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            _Dispatcher.BeginInvoke(action, argument);
        }
    }
}
