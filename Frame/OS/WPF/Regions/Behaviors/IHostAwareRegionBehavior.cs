using System.Windows;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public interface IHostAwareRegionBehavior : IRegionBehavior
    {
        DependencyObject HostControl { get; set; }
    }
}
