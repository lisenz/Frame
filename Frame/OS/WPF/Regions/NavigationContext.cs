using System;
using Frame.Core.Utility;

namespace Frame.OS.WPF.Regions
{
    public class NavigationContext
    {
        public NavigationContext(IRegionNavigationService navigationService, Uri uri)
        {
            this.NavigationService = navigationService;

            this.Uri = uri;
            this.Parameters = uri != null ? UriParsingHelper.ParseQuery(uri) : null;
        }

        public IRegionNavigationService NavigationService { get; private set; }

        public Uri Uri { get; private set; }

        public UriQuery Parameters { get; private set; }
    }
}
