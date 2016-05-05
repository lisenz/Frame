using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class RegionMemberLifetimeBehavior : RegionBehavior
    {
        public const string BehaviorKey = "RegionMemberLifetimeBehavior";

        protected override void OnAttach()
        {
            this.Region.ActiveViews.CollectionChanged += this.OnActiveViewsChanged;
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove) 
                return;

            var inactiveViews = e.OldItems;
            foreach (var inactiveView in inactiveViews)
            {
                if (!ShouldKeepAlive(inactiveView))
                {
                    this.Region.Remove(inactiveView);
                }
            }
        }

        private static bool ShouldKeepAlive(object inactiveView)
        {
            IRegionMemberLifetime lifetime = GetItemOrContextLifetime(inactiveView);
            if (lifetime != null)
            {
                return lifetime.KeepAlive;
            }

            RegionMemberLifetimeAttribute lifetimeAttribute = GetItemOrContextLifetimeAttribute(inactiveView);
            if (lifetimeAttribute != null)
            {
                return lifetimeAttribute.KeepAlive;
            }

            return true;
        }

        private static RegionMemberLifetimeAttribute GetItemOrContextLifetimeAttribute(object inactiveView)
        {
            var lifetimeAttribute = GetCustomAttributes<RegionMemberLifetimeAttribute>(inactiveView.GetType()).FirstOrDefault();
            if (lifetimeAttribute != null)
            {
                return lifetimeAttribute;
            }

            var frameworkElement = inactiveView as System.Windows.FrameworkElement;
            if (frameworkElement != null && frameworkElement.DataContext != null)
            {
                var dataContext = frameworkElement.DataContext;
                var contextLifetimeAttribute =
                    GetCustomAttributes<RegionMemberLifetimeAttribute>(dataContext.GetType()).FirstOrDefault();
                return contextLifetimeAttribute;
            }

            return null;
        }

        private static IRegionMemberLifetime GetItemOrContextLifetime(object inactiveView)
        {
            var regionLifetime = inactiveView as IRegionMemberLifetime;
            if (regionLifetime != null)
            {
                return regionLifetime;
            }

            var frameworkElement = inactiveView as System.Windows.FrameworkElement;
            if (frameworkElement != null)
            {
                return frameworkElement.DataContext as IRegionMemberLifetime;
            }

            return null;
        }

        private static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }
    }
}
