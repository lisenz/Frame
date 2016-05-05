using System;

namespace Frame.OS.WPF.Regions
{
    public interface INavigateAsync
    {
        void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback);
    }
}
