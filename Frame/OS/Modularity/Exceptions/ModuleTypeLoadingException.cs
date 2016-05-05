using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Frame.OS.Modularity.Exceptions
{
    public class ModuleTypeLoadingException : ModularityException
    {
        public ModuleTypeLoadingException()
            : base()
        {
        }

        public ModuleTypeLoadingException(string message)
            : base(message)
        {
        }

        public ModuleTypeLoadingException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public ModuleTypeLoadingException(string moduleName, string message)
            : this(moduleName, message, null)
        {
        }

        public ModuleTypeLoadingException(string moduleName, string message, Exception innerException)
            : base(moduleName, String.Format(CultureInfo.CurrentCulture, "未能加载模块{0}."
                + "如果这个错误发生在Silverlight应用程序中使用MEF,"
                + "请确保在主应用程序或壳中MefExtensions程序集中CopyLocal属性的引用被设置为true，"
                + "而在其他所有组件中为false.错误信息为: {1}.",
            moduleName, message), innerException)
        {
        }

        protected ModuleTypeLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
