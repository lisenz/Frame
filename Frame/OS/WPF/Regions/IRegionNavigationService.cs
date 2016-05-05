using System;

namespace Frame.OS.WPF.Regions
{
    public interface IRegionNavigationService : INavigateAsync
    {
        IRegion Region { get; set; }

        IRegionNavigationJournal Journal { get; }

        event EventHandler<RegionNavigationEventArgs> Navigating;

        event EventHandler<RegionNavigationEventArgs> Navigated;

        event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
    }
}
