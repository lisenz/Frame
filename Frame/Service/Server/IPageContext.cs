using System.Collections.Generic;

namespace Frame.Service.Server
{
    /// <summary>
    /// 请求页面的当前上下文对象接口
    /// </summary>
    public interface IPageContext
    {
        /// <summary>
        /// 获取请求页面地址的URL模式。
        /// </summary>
        string Path { get; }

        /// <summary>
        /// 获取页面名称。
        /// </summary>
        string Page { get; }

        /// <summary>
        /// 获取页面中的方法名称
        /// </summary>
        string Action { get; }

        /// <summary>
        /// 获取方法的参数集合。
        /// </summary>
        IDictionary<string, object> Params { get; }
    }
}
