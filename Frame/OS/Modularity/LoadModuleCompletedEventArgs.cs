using System;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 为模块加载完成后的事件提供数据。
    /// </summary>
    public class LoadModuleCompletedEventArgs : EventArgs
    {
        public LoadModuleCompletedEventArgs(ModuleInfo moduleInfo, Exception error)
        {
            if (moduleInfo == null)
            {
                throw new ArgumentNullException("moduleInfo");
            }

            this.ModuleInfo = moduleInfo;
            this.Error = error;
        }

        /// <summary>
        /// 
        /// </summary>
        public ModuleInfo ModuleInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsErrorHandled { get; set; }
    }
}
