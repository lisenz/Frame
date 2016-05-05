using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Collections.Specialized;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class RegionActiveAwareBehavior : IRegionBehavior
    {
        public const string BehaviorKey = "ActiveAware";

        public IRegion Region { get; set; }

        public void Attach()
        {
            INotifyCollectionChanged collection = this.GetCollection();
            if (collection != null)
            {
                collection.CollectionChanged += OnCollectionChanged;
            }
        }

        public void Detach()
        {
            INotifyCollectionChanged collection = this.GetCollection();
            if (collection != null)
            {
                collection.CollectionChanged -= OnCollectionChanged;
            }
        }

        private static void InvokeOnActiveAwareElement(object item, Action<IActiveAware> invocation)
        {
            var activeAware = item as IActiveAware;
            if (activeAware != null)
            {
                invocation(activeAware);
            }

            var frameworkElement = item as FrameworkElement;
            if (frameworkElement != null)
            {
                var activeAwareDataContext = frameworkElement.DataContext as IActiveAware;
                if (activeAwareDataContext != null)
                {
                    invocation(activeAwareDataContext);
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    Action<IActiveAware> invocation = activeAware => activeAware.IsActive = true;

                    InvokeOnActiveAwareElement(item, invocation);
                    InvokeOnSynchronizedActiveAwareChildren(item, invocation);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    Action<IActiveAware> invocation = activeAware => activeAware.IsActive = false;

                    InvokeOnActiveAwareElement(item, invocation);
                    InvokeOnSynchronizedActiveAwareChildren(item, invocation);
                }
            }
        }

        private void InvokeOnSynchronizedActiveAwareChildren(object item, Action<IActiveAware> invocation)
        {
            var dependencyObjectView = item as DependencyObject;

            if (dependencyObjectView != null)
            {
                var regionManager = RegionManager.GetRegionManager(dependencyObjectView);
                if (regionManager == null || regionManager == this.Region.RegionManager) 
                    return;

                var activeViews = regionManager.Regions.SelectMany(e => e.ActiveViews);
                var syncActiveViews = activeViews.Where(ShouldSyncActiveState);

                foreach (var syncActiveView in syncActiveViews)
                {
                    InvokeOnActiveAwareElement(syncActiveView, invocation);
                }
            }
        }

        private bool ShouldSyncActiveState(object view)
        {
            if (Attribute.IsDefined(view.GetType(), typeof(SyncActiveStateAttribute)))
            {
                return true;
            }

            var viewAsFrameworkElement = view as FrameworkElement;

            if (viewAsFrameworkElement != null)
            {
                var viewModel = viewAsFrameworkElement.DataContext;

                return viewModel != null && Attribute.IsDefined(viewModel.GetType(), typeof(SyncActiveStateAttribute));
            }

            return false;
        }

        private INotifyCollectionChanged GetCollection()
        {
            return this.Region.ActiveViews;
        }
    }
}
