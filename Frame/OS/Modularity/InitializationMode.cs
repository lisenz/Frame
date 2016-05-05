namespace Frame.OS.Modularity
{
    /// <summary>
    /// 定义了程序模块初始化的模式。
    /// </summary>
    public enum InitializationMode
    {
        /// <summary>
        /// 在程序启动时自动加载。
        /// </summary>
        WhenAvailable,

        /// <summary>
        /// 按需加载。
        /// </summary>
        OnDemand
    }
}
