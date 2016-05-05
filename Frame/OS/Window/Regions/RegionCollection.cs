using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace Frame.OS.Window.Regions
{
    internal class RegionCollection : IRegionCollection
    {
        private readonly IRegionManager regionManager;
        private readonly List<IRegion> regions;

        public RegionCollection(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            this.regions = new List<IRegion>();
        }
        
        public IEnumerator<IRegion> GetEnumerator()
        {
            return this.regions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IRegion this[string regionName]
        {
            get
            {
                IRegion region = GetRegionByName(regionName);
                if (region == null)
                {
                    throw new KeyNotFoundException(string.Format(CultureInfo.CurrentUICulture, "该部件管理器不包括部件{0}.", regionName));
                }

                return region;
            }
        }

        public void Add(IRegion region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            if (region.Name == null)
            {
                throw new InvalidOperationException("部件名称不能为空.");
            }

            if (this.GetRegionByName(region.Name) != null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                                          "部件{0}已经被注册在管理器中.", region.Name));
            }

            this.regions.Add(region);
            region.RegionManager = this.regionManager;
        }

        public bool Remove(string regionName)
        {
            bool removed = false;

            IRegion region = GetRegionByName(regionName);
            if (region != null)
            {
                removed = true;
                this.regions.Remove(region);
                region.RegionManager = null;

            }

            return removed;
        }

        public bool ContainsRegionWithName(string regionName)
        {
            return GetRegionByName(regionName) != null;
        }

        private IRegion GetRegionByName(string regionName)
        {
            return this.regions.FirstOrDefault(r => r.Name == regionName);
        }
    }
}
