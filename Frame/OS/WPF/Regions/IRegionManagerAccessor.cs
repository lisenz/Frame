using System;
using System.Windows;

namespace Frame.OS.WPF.Regions
{
    public interface IRegionManagerAccessor
    {
        event EventHandler UpdatingRegions;

        string GetRegionName(DependencyObject element);

        IRegionManager GetRegionManager(DependencyObject element);
    }
}
