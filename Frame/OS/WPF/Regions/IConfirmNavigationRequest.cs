using System;

namespace Frame.OS.WPF.Regions
{
    public interface IConfirmNavigationRequest : INavigationAware
    {
        void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback);
    }
}
