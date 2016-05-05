using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.OS.Window.Regions
{
    /// <summary>
    /// 提供部件的一系列属性和方法。
    /// </summary>
    public interface IRegion
    {
        IViewsCollection Views { get; }
        IViewsCollection ActiveViews { get; }
        object Context { get; set; }
        string Name { get; set; }
        Comparison<object> SortComparison { get; set; }
        IRegionManager RegionManager { get; set; }
        
        IRegionManager Add(object view);
        IRegionManager Add(object view, string viewName);
        IRegionManager Add(object view, string viewName, bool createRegionManagerScope);
        void Remove(object view);
        void Activate(object view);
        void Deactivate(object view);
        object GetView(string viewName);
    }
}
