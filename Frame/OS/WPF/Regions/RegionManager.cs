using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Practices.ServiceLocation;
using Frame.Core.Extensions;
using Frame.OS.WPF.Regions.Behaviors;

namespace Frame.OS.WPF.Regions
{
    public class RegionManager : IRegionManager
    {
        private readonly RegionCollection _RegionCollection;
        private static readonly WeakDelegatesManager _UpdatingRegionsListeners = new WeakDelegatesManager();
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(RegionManager),
            new PropertyMetadata(OnSetRegionNameCallback));
        private static void OnSetRegionNameCallback(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (!IsInDesignMode(element))
            {
                CreateRegion(element);
            }
        }

        private static readonly DependencyProperty ObservableRegionProperty = DependencyProperty.RegisterAttached(
            "ObservableRegion", 
            typeof(ObservableObject<IRegion>), 
            typeof(RegionManager), 
            null);
        
        public static readonly DependencyProperty RegionManagerProperty = DependencyProperty.RegisterAttached(
            "RegionManager", 
            typeof(IRegionManager), 
            typeof(RegionManager), 
            null);

        public static readonly DependencyProperty RegionContextProperty =  DependencyProperty.RegisterAttached(
            "RegionContext", 
            typeof(object), 
            typeof(RegionManager), 
            new PropertyMetadata(OnRegionContextChanged));
        private static void OnRegionContextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (RegionContext.GetObservableContext(depObj).Value != e.NewValue)
            {
                RegionContext.GetObservableContext(depObj).Value = e.NewValue;
            }
        }

        public static void SetRegionName(DependencyObject regionTarget, string regionName)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            regionTarget.SetValue(RegionNameProperty, regionName);
        }

        public static string GetRegionName(DependencyObject regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            return regionTarget.GetValue(RegionNameProperty) as string;
        }

        public static IRegionManager GetRegionManager(DependencyObject target)
        {
            if (target == null) throw new ArgumentNullException("target");
            return (IRegionManager)target.GetValue(RegionManagerProperty);
        }

        public static void SetRegionManager(DependencyObject target, IRegionManager value)
        {
            if (target == null) throw new ArgumentNullException("target");
            target.SetValue(RegionManagerProperty, value);
        }

        public static object GetRegionContext(DependencyObject target)
        {
            if (target == null) throw new ArgumentNullException("target");
            return target.GetValue(RegionContextProperty);
        }

        public static void SetRegionContext(DependencyObject target, object value)
        {
            if (target == null) throw new ArgumentNullException("target");
            target.SetValue(RegionContextProperty, value);
        }

        public static ObservableObject<IRegion> GetObservableRegion(DependencyObject view)
        {
            if (view == null) 
                throw new ArgumentNullException("view");

            ObservableObject<IRegion> regionWrapper = view.GetValue(ObservableRegionProperty) as ObservableObject<IRegion>;

            if (regionWrapper == null)
            {
                regionWrapper = new ObservableObject<IRegion>();
                view.SetValue(ObservableRegionProperty, regionWrapper);
            }

            return regionWrapper;
        }

        private static void CreateRegion(DependencyObject element)
        {
            IServiceLocator locator = ServiceLocator.Current;
            DelayedRegionCreationBehavior regionCreationBehavior = locator.GetInstance<DelayedRegionCreationBehavior>();
            regionCreationBehavior.TargetElement = element;
            regionCreationBehavior.Attach();
        }

        public static event EventHandler UpdatingRegions
        {
            add { _UpdatingRegionsListeners.AddListener(value); }
            remove { _UpdatingRegionsListeners.RemoveListener(value); }
        }

        public static void UpdateRegions()
        {
            try
            {
                _UpdatingRegionsListeners.Raise(null, EventArgs.Empty);
            }
            catch (TargetInvocationException ex)
            {
                Exception rootException = ex.GetRootException();

                throw new UpdateRegionsException(string.Format("An exception occurred while trying to create region objects.- The most likely causing exception was: '{0}'.But also check the InnerExceptions for more detail or call .GetRootException(). ",
                    rootException), ex.InnerException);
            }
        }

        private static bool IsInDesignMode(DependencyObject element)
        {
            // Due to a known issue in Cider, GetIsInDesignMode attached property value is not enough to know if it's in design mode.
            return DesignerProperties.GetIsInDesignMode(element) || Application.Current == null
                   || Application.Current.GetType() == typeof(Application);
        }

        #region 实现接口IRegionManager

        public RegionManager()
        {
            this._RegionCollection = new RegionCollection(this);
        }

        public IRegionCollection Regions
        {
            get { return this._RegionCollection; }
        }

        public IRegionManager CreateRegionManager()
        {
            return new RegionManager();
        }

        #endregion

        private class RegionCollection : IRegionCollection
        {
            private readonly IRegionManager regionManager;
            private readonly List<IRegion> regions;

            public RegionCollection(IRegionManager regionManager)
            {
                this.regionManager = regionManager;
                this.regions = new List<IRegion>();
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public IEnumerator<IRegion> GetEnumerator()
            {
                UpdateRegions();

                return this.regions.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IRegion this[string regionName]
            {
                get
                {
                    UpdateRegions();

                    IRegion region = GetRegionByName(regionName);
                    if (region == null)
                    {
                        throw new KeyNotFoundException(string.Format("The region manager does not contain the {0} region.", regionName));
                    }

                    return region;
                }
            }

            public void Add(IRegion region)
            {
                if (region == null) throw new ArgumentNullException("region");
                UpdateRegions();

                if (region.Name == null)
                {
                    throw new InvalidOperationException("The region name cannot be null or empty.");
                }

                if (this.GetRegionByName(region.Name) != null)
                {
                    throw new ArgumentException(string.Format("Region with the given name is already registered: {0}", region.Name));
                }

                this.regions.Add(region);
                region.RegionManager = this.regionManager;

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, region, 0));
            }

            public bool Remove(string regionName)
            {
                UpdateRegions();

                bool removed = false;

                IRegion region = GetRegionByName(regionName);
                if (region != null)
                {
                    removed = true;
                    this.regions.Remove(region);
                    region.RegionManager = null;

                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, region, 0));
                }

                return removed;
            }

            public bool ContainsRegionWithName(string regionName)
            {
                UpdateRegions();

                return GetRegionByName(regionName) != null;
            }

            private IRegion GetRegionByName(string regionName)
            {
                return this.regions.FirstOrDefault(r => r.Name == regionName);
            }

            private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
            {
                var handler = this.CollectionChanged;

                if (handler != null)
                {
                    handler(this, notifyCollectionChangedEventArgs);
                }
            }
        }
    }
}
