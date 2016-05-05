using System;
using System.Windows;
using System.ComponentModel;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class SyncRegionContextWithHostBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        private const string _RegionContextPropertyName = "Context";
        private DependencyObject _HostControl;

        public static readonly string BehaviorKey = "SyncRegionContextWithHost";

        private ObservableObject<object> HostControlRegionContext
        {
            get
            {
                return RegionContext.GetObservableContext(this._HostControl);
            }
        }

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
                    throw new InvalidOperationException("HostControl 属性不能在Attach方法被调用后设置值.");
                }
                this._HostControl = value;
            }
        }

        protected override void OnAttach()
        {
            if (this.HostControl != null)
            {
                SynchronizeRegionContext();

                this.HostControlRegionContext.PropertyChanged += this.RegionContextObservableObject_PropertyChanged;
                this.Region.PropertyChanged += this.Region_PropertyChanged;
            }
        }

        void Region_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _RegionContextPropertyName)
            {
                if (RegionManager.GetRegionContext(this.HostControl) != this.Region.Context)
                {
                    RegionManager.SetRegionContext(this._HostControl, this.Region.Context);
                }
            }
        }

        private void RegionContextObservableObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                SynchronizeRegionContext();
        }

        private void SynchronizeRegionContext()
        {
            if (this.Region.Context != this.HostControlRegionContext.Value)
                this.Region.Context = this.HostControlRegionContext.Value;

            if (RegionManager.GetRegionContext(this.HostControl) != this.HostControlRegionContext.Value)
                RegionManager.SetRegionContext(this.HostControl, this.HostControlRegionContext.Value);
        }
    }
}
