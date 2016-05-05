using System.Web;
using System.Web.Routing;

namespace Frame.Service.Server
{
    public interface IPageHandler : IHttpHandler, IRouteHandler
    {
        /// <summary>
        /// 用于Page Engine在初始化时设置当前的IPageContainer
        /// </summary>
        IPageContainer PageContainer { get; set; }
    }
}
