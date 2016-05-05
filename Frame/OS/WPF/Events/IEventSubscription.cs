using System;

namespace Frame.OS.WPF.Events
{
    public interface IEventSubscription
    {
        SubscriptionToken SubscriptionToken { get; set; }

        Action<object[]> GetExecutionStrategy();
    }
}
