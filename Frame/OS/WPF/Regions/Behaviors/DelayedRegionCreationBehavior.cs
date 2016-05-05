using System;
using System.Windows;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class DelayedRegionCreationBehavior
    {
        private readonly RegionAdapterMappings _RegionAdapterMappings;
        private WeakReference _ElementWeakReference;
        private bool _RegionCreated;

        public DelayedRegionCreationBehavior(RegionAdapterMappings regionAdapterMappings)
        {
            this._RegionAdapterMappings = regionAdapterMappings;
            this.RegionManagerAccessor = new DefaultRegionManagerAccessor();
        }

        public IRegionManagerAccessor RegionManagerAccessor { get; set; }

        public DependencyObject TargetElement
        {
            get { return this._ElementWeakReference != null ? this._ElementWeakReference.Target as DependencyObject : null; }
            set { this._ElementWeakReference = new WeakReference(value); }
        }

        public void Attach()
        {
            this.RegionManagerAccessor.UpdatingRegions += this.OnUpdatingRegions;
            this.WireUpTargetElement();
        }

        public void Detach()
        {
            this.RegionManagerAccessor.UpdatingRegions -= this.OnUpdatingRegions;
            this.UnWireTargetElement();
        }

        public void OnUpdatingRegions(object sender, EventArgs e)
        {
            this.TryCreateRegion();
        }

        private void TryCreateRegion()
        {
            DependencyObject targetElement = this.TargetElement;
            if (targetElement == null)
            {
                this.Detach();
                return;
            }

            if (targetElement.CheckAccess())
            {
                this.Detach();

                if (!this._RegionCreated)
                {
                    string regionName = this.RegionManagerAccessor.GetRegionName(targetElement);
                    CreateRegion(targetElement, regionName);
                    this._RegionCreated = true;
                }
            }
        }

        protected virtual IRegion CreateRegion(DependencyObject targetElement, string regionName)
        {
            if (targetElement == null) throw new ArgumentNullException("targetElement");
            try
            {
                // Build the region
                IRegionAdapter regionAdapter = this._RegionAdapterMappings.GetMapping(targetElement.GetType());
                IRegion region = regionAdapter.Initialize(targetElement, regionName);

                return region;
            }
            catch (Exception ex)
            {
                throw new RegionCreationException(string.Format("An exception occurred while creating a region with name '{0}'. The exception was: {1}. ",
                    regionName, ex), ex);
            }
        }

        private void ElementLoaded(object sender, RoutedEventArgs e)
        {
            this.UnWireTargetElement();
            this.TryCreateRegion();
        }

        private void WireUpTargetElement()
        {
            FrameworkElement element = this.TargetElement as FrameworkElement;
            if (element != null)
            {
                element.Loaded += this.ElementLoaded;
            }
        }

        private void UnWireTargetElement()
        {
            FrameworkElement element = this.TargetElement as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= this.ElementLoaded;
            }
        }
    }
}
