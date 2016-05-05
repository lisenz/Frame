using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace Frame.OS.WPF.Regions
{
    public class ContentControlRegionAdapter : RegionAdapterBase<ContentControl>
    {
        public ContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, ContentControl regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            bool contentIsSet = regionTarget.Content != null;
            if (contentIsSet)
            {
                throw new InvalidOperationException("ContentControl的Content属性已有内容. "
                    + "该控件被关联到一个Region中, 但该控件已经被绑定有信息."
                    + "如果没有显式地设置控件的Content属性,这个异常也许是因为继承RegionManager附加属性的值发生改变所导致.");
            }

            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault();
            };

            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                        region.Activate(e.NewItems[0]);
                    }
                };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
