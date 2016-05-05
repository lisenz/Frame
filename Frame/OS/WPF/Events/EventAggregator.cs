using System;
using System.Collections.Generic;

namespace Frame.OS.WPF.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, EventBase> _Events = new Dictionary<Type, EventBase>();

        #region 实现接口IEventAggregator

        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            EventBase existingEvent = null;

            if (!this._Events.TryGetValue(typeof(TEventType), out existingEvent))
            {
                TEventType newEvent = new TEventType();
                this._Events[typeof(TEventType)] = newEvent;

                return newEvent;
            }
            else
            {
                return (TEventType)existingEvent;
            }
        }

        #endregion
    }
}
