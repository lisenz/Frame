using System;
using System.Linq;
using System.Collections.Generic;

namespace Frame.OS.WPF.Events
{
    public abstract class EventBase
    {
        private readonly List<IEventSubscription> _Subscriptions = new List<IEventSubscription>();

        protected ICollection<IEventSubscription> Subscriptions
        {
            get { return this._Subscriptions; }
        }

        /// <summary>
        /// 内部订阅
        /// </summary>
        /// <param name="eventSubscription"></param>
        /// <returns></returns>
        protected virtual SubscriptionToken InternalSubscribe(IEventSubscription eventSubscription)
        {
            if (eventSubscription == null) 
                throw new System.ArgumentNullException("eventSubscription");

            eventSubscription.SubscriptionToken = new SubscriptionToken(Unsubscribe);

            lock (_Subscriptions)
            {
                this._Subscriptions.Add(eventSubscription);
            }
            return eventSubscription.SubscriptionToken;
        }

        /// <summary>
        /// 内部发布
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual void InternalPublish(params object[] arguments)
        {
            List<Action<object[]>> executionStrategies = PruneAndReturnStrategies();
            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(arguments);
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="token"></param>
        public virtual void Unsubscribe(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                IEventSubscription subscription = this._Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                if (subscription != null)
                {
                    this._Subscriptions.Remove(subscription);
                }
            }
        }

        public virtual bool Contains(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                IEventSubscription subscription = this._Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                return subscription != null;
            }
        }

        private List<Action<object[]>> PruneAndReturnStrategies()
        {
            List<Action<object[]>> returnList = new List<Action<object[]>>();

            lock (Subscriptions)
            {
                for (var i = this._Subscriptions.Count - 1; i >= 0; i--)
                {
                    Action<object[]> listItem =
                        this._Subscriptions[i].GetExecutionStrategy();

                    if (listItem == null)
                    {
                        // Prune from main list. Log?
                        this._Subscriptions.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(listItem);
                    }
                }
            }

            return returnList;
        }
    }
}
