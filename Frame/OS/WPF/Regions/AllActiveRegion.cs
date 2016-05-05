using System;

namespace Frame.OS.WPF.Regions
{
    public class AllActiveRegion : Region
    {
        public override IViewsCollection ActiveViews
        {
            get { return Views; }
        }

        public override void Deactivate(object view)
        {
            throw new InvalidOperationException("该类型的部件不可进行Deactivate操作.");
        }
    }
}
