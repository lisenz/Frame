using System.Web;
using System.Web.Routing;

namespace Frame.Service.Server
{
    /// <summary>
    /// 此接口用于处理每个匹配了服务路由规则的Http请求
    /// </summary>
    public interface IServiceHandler : IHttpHandler, IRouteHandler
    {
        /// <summary>
        /// 用于Service Engine在初始化时设置当前的IServiceContainer
        /// </summary>
        IServiceContainer ServiceContainer { get; set; }
    }
}
