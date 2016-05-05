using System;

namespace Frame.OS.WPF.Regions
{
    public interface IRegionNavigationContentLoader
    {
        object LoadContent(IRegion region, NavigationContext navigationContext);
    }
}
