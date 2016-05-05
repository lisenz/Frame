using Frame.OS.WPF.Regions.Behaviors;
using System.Windows.Controls.Primitives;

namespace Frame.OS.WPF.Regions
{
    public class SelectorRegionAdapter : RegionAdapterBase<Selector>
    {
        public SelectorRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, Selector regionTarget)
        {
        }

        protected override void AttachBehaviors(IRegion region, Selector regionTarget)
        {
            if (region == null) throw new System.ArgumentNullException("region");
            // Add the behavior that syncs the items source items with the rest of the items
            region.Behaviors.Add(SelectorItemsSourceSyncBehavior.BehaviorKey, new SelectorItemsSourceSyncBehavior()
            {
                HostControl = regionTarget
            });

            base.AttachBehaviors(region, regionTarget);
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}
