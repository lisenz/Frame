using System;

namespace Frame.OS.WPF.Events
{
    public class DataEventArgs<TData> : EventArgs
    {
        private readonly TData _Value;

        public DataEventArgs(TData value)
        {
            _Value = value;
        }

        public TData Value
        {
            get { return _Value; }
        }
    }
}
