using System;

namespace Frame.OS
{
    public interface IDelegateReference
    {
        Delegate Target { get; }
    }
}
