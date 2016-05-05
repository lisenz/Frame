using System;
//-----
using System.Net;
using System.Threading;

namespace Frame.Service.Client
{
    /// <summary>
    /// 表示一个异步HTTP请求的封装对象。
    /// </summary>
    public class AjaxRequest
    {

        /// <summary>
        /// 一个HTTP请求。
        /// </summary>
        private readonly HttpWebRequest _request;

        /// <summary>
        /// Ajax设置对象。
        /// </summary>
        private AjaxOptions _options;

        /// <summary>
        /// Ajax请求状态。
        /// </summary>
        private AjaxStage _stage;

        /// <summary>
        /// 线程同步上下文对象。
        /// </summary>
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// 初始化请求信息。
        /// </summary>
        /// <param name="request">一个HTTP请求对象。</param>
        /// <param name="options">请求的相关设置。</param>
        internal AjaxRequest(HttpWebRequest request, AjaxOptions options)
        {
            this._request = request;
            this._options = options;
            this._stage = AjaxStage.Created;
            this._syncContext = SynchronizationContext.Current;

            this.init();
        }

        /// <summary>
        /// 初始化。
        /// </summary>
        private void init()
        {
            _request.Method = _options.Method;

            if (_options.IsPost)
            {
                _request.ContentType = _options.ContentType;
            }
        }

        /// <summary>
        /// 获取当前线程的同步上下文对象。
        /// </summary>
        internal SynchronizationContext SyncContext
        {
            get { return _syncContext; }
        }

        /// <summary>
        /// 获取当前HTTP请求对象。
        /// </summary>
        public HttpWebRequest HttpRequest
        {
            get { return _request; }
        }

        /// <summary>
        /// 获取当前请求的请求方法。
        /// </summary>
        public string Method
        {
            get { return _request.Method; }
        }

        /// <summary>
        /// 获取当前请求的URL对象。
        /// </summary>
        public Uri Uri
        {
            get { return _request.RequestUri; }
        }

        /// <summary>
        /// 获取当前请求的URL字符串。
        /// </summary>
        public string UriString
        {
            get { return Uri.OriginalString; }
        }

        /// <summary>
        /// 获取当前请求的设置信息对象。
        /// </summary>
        public AjaxOptions Options
        {
            get { return _options; }
        }

        /// <summary>
        /// 获取当前请求的状态标识。
        /// </summary>
        public AjaxStage Stage
        {
            get { return _stage; }
            internal set { _stage = value; }
        }

        /// <summary>
        /// 获取一个值，该值标识请求是否已收到服务端的响应。
        /// </summary>
        public bool Responsed
        {
            get { return AjaxStage.Responsed == _stage; }
        }

        /// <summary>
        /// 获取或设置当前请求传输的数据。
        /// </summary>
        public object Data { get; set; }
    }
}
