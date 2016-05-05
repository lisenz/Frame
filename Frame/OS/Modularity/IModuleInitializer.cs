namespace Frame.OS.Modularity
{
    /// <summary>
    /// 提供创建模块并对模块进行初始化的方法。
    /// </summary>
    public interface IModuleInitializer
    {
        /// <summary>
        /// 对模块进行创建和初始化。
        /// </summary>
        /// <param name="moduleInfo">创建和执行初始化工作的模块的元数据对象。</param>
        void Initialize(ModuleInfo moduleInfo);
    }
}
