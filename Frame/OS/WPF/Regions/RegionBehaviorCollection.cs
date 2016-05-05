using System;
using System.Collections;
using System.Collections.Generic;

namespace Frame.OS.WPF.Regions
{
    public class RegionBehaviorCollection : IRegionBehaviorCollection
    {
        private readonly IRegion _Region;
        private readonly Dictionary<string, IRegionBehavior> _Behaviors = new Dictionary<string, IRegionBehavior>();

        public RegionBehaviorCollection(IRegion region)
        {
            this._Region = region;
        }

        #region 实现接口IRegionBehaviorCollection

        public void Add(string key, IRegionBehavior regionBehavior)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (regionBehavior == null)
                throw new ArgumentNullException("regionBehavior");

            if (this._Behaviors.ContainsKey(key))
                throw new ArgumentException("部件列表中不能重复添加同一个键值.", "key");

            this._Behaviors.Add(key, regionBehavior);
            regionBehavior.Region = this._Region;

            regionBehavior.Attach();
        }

        public bool ContainsKey(string key)
        {
            return this._Behaviors.ContainsKey(key);
        }

        public IRegionBehavior this[string key]
        {
            get { return this._Behaviors[key]; }
        }

        public IEnumerator<KeyValuePair<string, IRegionBehavior>> GetEnumerator()
        {
            return this._Behaviors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Behaviors.GetEnumerator();
        }

        #endregion
    }
}
