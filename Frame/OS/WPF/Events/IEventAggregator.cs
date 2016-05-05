namespace Frame.OS.WPF.Events
{
    /// <summary>
    /// 事件聚合器
    /// </summary>
    public interface IEventAggregator
    {
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
