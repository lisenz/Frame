using System;

namespace Frame.OS.WPF
{
    public interface IActiveAware
    {
        bool IsActive { get; set; }

        event EventHandler IsActiveChanged;
    }
}
