using System.Windows.Controls;

namespace Frame.OS.WPF.Regions
{
    public interface IRegionManager
    {
        IRegionCollection Regions { get; }
        IRegionManager CreateRegionManager();
    }
}
