using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Service.Server
{
    /// <summary>
    /// 定义了请求服务的一系列常量。
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 页面路由路径。一般指定为页面文件存放的文件夹。
        /// </summary>
        public const string PageRoutePath = "pages";

        /// <summary>
        /// 页面路由规则中页面的名称。
        /// </summary>
        public const string PageRouteKey = "page";

        /// <summary>
        /// 页面路由规则中Action方法的名称。
        /// </summary>
        public const string ActionRouteKey = "action";

        /// <summary>
        /// 服务路由路径。一般指定为服务文件存放的文件夹。
        /// </summary>
        public const string ServiceRoutePath = "services";

        /// <summary>
        /// 服务路由规则中服务的名称。
        /// </summary>
        public const string ServiceRouteKey = "service";

        /// <summary>
        /// 服务路由规则中服务方法的名称。
        /// </summary>
        public const string MethodRouteKey = "method";

        /// <summary>
        /// 日期格式。
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.ffzzz";

        /// <summary>
        /// 数据传输格式：GET。
        /// </summary>
        public const string HttpGet = "GET";

        /// <summary>
        /// 数据传输格式：POST。
        /// </summary>
        public const string HttpPost = "POST";

        /// <summary>
        /// 表示服务器通知客户机传送数据的格式为纯文本格式。
        /// </summary>
        public const string TextPlain = "text/plain";

        /// <summary>
        /// 表示服务器通知客户机传送数据的格式为text/html格式。
        /// </summary>
        public const string TextHtml = "text/html";

        /// <summary>
        /// 表示服务器通知客户机传送数据的格式为JSON格式。
        /// </summary>
        public const string ApplicationJson = "application/json";

        /// <summary>
        /// 用于与HTTP请求的头集合中标识为异步请求进行匹配对比的信息。
        /// </summary>
        public const string XmlHttpRequest = "XMLHttpRequest";

        /// <summary>
        /// HTTP请求的头集合中表示为异步请求的标识。
        /// </summary>
        public const string XRequestedWith = "X-Requested-With";

        /// <summary>
        /// HTTP查询字符串变量集合中表示为异步请求的标识。
        /// </summary>
        public const string XAjax = "x-ajax";


    }
}
