using System.Linq;

namespace Frame.OS.WPF.Regions
{
    public class SingleActiveRegion : Region
    {
        public override void Activate(object view)
        {
            object currentActiveView = ActiveViews.FirstOrDefault();

            if (currentActiveView != null && currentActiveView != view && this.Views.Contains(currentActiveView))
            {
                base.Deactivate(currentActiveView);
            }
            base.Activate(view);
        }
    }
}
