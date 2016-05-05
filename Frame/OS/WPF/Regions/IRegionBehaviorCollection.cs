using System.Collections.Generic;

namespace Frame.OS.WPF.Regions
{
    public interface IRegionBehaviorCollection : IEnumerable<KeyValuePair<string, IRegionBehavior>>
    {
        void Add(string key, IRegionBehavior regionBehavior);
        bool ContainsKey(string key);

        IRegionBehavior this[string key] { get; }
    }
}
