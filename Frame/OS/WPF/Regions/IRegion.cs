using System;
using System.Windows;
using System.ComponentModel;

namespace Frame.OS.WPF.Regions
{
    public interface IRegion : INavigateAsync, INotifyPropertyChanged
    {
        IViewsCollection Views { get; }

        IViewsCollection ActiveViews { get; }

        object Context { get; set; }

        string Name { get; set; }

        Comparison<object> SortComparison { get; set; }

        IRegionManager Add(object view);

        IRegionManager Add(object view, string viewName);

        IRegionManager Add(object view, string viewName, bool createRegionManagerScope);

        void Remove(object view);

        void Activate(object view);

        void Deactivate(object view);

        object GetView(string viewName);

        IRegionManager RegionManager { get; set; }

        IRegionBehaviorCollection Behaviors { get; }

        IRegionNavigationService NavigationService { get; set; }
    }
}
