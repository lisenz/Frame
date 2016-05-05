using System;
using System.Collections.Generic;

namespace Frame.OS.WPF.Regions
{
    public interface IRegionBehaviorFactory : IEnumerable<string>
    {
        void AddIfMissing(string behaviorKey, Type behaviorType);

        bool ContainsKey(string behaviorKey);

        IRegionBehavior CreateFromKey(string key);
    }
}
