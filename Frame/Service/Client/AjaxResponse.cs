using System;
using System.IO;
using System.Net;
using System.Text;

namespace Frame.Service.Client
{
    /// <summary>
    /// 表示一个异步HTTP响应信息对象，提供对HTTP请求响应操作的封装。
    /// </summary>
    public class AjaxResponse
    {

        /// <summary>
        /// 一个HTTP响应对象。
        /// </summary>
        private readonly HttpWebResponse _response;

        /// <summary>
        /// HTTP定义的状态代码对象。
        /// </summary>
        private HttpStatusCode _status;

        /// <summary>
        /// 初始化请求响应的信息。
        /// </summary>
        /// <param name="response">一个HTTP请求响应对象。</param>
        internal AjaxResponse(HttpWebResponse response)
        {
            this._response = response;
            this.init();
        }

        /// <summary>
        /// 初始化响应信息。
        /// </summary>
        private void init()
        {
            this._status = _response.StatusCode;
            GetResponseText();
        }

        /// <summary>
        /// 获取一个值，该值标识是否请求成功。
        /// </summary>
        public bool OK
        {
            get { return HttpStatusCode.OK == _status; }
        }

        /// <summary>
        /// 获取当前请求响应对象。
        /// </summary>
        public HttpWebResponse HttpResponse
        {
            get { return _response; }
        }

        /// <summary>
        /// 获取当前请求的状态代码对象。
        /// </summary>
        public HttpStatusCode Status
        {
            get { return _status; }
        }

        /// <summary>
        /// 获取来自服务端响应的内容信息文本。
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 获取来自服务端响应的信息。
        /// </summary>
        private void GetResponseText()
        {
            using (Stream stream = _response.GetResponseStream())
            {
                //TODO : Get Encoding from Content-Encoding
                //string encoding = _response.ContentEncoding;
                //if (string.IsNullOrEmpty(encoding))
                //{
                //    encoding = Ajax.Options.Encoding;
                //}
                if (null == stream)
                {
                    Text = "";
                }
                else
                {
                    string encoding = Ajax.Options.Encoding;
                    using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                    {
                        Text = reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
