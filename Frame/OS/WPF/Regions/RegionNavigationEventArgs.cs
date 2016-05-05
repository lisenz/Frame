using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Frame.OS.WPF.Regions
{
    public class RegionNavigationEventArgs : EventArgs
    {
        public RegionNavigationEventArgs(NavigationContext navigationContext)
        {
            if (navigationContext == null)
            {
                throw new ArgumentNullException("navigationContext");
            }

            this.NavigationContext = navigationContext;
        }

        public NavigationContext NavigationContext { get; private set; }

        public Uri Uri
        {
            get
            {
                if (this.NavigationContext != null)
                {
                    return this.NavigationContext.Uri;
                }

                return null;
            }
        }
    }
}
