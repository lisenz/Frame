using System.Web;

namespace Frame.Service.Server
{
    /// <summary>
    /// 负责监听HttpRequest，可在Web.Config中进行配置来启动ServiceEngine。
    /// 若要配置此服务，需在Config文件中的[system.web]的[httpModules]配置节中加入
    /// [add name="UrlRoutingModule" type="System.Web.Routing.UrlRoutingModule, System.Web.Routing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35"/]
    /// [add type="Frame.Service.Server.HttpModule,Frame.Service.Server" name="ServiceEngine"/]。
    /// </summary>
    public class HttpModule : IHttpModule
    {
        /// <summary>
        /// 初始化模块，启动ServiceEngine，并使其为处理请求做好准备。
        /// </summary>
        /// <param name="context">一个System.Web.HttpApplication，它提供对应用程序内所有应用程序对象的公用的方法、属性和事件的访问。在该模块，此对象不做处理。</param>
        void IHttpModule.Init(HttpApplication context)
        {
            ServiceEngine.Run();
        }

        /// <summary>
        /// 销毁处置实现模块使用的资源。
        /// </summary>
        void IHttpModule.Dispose()
        {

        }
    }
}
