using System;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Frame.OS.Modularity.Exceptions
{
    public class ModularityException : Exception
    {
        public ModularityException()
            : this(null)
        {
        }

        public ModularityException(string message)
            : this(null, message)
        {
        }

        public ModularityException(string message, Exception innerException)
            : this(null, message, innerException)
        {
        }

        public ModularityException(string moduleName, string message)
            : this(moduleName, message, null)
        {
        }

        public ModularityException(string moduleName, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ModuleName = moduleName;
        }

        protected ModularityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ModuleName = info.GetValue("ModuleName", typeof(string)) as string;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleName", this.ModuleName);
        }

        public string ModuleName { get; set; }
    }
}
