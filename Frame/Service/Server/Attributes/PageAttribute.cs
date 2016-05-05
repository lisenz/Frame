using System;

namespace Frame.Service.Server.Attributes
{
    /// <summary>
    /// 表示一个页面特性，标记在需要作为服务提供接口的类或Web应用处理程序。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PageAttribute : Attribute
    {
        /// <summary>
        /// 设置页面名称。
        /// </summary>
        /// <param name="name">页面的名称。</param>
        /// <param name="template">页面的模版文件名称。</param>
        public PageAttribute(string name, string template)
        {
            Name = name;
            Template = template;
        }

        /// <summary>
        /// 获取页面的名称。
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// 页面对应的View模版文件名称。
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 获取或设置页面路由的URL路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 获取或设置页面的默认执行方法的名称。
        /// </summary>
        public string DefaultAction { get; set; }
    }
}
