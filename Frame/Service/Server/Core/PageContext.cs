using System.IO;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Frame.Core.Extensions;

namespace Frame.Service.Server.Core
{
    /// <summary>
    /// 解出URL Route信息和参数信息,提供对用于为HTTP请求提供服务的内部服务器对象的引用。
    /// </summary>
    public class PageContext : IPageContext
    {
        /// <summary>
        /// 客户端在Web请求期间发送的HTTP值。
        /// </summary>
        private readonly HttpRequest _request;

        /// <summary>
        /// HTTP特定信息的对象包含的路由信息。
        /// </summary>
        private readonly RouteData _routeData;

        /// <summary>
        /// 页面的路由URL模式规则。
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// 页面名称。
        /// </summary>
        private readonly string _page;

        /// <summary>
        /// 页面方法。
        /// </summary>
        private readonly string _action;

        /// <summary>
        /// 页面方法的参数列表。
        /// </summary>
        private IDictionary<string, object> _params;

        /// <summary>
        /// 初始化为HTTP请求提供服务的相关数据及信息。
        /// </summary>
        /// <param name="context">包含有关某个HTTP请求的HTTP特定信息的对象。</param>
        /// <param name="request">客户端在Web请求期间发送的HTTP值。</param>
        public PageContext(HttpContextBase context, HttpRequest request)
        {
            _request = request;
            _path = request.AppRelativeCurrentExecutionFilePath.Substring(1);
            _routeData = RouteTable.Routes.GetRouteData(context);
            _page = null == _routeData ? null : _routeData.Values[Constants.PageRouteKey] as string;
            _action = null == _routeData ? null : _routeData.Values[Constants.ActionRouteKey] as string;
        }

        /// <summary>
        /// 获取服务的路由URL模式规则。
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// 获取服务名称。
        /// </summary>
        public string Page
        {
            get { return _page; }
        }

        /// <summary>
        /// 获取服务的服务方法名称。
        /// </summary>
        public string Action
        {
            get { return _action; }
        }

        /// <summary>
        /// 获取服务方法的参数列表。
        /// </summary>
        public IDictionary<string, object> Params
        {
            get
            {
                if (_params != null)
                {
                    return _params;
                }

                _params = new Dictionary<string, object>();
                foreach (var key in _request.Files.Keys)
                {
                    string name = key.ToString();
                    _params.Add(name, _request.Files[name]);
                }

                //添加Form中的参数
                foreach (var key in _request.Form.Keys)
                {
                    string name = key.ToString();
                    _params.Add(name, _request.Form[name]);
                }

                //添加QueryString中的参数
                foreach (var key in _request.QueryString.Keys)
                {
                    if (null != key)
                    {
                        string name = key.ToString();
                        _params.Add(name, _request.QueryString[name]);
                    }
                }

                if (_request.HttpMethod.EqualsIgnoreCase(Constants.HttpPost)
                    && (!string.IsNullOrEmpty(_request.ContentType)
                    && _request.ContentType.ToLower().StartsWith("application/json")))
                {
                    using (var sr = new StreamReader(_request.InputStream, _request.ContentEncoding))
                    {
                        var converter = new KeyValuePairConverter();
                        var jsonParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd(), converter);

                        foreach (var item in jsonParams)
                        {
                            _params[item.Key] = item.Value;
                        }
                    }
                }

                return _params;
            }
        }
    }
}
