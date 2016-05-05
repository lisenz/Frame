using System;
using System.Runtime.Serialization;

namespace Frame.OS.Modularity.Exceptions
{
    public class CyclicDependencyFoundException : ModularityException
    {
        public CyclicDependencyFoundException() : base() { }

        public CyclicDependencyFoundException(string message) : base(message) { }

        public CyclicDependencyFoundException(string message, Exception innerException) : base(message, innerException) { }

        public CyclicDependencyFoundException(string moduleName, string message, Exception innerException) : base(moduleName, message, innerException) { }

        protected CyclicDependencyFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
