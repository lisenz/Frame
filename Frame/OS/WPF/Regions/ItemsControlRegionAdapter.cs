using System;
using System.Windows.Data;
using System.Windows.Controls;

namespace Frame.OS.WPF.Regions
{
    public class ItemsControlRegionAdapter : RegionAdapterBase<ItemsControl>
    {
        public ItemsControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, ItemsControl regionTarget)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");

            bool itemsSourceIsSet = regionTarget.ItemsSource != null;
            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException("ItemsControl's ItemsSource属性不为空."
                    + "该控件被关联到一个Region中, 但该控件已经被绑定有信息."
                    + "如果没有显式地设置控件的ItemSource属性,这个异常也许是因为继承RegionManager附加属性的值发生改变所导致.");
            }

            // 如果该控件已有子项，添加到部件中，如果子项已存在部件中则不可设置ItemsSource属性
            if (regionTarget.Items.Count > 0)
            {
                foreach (object childItem in regionTarget.Items)
                {
                    region.Add(childItem);
                }
                // 设置ItemsSource属性之前该控件的子项列表必须为空
                regionTarget.Items.Clear();
            }

            regionTarget.ItemsSource = region.Views;
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}
