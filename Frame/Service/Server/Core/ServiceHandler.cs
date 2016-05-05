using System;
using System.Web;
using System.Web.Routing;
using Frame.Core.Extensions;
using System.Web.SessionState;

namespace Frame.Service.Server.Core
{
    public class ServiceHandler : IServiceHandler, IRequiresSessionState
    {
        /// <summary>
        /// 一个服务容器对象。
        /// </summary>
        private IServiceContainer _serviceContainer;

        /// <summary>
        /// 获取一个值，该值指示其他请求可以再次使用该实例。
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// 获取或设置服务容器对象。
        /// </summary>
        public IServiceContainer ServiceContainer
        {
            get { return _serviceContainer; }
            set { _serviceContainer = value; }
        }

        /// <summary>
        /// 获取一个Http处理程序请求对象。
        /// </summary>
        /// <param name="requestContext">Http请求对象。</param>
        /// <returns>返回当前服务请求对象。</returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        /// <summary>
        /// 通过实现System.Web.IHttpHandler接口的自定义HttpHandler启用 HTTP Web 请求的处理。
        /// </summary>
        /// <param name="context">System.Web.HttpContext 对象，它提供对用于为 HTTP 请求提供服务的内部服务器对象（如 Request、Response、Session和 Server）的引用。</param>
        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context), context.Request, context.Response);
        }

        /// <summary>
        /// 通过实现System.Web.IHttpHandler接口的自定义HttpHandler启用 HTTP Web 请求的处理。
        /// </summary>
        /// <param name="context">包含有关某个HTTP请求的HTTP特定信息的对象。</param>
        /// <param name="request">客户端在Web请求期间发送的HTTP值。</param>
        /// <param name="response">HTTP响应信息对象。</param>
        protected virtual void ProcessRequest(HttpContextBase context, HttpRequest request, HttpResponse response)
        {
            try
            {
                bool isAjax = IsAjax(request);

                SetContentType(request, response, isAjax);

                ServiceContext serviceContext = new ServiceContext(context, request);

                object value = CallService(serviceContext);

                response.Write(new Result(value));
            }
            catch (ServiceException e)
            {
                response.Write(new Result(e));
            }
            catch (Exception e)
            {
                response.Write(new Result(e));
            }
        }

        /// <summary>
        /// 调用HTTP请求提供服务的服务对象，并对其进行执行且返回执行结果。
        /// </summary>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>返回请求的服务对象执行后的结果。</returns>
        protected object CallService(ServiceContext context)
        {
            if (string.IsNullOrEmpty(context.Service))
            {
                throw new ServiceException(code: Result.NotFound,
                                           desc: "当前请求没有相关服务可供调用。");
            }

            IService service = ServiceContainer.Resolve(context.Service);

            if (null == service)
            {
                throw new ServiceException(code: Result.NotFound,
                                           desc: string.Format("没有搜索到服务'{0}'。", context.Service));
            }

            return service.Execute(context);
        }

        /// <summary>
        /// 检测客户端使用的HTTP数据传输方法是否有效。
        /// </summary>
        /// <param name="request">客户端在Web请求期间发送的HTTP值。</param>
        protected void CheckRequest(HttpRequest request)
        {
            string method = request.HttpMethod;
            if (!method.EqualsIgnoreCase(Constants.HttpGet) && !method.EqualsIgnoreCase(Constants.HttpPost))
            {
                throw new ServiceException(code: Result.BadRequest,
                                           desc: string.Format("当前不支持该HTTP数据传输方法{0}", method));
            }
        }

        /// <summary>
        /// 获取一个值，该值指示当前请求是否为JQuery异步请求。
        /// </summary>
        /// <param name="request">客户端在Web请求期间发送的HTTP值。</param>
        /// <returns>返回当前请求是否为JQuery异步请求的指示。</returns>
        protected bool IsAjax(HttpRequest request)
        {
            //当前符合两种情况中的一种，都为异步请求
            return Constants.XmlHttpRequest.Equals(request.Headers[Constants.XRequestedWith]) ||
                   null != request.QueryString[Constants.XAjax];
        }

        /// <summary>
        /// 设置输出流的HTTP MIME类型。
        /// </summary>
        /// <param name="request">客户端在Web请求期间发送的HTTP值。</param>
        /// <param name="response">HTTP响应信息对象。</param>
        /// <param name="isAjax">是否为异步请求。</param>
        protected void SetContentType(HttpRequest request, HttpResponse response, bool isAjax)
        {
            if (!isAjax && (request.UserAgent != null && request.UserAgent.StartsWith("Mozilla/")))
            {
                response.ContentType = Constants.TextPlain;
            }
            else
            {
                response.ContentType = Constants.ApplicationJson;
            }
        }
    }
}
