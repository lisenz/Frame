using System;
using System.Runtime.Serialization;

namespace Frame.OS
{
    public class ViewRegistrationException : Exception
    {
        public ViewRegistrationException()
        {
        }

        public ViewRegistrationException(string message)
            : base(message)
        {
        }

        public ViewRegistrationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ViewRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
