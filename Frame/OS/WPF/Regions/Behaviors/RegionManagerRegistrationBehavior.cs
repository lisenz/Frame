using System;
using System.Windows;
using System.ComponentModel;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class RegionManagerRegistrationBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public static readonly string BehaviorKey = "RegionManagerRegistration";

        private WeakReference _AttachedRegionManagerWeakReference;
        private DependencyObject _HostControl;

        public RegionManagerRegistrationBehavior()
        {
            this.RegionManagerAccessor = new DefaultRegionManagerAccessor();
        }

        public IRegionManagerAccessor RegionManagerAccessor { get; set; }

        public DependencyObject HostControl
        {
            get
            {
                return _HostControl;
            }
            set
            {
                if (IsAttached)
                {
                    throw new InvalidOperationException("HostControl属性不能在Attach方法被调用后进行设置值.");
                }
                this._HostControl = value;
            }
        }

        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged += this.Region_PropertyChanged;
            }
            else
            {
                this.StartMonitoringRegionManager();
            }
        }

        private void Region_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && !string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged -= this.Region_PropertyChanged;
                this.StartMonitoringRegionManager();
            }
        }

        private void StartMonitoringRegionManager()
        {
            this.RegionManagerAccessor.UpdatingRegions += this.OnUpdatingRegions;
            this.TryRegisterRegion();
        }

        private void TryRegisterRegion()
        {
            DependencyObject targetElement = this.HostControl;
            if (targetElement.CheckAccess())
            {
                IRegionManager regionManager = this.FindRegionManager(targetElement);

                IRegionManager attachedRegionManager = this.GetAttachedRegionManager();

                if (regionManager != attachedRegionManager)
                {
                    if (attachedRegionManager != null)
                    {
                        this._AttachedRegionManagerWeakReference = null;
                        attachedRegionManager.Regions.Remove(this.Region.Name);
                    }

                    if (regionManager != null)
                    {
                        this._AttachedRegionManagerWeakReference = new WeakReference(regionManager);
                        regionManager.Regions.Add(this.Region);
                    }
                }
            }
        }

        public void OnUpdatingRegions(object sender, EventArgs e)
        {
            this.TryRegisterRegion();
        }

        private IRegionManager FindRegionManager(DependencyObject dependencyObject)
        {
            var regionmanager = this.RegionManagerAccessor.GetRegionManager(dependencyObject);
            if (regionmanager != null)
            {
                return regionmanager;
            }

            DependencyObject parent = null;
            parent = LogicalTreeHelper.GetParent(dependencyObject);
            if (parent != null)
            {
                return this.FindRegionManager(parent);
            }

            return null;
        }

        private IRegionManager GetAttachedRegionManager()
        {
            if (this._AttachedRegionManagerWeakReference != null)
            {
                return this._AttachedRegionManagerWeakReference.Target as IRegionManager;
            }

            return null;
        }
    }
}
