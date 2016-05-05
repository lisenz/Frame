using System;
using System.Windows;

namespace Frame.OS.WPF.Regions
{
    internal class DefaultRegionManagerAccessor : IRegionManagerAccessor
    {
        public event EventHandler UpdatingRegions
        {
            add { RegionManager.UpdatingRegions += value; }
            remove { RegionManager.UpdatingRegions -= value; }
        }

        public string GetRegionName(DependencyObject element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return element.GetValue(RegionManager.RegionNameProperty) as string;
        }

        public IRegionManager GetRegionManager(DependencyObject element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return element.GetValue(RegionManager.RegionManagerProperty) as IRegionManager;
        }
    }
}
