using System;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 提供统一管理和操作模块的方法。
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// 运行模块管理器。
        /// </summary>
        void Run();

        /// <summary>
        /// 将指定名称的模块加载到管理器中。
        /// </summary>
        /// <param name="moduleName"></param>
        void LoadModule(string moduleName);

        /// <summary>
        /// 加载模块过程中进度变化触发事件。
        /// </summary>
        event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        /// <summary>
        /// 加载模块完成后触发的事件
        /// </summary>
        event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
    }
}
