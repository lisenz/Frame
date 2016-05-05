using System.Collections.Generic;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class AutoPopulateRegionBehavior : RegionBehavior
    {
        public const string BehaviorKey = "AutoPopulate";

        private readonly IRegionViewRegistry _RegionViewRegistry;

        public AutoPopulateRegionBehavior(IRegionViewRegistry regionViewRegistry)
        {
            this._RegionViewRegistry = regionViewRegistry;
        }

        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged += this.Region_PropertyChanged;
            }
            else
            {
                this.StartPopulatingContent();
            }
        }

        private void StartPopulatingContent()
        {
            foreach (object view in this.CreateViewsToAutoPopulate())
            {
                AddViewIntoRegion(view);
            }

            this._RegionViewRegistry.ContentRegistered += this.OnViewRegistered;
        }

        protected virtual IEnumerable<object> CreateViewsToAutoPopulate()
        {
            return this._RegionViewRegistry.GetContents(this.Region.Name);
        }

        protected virtual void AddViewIntoRegion(object viewToAdd)
        {
            this.Region.Add(viewToAdd);
        }

        private void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && !string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged -= this.Region_PropertyChanged;
                this.StartPopulatingContent();
            }
        }

        public virtual void OnViewRegistered(object sender, ViewRegisteredEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            if (e.RegionName == this.Region.Name)
            {
                AddViewIntoRegion(e.GetView());
            }
        }
    }
}
