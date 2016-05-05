using System;

namespace Frame.Core.Ioc
{
    public class ObjectContainerException : Exception
    {
        internal ObjectContainerException()
        {
        }

        internal ObjectContainerException(string message)
            : base(message)
        {
        }

        internal ObjectContainerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
