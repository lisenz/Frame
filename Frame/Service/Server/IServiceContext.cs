using System.Collections.Generic;

namespace Frame.Service.Server
{
    /// <summary>
    /// 服务上下文对象接口。
    /// </summary>
    public interface IServiceContext
    {
        /// <summary>
        /// 获取路由地址的URL模式。
        /// </summary>
        string Path { get; }

        /// <summary>
        /// 获取服务名称。
        /// </summary>
        string Service { get; }

        /// <summary>
        /// 获取服务中的方法名称
        /// </summary>
        string Method { get; }

        /// <summary>
        /// 获取方法的参数集合。
        /// </summary>
        IDictionary<string, object> Params { get; }
    }
}
