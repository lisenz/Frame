namespace Frame.OS.Window.Regions
{
    public interface IRegionManager
    {
        IRegionCollection Regions { get; }
        IRegionManager CreateRegionManager();
    }
}
