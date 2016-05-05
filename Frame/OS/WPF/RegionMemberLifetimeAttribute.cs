using System;

namespace Frame.OS.WPF
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public sealed class RegionMemberLifetimeAttribute : Attribute
    {
        public RegionMemberLifetimeAttribute()
        {
            KeepAlive = true;
        }

        public bool KeepAlive { get; set; }
    }
}
