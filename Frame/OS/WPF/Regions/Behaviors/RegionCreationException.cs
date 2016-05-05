using System;
using System.Runtime.Serialization;

namespace Frame.OS.WPF.Regions.Behaviors
{
    [Serializable]
    public class RegionCreationException : Exception
    {
        public RegionCreationException()
        {
        }

        public RegionCreationException(string message)
            : base(message)
        {
        }

        public RegionCreationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RegionCreationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
