namespace Frame.OS.WPF.Events
{
    /// <summary>
    /// 线程类型
    /// </summary>
    public enum ThreadOption
    {
        /// <summary>
        /// 发布者线程
        /// </summary>
        PublisherThread,

        /// <summary>
        /// UI线程
        /// </summary>
        UIThread,

        /// <summary>
        /// 后台线程
        /// </summary>
        BackgroundThread
    }
}
