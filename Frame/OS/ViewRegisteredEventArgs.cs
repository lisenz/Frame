using System;

namespace Frame.OS
{
    /// <summary>
    /// 视图注册事件数据对象。
    /// </summary>
    public class ViewRegisteredEventArgs : EventArgs
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="regionName">部件名称。</param>
        /// <param name="getViewDelegate">返回一个视图对象的委托。</param>
        public ViewRegisteredEventArgs(string regionName, Func<object> getViewDelegate)
        {
            this.GetView = getViewDelegate;
            this.RegionName = regionName;
        }

        /// <summary>
        /// 获取部件名称。
        /// </summary>
        public string RegionName { get; private set; }

        /// <summary>
        /// 获取视图对象。
        /// </summary>
        public Func<object> GetView { get; private set; }
    }
}
