using System;
using System.Runtime.Serialization;

namespace Frame.OS.WPF.Regions
{
    [Serializable]
    public class UpdateRegionsException : Exception
    {
        public UpdateRegionsException()
        {
        }

        public UpdateRegionsException(string message)
            : base(message)
        {
        }

        public UpdateRegionsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected UpdateRegionsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
