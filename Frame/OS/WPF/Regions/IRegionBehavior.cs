namespace Frame.OS.WPF.Regions
{
    public interface IRegionBehavior
    {
        IRegion Region { get; set; }
        void Attach();
    }
}
