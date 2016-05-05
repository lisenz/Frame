namespace Frame.OS.WPF.Regions
{
    public interface IRegionAdapter
    {
        IRegion Initialize(object regionTarget, string regionName);
    }
}
