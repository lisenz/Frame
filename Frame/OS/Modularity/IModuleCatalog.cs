using System.Collections.Generic;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 定义了模块目录(ModuleCatalog)需要实现的一系列核心方法和属性
    /// </summary>
    public interface IModuleCatalog
    {
        /// <summary>
        /// 获取该模块目录下的模块集合。
        /// </summary>
        IEnumerable<ModuleInfo> Modules { get; }

        /// <summary>
        /// 获取指定模块依赖加载的依赖模块集合。
        /// </summary>
        /// <param name="moduleInfo">要获取依赖模块集合信息的模块对象。</param>
        /// <returns>返回该模块加载需要依赖的依赖模块集合。</returns>
        IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo);

        /// <summary>
        /// 获取指定模块集合中的模块加载需要依赖的依赖模块集合。
        /// </summary>
        /// <param name="modules">模块集合。</param>
        /// <returns>返回一个模块集合加载需要依赖的依赖模块集合。该集合是合并结果集合。</returns>
        IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules);
        
        /// <summary>
        /// 目录初始化操作。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 往模块目录中添加一个模块信息。
        /// </summary>
        /// <param name="moduleInfo"></param>
        void AddModule(ModuleInfo moduleInfo);
    }
}
