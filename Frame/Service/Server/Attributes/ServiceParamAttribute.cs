using System;

namespace Frame.Service.Server.Attributes
{
    /// <summary>
    /// 表示一个参数特性，标记在作为服务方法的参数对象上。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ServiceParamAttribute : Attribute
    {
        /// <summary>
        /// 设置服务方法的参数名称。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        public ServiceParamAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 获取服务方法的参数名称。
        /// </summary>
        public string Name { get; private set; }
    }
}
