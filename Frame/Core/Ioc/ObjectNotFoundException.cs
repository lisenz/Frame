using System;

namespace Frame.Core.Ioc
{
    public sealed class ObjectNotFoundException : Exception
    {
        internal ObjectNotFoundException()
        {
        }

        internal ObjectNotFoundException(string message)
            : base(message)
        {
        }

        internal ObjectNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
