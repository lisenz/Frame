using System;
using System.Linq;
using System.Windows.Threading;

namespace Frame.OS.WPF.Events
{
    public class CompositePresentationEvent<TPayload> : EventBase
    {
        private IDispatcherFacade _UIDispatcher;

        private IDispatcherFacade UIDispatcher
        {
            get
            {
                if (_UIDispatcher == null)
                {
                    this._UIDispatcher = new DefaultDispatcher();
                }

                return _UIDispatcher;
            }
        }

        public SubscriptionToken Subscribe(Action<TPayload> action)
        {
            return Subscribe(action, ThreadOption.PublisherThread);
        }

        public SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption threadOption)
        {
            return Subscribe(action, threadOption, false);
        }

        public SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive)
        {
            return Subscribe(action, ThreadOption.PublisherThread, keepSubscriberReferenceAlive);
        }

        public SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            return Subscribe(action, threadOption, keepSubscriberReferenceAlive, null);
        }

        public virtual SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            IDelegateReference filterReference;
            if (filter != null)
            {
                filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);
            }
            else
            {
                filterReference = new DelegateReference(new Predicate<TPayload>(delegate { return true; }), true);
            }
            EventSubscription<TPayload> subscription;
            switch (threadOption)
            {
                case ThreadOption.PublisherThread:
                    subscription = new EventSubscription<TPayload>(actionReference, filterReference);
                    break;
                case ThreadOption.BackgroundThread:
                    subscription = new BackgroundEventSubscription<TPayload>(actionReference, filterReference);
                    break;
                case ThreadOption.UIThread:
                    subscription = new DispatcherEventSubscription<TPayload>(actionReference, filterReference, UIDispatcher);
                    break;
                default:
                    subscription = new EventSubscription<TPayload>(actionReference, filterReference);
                    break;
            }


            return base.InternalSubscribe(subscription);
        }

        public virtual void Publish(TPayload payload)
        {
            base.InternalPublish(payload);
        }

        public virtual void Unsubscribe(Action<TPayload> subscriber)
        {
            lock (Subscriptions)
            {
                IEventSubscription eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
                if (eventSubscription != null)
                {
                    Subscriptions.Remove(eventSubscription);
                }
            }
        }

        public virtual bool Contains(Action<TPayload> subscriber)
        {
            IEventSubscription eventSubscription;
            lock (Subscriptions)
            {
                eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
            }
            return eventSubscription != null;
        }
    }
}
