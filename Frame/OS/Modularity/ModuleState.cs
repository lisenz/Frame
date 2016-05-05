namespace Frame.OS.Modularity
{
    /// <summary>
    /// 定义了模块从注册到加载到初始化的整个过程中的状态。
    /// </summary>
    public enum ModuleState
    {
        /// <summary>
        /// 尚未开始加载。
        /// </summary>
        NotStarted,

        /// <summary>
        /// 正在加载类型。
        /// </summary>
        LoadingTypes,

        /// <summary>
        /// 准备初始化。
        /// </summary>
        ReadyForInitialization,

        /// <summary>
        /// 正在初始化。
        /// </summary>
        Initializing,

        /// <summary>
        /// 已初始化。
        /// </summary>
        Initialized
    }
}
