using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Service.Client
{
    /// <summary>
    /// 表示一个Ajax全局选项配置信息对象。
    /// </summary>
    public struct AjaxOptions
    {
        
        /// <summary>
        /// 表示当前Ajax请求的请求方法[GET/POST]。
        /// </summary>
        private string _method;

        /// <summary>
        /// Content-typeHTTP 标头。
        /// </summary>
        private string _contentType;

        /// <summary>
        /// 是否异步。
        /// </summary>
        private bool? _async;

        /// <summary>
        /// Encoding编码类型。
        /// </summary>
        private string _encoding;

        /// <summary>
        /// 发送请求前调用的方法。
        /// </summary>
        private Func<AjaxRequest, bool> _onBeforeSend;

        /// <summary>
        /// 发送请求后调用的方法。
        /// </summary>
        private Action<AjaxRequest> _onAfterSend;

        /// <summary>
        /// 请求成功时调用的方法。
        /// </summary>
        private Action<AjaxRequest, AjaxResponse> _onSuccess;

        /// <summary>
        /// 请求发生异常错误时调用的方法。
        /// </summary>
        private Action<AjaxRequest, AjaxResponse, Exception> _onError;

        /// <summary>
        /// 初始化Ajax配置信息。
        /// </summary>
        /// <param name="method">当前Ajax请求的请求方法</param>
        /// <param name="contentType">传输内容的类型。</param>
        /// <param name="async">是否异步。</param>
        /// <param name="encoding">编码类型。</param>
        /// <param name="onBeforeSend">发送请求前调用的方法。</param>
        /// <param name="onAfterSend">发送请求后调用的方法。</param>
        /// <param name="onSuccess">请求成功时调用的方法。</param>
        /// <param name="onError">请求发生异常错误时调用的方法。</param>
        public AjaxOptions(string method = null, 
            string contentType = null, 
            bool? async = null, 
            string encoding = null, 
            Func<AjaxRequest, bool> onBeforeSend = null, 
            Action<AjaxRequest> onAfterSend = null, 
            Action<AjaxRequest, AjaxResponse> onSuccess = null, 
            Action<AjaxRequest, AjaxResponse, Exception> onError = null)
        {
            this._method = method;
            this._contentType = contentType;
            this._async = async;
            this._encoding = encoding;
            this._onBeforeSend = onBeforeSend;
            this._onAfterSend = onAfterSend;
            this._onSuccess = onSuccess;
            this._onError = onError;
        }

        /// <summary>
        /// 获取一个值，该值标识当前Ajax请求的方法是否为GET。
        /// </summary>
        public bool IsGet
        {
            get { return Ajax.METHOD_GET.Equals(_method, StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// 获取一个值，该值标识当前Ajax请求的方法是否为POST。
        /// </summary>
        public bool IsPost
        {
            get { return Ajax.METHOD_POST.Equals(_method, StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// 获取或设置Ajax请求的方法。
        /// </summary>
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        /// <summary>
        /// 获取或设置 Content-typeHTTP 标头的值。
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        /// <summary>
        /// 获取或设置一个值，该值标识是否为异步请求。
        /// </summary>
        public bool Async
        {
            get { return _async.HasValue ? _async.Value : true; }
            set { _async = value; }
        }

        /// <summary>
        /// 获取或设置HTTP编码方式。
        /// </summary>
        public string Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary>
        /// 获取或设置发送请求前调用的方法。
        /// </summary>
        public Func<AjaxRequest, bool> OnBeforeSend
        {
            get { return _onBeforeSend; }
            set { _onBeforeSend = value; }
        }

        /// <summary>
        /// 获取或设置发送请求后调用的方法。
        /// </summary>
        public Action<AjaxRequest> OnAfterSend
        {
            get { return _onAfterSend; }
            set { _onAfterSend = value; }
        }

        /// <summary>
        /// 获取或设置请求成功时调用的方法。
        /// </summary>
        public Action<AjaxRequest, AjaxResponse> OnSuccess
        {
            get { return _onSuccess; }
            set { _onSuccess = value; }
        }

        /// <summary>
        /// 获取或设置请求发生异常错误时调用的方法。
        /// </summary>
        public Action<AjaxRequest, AjaxResponse, Exception> OnError
        {
            get { return _onError; }
            set { _onError = value; }
        }

        /// <summary>
        /// 合并全局选项设置。
        /// </summary>
        /// <param name="from">Ajax设置对象。</param>
        internal void merge(AjaxOptions from)
        {
            if (!_async.HasValue)
            {
                _async = from._async;
            }

            if (string.IsNullOrEmpty(_method))
            {
                _method = from._method;
            }

            if (string.IsNullOrEmpty(ContentType))
            {
                _contentType = from._contentType;
            }

            if (string.IsNullOrEmpty(_encoding))
            {
                _encoding = from._encoding;
            }

            if (null == _onBeforeSend)
            {
                _onBeforeSend = from._onBeforeSend;
            }

            if (null == _onAfterSend)
            {
                _onAfterSend = from._onAfterSend;
            }

            if (null == _onSuccess)
            {
                _onSuccess = from._onSuccess;
            }

            if (null == _onError)
            {
                _onError = from._onError;
            }
        }
    }
}
