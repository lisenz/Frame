using System;

namespace Frame.Service.Server.Attributes
{
    /// <summary>
    /// 表示一个方法特性，标记在服务类的方法体上。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServiceMethodAttribute : Attribute
    {

    }
}
