using System;

namespace Frame.Service.Server.Attributes
{
    /// <summary>
    /// 表示一个服务特性，标记在需要作为服务提供接口的类或Web应用处理程序。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// 设置服务名称。
        /// </summary>
        /// <param name="name">服务的名称。</param>
        public ServiceAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取服务的名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取或设置服务路由的URL路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 获取或设置服务的默认执行方法的名称。
        /// </summary>
        public string DefaultMethod { get; set; }
    }
}
