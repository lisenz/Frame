using System;
using System.Collections.Generic;

namespace Frame.OS
{
    /// <summary>
    /// 提供视图登记注册到部件中的方法。
    /// </summary>
    public interface IRegionViewRegistry
    {
        event EventHandler<ViewRegisteredEventArgs> ContentRegistered;

        IEnumerable<object> GetContents(string regionName);

        void RegisterViewWithRegion(string regionName, Type viewType);

        void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate);
    }
}
