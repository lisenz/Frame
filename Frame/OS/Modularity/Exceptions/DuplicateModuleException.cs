using System;
using System.Runtime.Serialization;

namespace Frame.OS.Modularity.Exceptions
{
    /// <summary>
    /// 表示出现重复模块时的异常。
    /// </summary>
    public class DuplicateModuleException : ModularityException
    {
        public DuplicateModuleException() { }

        public DuplicateModuleException(string message) : base(message) { }

        public DuplicateModuleException(string message, Exception innerException) : base(message, innerException) { }

        public DuplicateModuleException(string moduleName, string message) : base(moduleName, message) { }

        public DuplicateModuleException(string moduleName, string message, Exception innerException) : base(moduleName, message, innerException) { }

        protected DuplicateModuleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
