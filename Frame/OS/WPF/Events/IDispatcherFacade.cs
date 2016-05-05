using System;

namespace Frame.OS.WPF.Events
{
    public interface IDispatcherFacade
    {
        void BeginInvoke(Delegate method, object arg);
    }
}
