using System;
using System.ComponentModel;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 为下载模块进度的事件提供数据。
    /// </summary>
    public class ModuleDownloadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        /// <summary>
        /// 创建一个ModuleDownloadProgressChangedEventArgs事件对象。
        /// </summary>
        /// <param name="moduleInfo">模块信息对象。</param>
        /// <param name="bytesReceived">接收的字节数。</param>
        /// <param name="totalBytesToReceive">总共接收的字节数。</param>
        public ModuleDownloadProgressChangedEventArgs(ModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive)
            : base(CalculateProgressPercentage(bytesReceived, totalBytesToReceive), null)
        {
            if (moduleInfo == null)
            {
                throw new ArgumentNullException("moduleInfo");
            }

            this.ModuleInfo = moduleInfo;
            this.BytesReceived = bytesReceived;
            this.TotalBytesToReceive = totalBytesToReceive;
        }

        /// <summary>
        /// 
        /// </summary>
        public ModuleInfo ModuleInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public long BytesReceived { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public long TotalBytesToReceive { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytesReceived"></param>
        /// <param name="totalBytesToReceive"></param>
        /// <returns></returns>
        private static int CalculateProgressPercentage(long bytesReceived, long totalBytesToReceive)
        {
            if ((bytesReceived == 0L) || (totalBytesToReceive == 0L) || (totalBytesToReceive == -1L))
            {
                return 0;
            }

            return (int)((bytesReceived * 100L) / totalBytesToReceive);

        }
    }
}
