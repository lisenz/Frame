using System;
using System.Threading;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;

namespace Frame.OS.Window.Regions
{
    public static class RegionManagerExtensions
    {
        public static IRegionManager AddToRegion(this IRegionManager regionManager, string regionName, object view)
        {
            if (regionManager == null) throw new ArgumentNullException("regionManager");

            if (!regionManager.Regions.ContainsRegionWithName(regionName))
            {
                throw new ArgumentException(
                    string.Format(Thread.CurrentThread.CurrentCulture,
                    "该部件管理器不包括部件'{0}'.",
                    regionName), "regionName");
            }

            IRegion region = regionManager.Regions[regionName];

            return region.Add(view);
        }

        public static IRegionManager RegisterViewWithRegion(this IRegionManager regionManager, string regionName, Type viewType)
        {
            var regionViewRegistry = ServiceLocator.Current.GetInstance<IRegionViewRegistry>();

            regionViewRegistry.RegisterViewWithRegion(regionName, viewType);

            return regionManager;
        }

        public static IRegionManager RegisterViewWithRegion(this IRegionManager regionManager, string regionName, Func<object> getContentDelegate)
        {
            var regionViewRegistry = ServiceLocator.Current.GetInstance<IRegionViewRegistry>();

            regionViewRegistry.RegisterViewWithRegion(regionName, getContentDelegate);

            return regionManager;
        }

        public static void Add(this IRegionCollection regionCollection, string regionName, IRegion region)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionCollection == null) throw new ArgumentNullException("regionCollection");

            if (region.Name != null && region.Name != regionName)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "该部件 '{0}'已经在管理器中进行注册, 无法使用另外的名称 ('{1}')添加到管理器中.",
                    region.Name, regionName), "regionName");
            }

            if (region.Name == null)
            {
                region.Name = regionName;
            }

            regionCollection.Add(region);
        }

    }
}
