using System;
using Frame.OS.WPF.Regions;

namespace Frame.OS.WPF.Extensions
{
    public static class NavigationAsyncExtensions
    {
        public static void RequestNavigate(this INavigateAsync navigation, string target)
        {
            RequestNavigate(navigation, target, nr => { });
        }

        public static void RequestNavigate(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback)
        {
            if (navigation == null) 
                throw new ArgumentNullException("navigation");
            if (target == null) 
                throw new ArgumentNullException("target");

            var targetUri = new Uri(target, UriKind.RelativeOrAbsolute);

            navigation.RequestNavigate(targetUri, navigationCallback);
        }

        public static void RequestNavigate(this INavigateAsync navigation, Uri target)
        {
            if (navigation == null) 
                throw new ArgumentNullException("navigation");

            navigation.RequestNavigate(target, nr => { });
        }
    }
}
