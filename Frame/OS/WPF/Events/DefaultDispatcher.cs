using System;
using System.Windows;
using System.Windows.Threading;

namespace Frame.OS.WPF.Events
{
    public class DefaultDispatcher : IDispatcherFacade
    {
        public void BeginInvoke(Delegate method, object arg)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method, arg);
            }
        }
    }
}
