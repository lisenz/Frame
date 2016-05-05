using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Frame.OS.Modularity.Exceptions
{
    public class ModuleInitializeException : ModularityException
    {
        public ModuleInitializeException()
        {
        }

        public ModuleInitializeException(string message)
            : base(message)
        {
        }

        public ModuleInitializeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ModuleInitializeException(string moduleName, string moduleAssembly, string message)
            : this(moduleName, message, moduleAssembly, null)
        {
        }

        public ModuleInitializeException(string moduleName, string moduleAssembly, string message, Exception innerException)
            : base(
                moduleName,
                String.Format(CultureInfo.CurrentCulture, "当正在加载模块 '{0}'时发生异常."
                    + "- 异常信息为: {2} - 组装的模块试图从程序集:{1} 进行加载."
                    + "检查内部异常以获取更多信息. "
                    + "如果异常发生在一个DI容器中创建一个对象, "
                    + "你可以通过exception.GetRootException() 来辅助找到问题的根源.",
            moduleName, moduleAssembly, message),
                innerException)
        {
        }

        public ModuleInitializeException(string moduleName, string message, Exception innerException)
            : base(
                moduleName,
                String.Format(CultureInfo.CurrentCulture, "当正在加载模块 '{0}'时发生异常."
                    + "- 异常信息为: {2} - 组装的模块试图从程序集:{1} 进行加载."
                    + "检查内部异常以获取更多信息. "
                    + "如果异常发生在一个DI容器中创建一个对象, "
                    + "你可以通过exception.GetRootException() 来辅助找到问题的根源.",
            moduleName, message),
                innerException)
        {
        }

        protected ModuleInitializeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
