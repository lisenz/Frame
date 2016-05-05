using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;

namespace Frame.Service.Client
{
    /// <summary>
    /// 提供对Ajax请求的统一方法。
    /// </summary>
    public class Ajax
    {
        /// <summary>
        /// 私有构造函数，禁止无参实例化该类。
        /// </summary>
        private Ajax()
        {

        }

        /// <summary>
        /// Ajax请求的URL对象。
        /// </summary>
        private static Uri _baseUri;

        /// <summary>
        /// 表示GET请求方法。
        /// </summary>
        public const string METHOD_GET = "GET";

        /// <summary>
        /// 表示POST请求方法。
        /// </summary>
        public const string METHOD_POST = "POST";

        /// <summary>
        /// 表示提交数据的格式，指定将数据回发到服务器时浏览器使用的编码类型，这里是标准格式，窗体数据被编码为名称/值对。
        /// </summary>
        public const string CONTENT_TYPE_FORM_URLENCODED = "application/x-www-form-urlencoded";

        /// <summary>
        /// 表示提交数据的格式，指定将数据回发到服务器时浏览器使用的编码类型，这里是标准格式，窗体数据被编码为JSON类型。
        /// </summary>
        public const string CONTENT_TYPE_APPLICATION_JSON = "application/json";

        /// <summary>
        /// 表示提交数据的格式，指定将数据回发到服务器时浏览器使用的编码类型，这里是标准格式，窗体数据被编码为浏览器用表单上传文件的方式。
        /// </summary>
        public const string CONTENT_TYPE_MULTIPART_FORM_DATA = "multipart/form-data, boundary=";

        /// <summary>
        /// Ajax请求的全局选项设置，此选项的更改将影响所有请求，除非在每个请求中单独设置选项值以覆盖全局选项的值。
        /// </summary>
        public static readonly AjaxOptions Options = new AjaxOptions(method: METHOD_POST, async: true, encoding: "UTF-8", contentType: CONTENT_TYPE_FORM_URLENCODED);

        /// <summary>
        /// 如果Ajax请求地址是相对路径，那么会自动给相对路径附加上BaseUri
        /// </summary>
        public static string BaseUri
        {
            get { return null == _baseUri ? null : _baseUri.OriginalString; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _baseUri = new Uri(value);
                }
                else
                {
                    _baseUri = null;
                }
            }
        }

        #region 异步调用

        public static void AsynGet(string uri, Action<AjaxRequest, AjaxResponse> onSuccess, string contentType = null, object data = null, Action<AjaxRequest, AjaxResponse, Exception> onError = null, string encoding = null, AjaxOptions? options = null)
        {
            AsynRequest(uri, method: METHOD_GET, contentType: contentType, data: data, onSuccess: onSuccess, onError: onError, encoding: encoding, options: options);
        }

        public static void AsynPost(string uri, Action<AjaxRequest, AjaxResponse> onSuccess, string contentType = null, object data = null, Action<AjaxRequest, AjaxResponse, Exception> onError = null, string encoding = null, AjaxOptions? options = null)
        {
            AsynRequest(uri, method: METHOD_POST, contentType: contentType, data: data, onSuccess: onSuccess, onError: onError, encoding: encoding, options: options);
        }

        public static void AsynRequest(string uri, Action<AjaxRequest, AjaxResponse> onSuccess, string method = null, string contentType = null, object data = null, Action<AjaxRequest, AjaxResponse, Exception> onError = null, string encoding = null, AjaxOptions? options = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException("uri");
            }
            AsynRequest(new Uri(uri), method: method, contentType: contentType, data: data, onSuccess: onSuccess, onError: onError, encoding: encoding, options: options);
        }

        public static void AsynRequest(Uri uri, Action<AjaxRequest, AjaxResponse> onSuccess, string method = null, string contentType = null, object data = null, Action<AjaxRequest, AjaxResponse, Exception> onError = null, string encoding = null, AjaxOptions? options = null)
        {
            if (null == uri)
            {
                throw new ArgumentNullException(uri.OriginalString);
            }
            if (uri.IsAbsoluteUri && !(uri.Scheme.Equals("http") || uri.Scheme.Equals("https")))
            {
                throw new ArgumentException("目前暂支持 http:// 或者 https:// 的URL请求", uri.OriginalString);
            }

            AjaxOptions opt = CreateOptions(method, contentType, onSuccess, onError, true, encoding, options);

            if (!opt.IsGet && !opt.IsPost)
            {
                throw new ArgumentException("当前Ajax请求只支持 'GET' 或者 'POST' 两种请求方法。", method);
            }

            if (null != data && opt.IsGet && !(data is IDictionary<string, object> || data is string))
            {
                throw new ArgumentException("当前请求传输的数据格式必须是IDictionary<string,object>或者当请求是GET请求方法时以'a=b&c=d'格式设置。", data.ToString());
            }

            //附加QueryString到uri上
            if (opt.IsGet && null != data)
            {
                uri = CreateNewUriForGetMethod(uri, data);
            }

            //处理相对路径
            if (!uri.IsAbsoluteUri)
            {
                if (null == _baseUri)
                {
                    throw new ArgumentException("'uri' 必须是一个绝对路径或者设置'Ajax.BaseUrl'属性", uri.OriginalString);
                }
                uri = new Uri(_baseUri, uri);
            }

            AjaxRequest request = new AjaxRequest((HttpWebRequest)WebRequest.Create(uri), opt);
            if (opt.IsPost)
            {
                request.Data = data;
            }

            if (opt.IsGet)
            {
                SendGetRequest(request);
            }
            else
            {
                request.HttpRequest.BeginGetRequestStream(BeginGetRequestStreamCallback, request);
            }
        }

        public static void AsynFile(Uri uri, Action<AjaxRequest, AjaxResponse> onSuccess, string method = null, string contentType = null, object data = null, Action<AjaxRequest, AjaxResponse, Exception> onError = null, string encoding = null, AjaxOptions? options = null)
        {
            if (null == uri)
            {
                throw new ArgumentNullException(uri.OriginalString);
            }
            if (uri.IsAbsoluteUri && !(uri.Scheme.Equals("http") || uri.Scheme.Equals("https")))
            {
                throw new ArgumentException("目前暂支持 http:// 或者 https:// 的URL请求", uri.OriginalString);
            }

            AjaxOptions opt = CreateOptions(method, contentType, onSuccess, onError, true, encoding, options);

            if (!opt.IsPost)
            {
                throw new ArgumentException("当前Ajax请求只支持 'POST' 请求方法。", method);
            }

            //处理相对路径
            if (!uri.IsAbsoluteUri)
            {
                if (null == _baseUri)
                {
                    throw new ArgumentException("'uri' 必须是一个绝对路径或者设置'Ajax.BaseUrl'属性", uri.OriginalString);
                }
                uri = new Uri(_baseUri, uri);
            }

            AjaxRequest request = new AjaxRequest((HttpWebRequest)WebRequest.Create(uri), opt);

            request.Data = data;

            request.HttpRequest.BeginGetRequestStream(BeginGetRequestFileStreamCallback, request);
        }

        #endregion

        #region 同步调用

        public static AjaxResponse SynGet(string uri, string contentType = null, object data = null, string encoding = null, AjaxOptions? options = null)
        {
            return SynRequest(uri, method: METHOD_GET, contentType: contentType, data: data, encoding: encoding, options: options);
        }

        public static AjaxResponse SynPost(string uri, string contentType = null, object data = null, string encoding = null, AjaxOptions? options = null)
        {
            return SynRequest(uri, method: METHOD_POST, contentType: contentType, data: data, encoding: encoding, options: options);
        }

        public static AjaxResponse SynRequest(string uri, string method = null, string contentType = null, object data = null, string encoding = null, AjaxOptions? options = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException("uri");
            }
            return SynRequest(new Uri(uri), method: method, contentType: contentType, data: data, encoding: encoding, options: options);
        }

        public static AjaxResponse SynRequest(Uri uri, string method = null, string contentType = null, object data = null, string encoding = null, AjaxOptions? options = null)
        {
            if (null == uri)
            {
                throw new ArgumentNullException(uri.OriginalString);
            }
            if (uri.IsAbsoluteUri && !(uri.Scheme.Equals("http") || uri.Scheme.Equals("https")))
            {
                throw new ArgumentException("目前暂支持 http:// 或者 https:// 的URL请求", uri.OriginalString);
            }

            AjaxOptions opt = CreateOptions(method, contentType, false, encoding, options);

            if (!opt.IsGet && !opt.IsPost)
            {
                throw new ArgumentException("当前Ajax请求只支持 'GET' 或者 'POST' 两种请求方法。", method);
            }

            if (null != data && opt.IsGet && !(data is IDictionary<string, object> || data is string))
            {
                throw new ArgumentException("当前请求传输的数据格式必须是IDictionary<string,object>或者当请求是GET请求方法时以'a=b&c=d'格式设置。", data.ToString());
            }

            //附加QueryString到uri上
            if (opt.IsGet && null != data)
            {
                uri = CreateNewUriForGetMethod(uri, data);
            }

            //处理相对路径
            if (!uri.IsAbsoluteUri)
            {
                if (null == _baseUri)
                {
                    throw new ArgumentException("'uri' 必须是一个绝对路径或者设置'Ajax.BaseUrl'属性", uri.OriginalString);
                }
                uri = new Uri(_baseUri, uri);
            }

            AjaxRequest request = new AjaxRequest((HttpWebRequest)WebRequest.Create(uri), opt);

            try
            {
                if (opt.IsPost)
                {
                    request.Data = data;
                    SetRequestStream(request);
                }
                return GetResponse(request);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public static AjaxResponse SynFile(Uri uri, string method = null, string contentType = null, object data = null, string encoding = null, AjaxOptions? options = null)
        {
            if (null == uri)
            {
                throw new ArgumentNullException(uri.OriginalString);
            }
            if (uri.IsAbsoluteUri && !(uri.Scheme.Equals("http") || uri.Scheme.Equals("https")))
            {
                throw new ArgumentException("目前暂支持 http:// 或者 https:// 的URL请求", uri.OriginalString);
            }

            AjaxOptions opt = CreateOptions(method, contentType, false, encoding, options);

            if (!opt.IsPost)
            {
                throw new ArgumentException("当前Ajax请求只支持 'POST' 请求方法。", method);
            }

            //处理相对路径
            if (!uri.IsAbsoluteUri)
            {
                if (null == _baseUri)
                {
                    throw new ArgumentException("'uri' 必须是一个绝对路径或者设置'Ajax.BaseUrl'属性", uri.OriginalString);
                }
                uri = new Uri(_baseUri, uri);
            }

            AjaxRequest request = new AjaxRequest((HttpWebRequest)WebRequest.Create(uri), opt);

            try
            {
                if (opt.IsPost)
                {
                    request.Data = data;
                    SetRequestFileStream(request);
                }
                return GetResponse(request);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region 异步调用内部回滚事件方法

        /// <summary>
        /// 当Ajax请求为GET请求方法时进行调用的请求方法。
        /// </summary>
        /// <param name="request">一个HTTP请求对象。</param>
        private static void SendGetRequest(AjaxRequest request)
        {
            try
            {
                request.Stage = AjaxStage.Prepared;

                request.HttpRequest.BeginGetResponse(BeginGetResponseCallback, request);

                request.Stage = AjaxStage.Requested;
            }
            catch (Exception e)
            {
                OnAjaxError(request, null, e);
            }
        }

        /// <summary>
        /// 当请求文件上传时对用来写入数据的 System.IO.Stream 对象的异步请求执行后的回滚方法。
        /// </summary>
        /// <param name="result">引用该异步请求的 System.IAsyncResult。</param>
        private static void BeginGetRequestFileStreamCallback(IAsyncResult result)
        {
            AjaxRequest request = (AjaxRequest)result.AsyncState;
            try
            {
                SetRequestFileStream(request, result);
                SendGetRequest(request);
            }
            catch (Exception e)
            {
                OnAjaxError(request, null, e);
            }
        }

        /// <summary>
        /// 当Ajax请求为POST请求方法时对用来写入数据的 System.IO.Stream 对象的异步请求执行后的回滚方法。
        /// </summary>
        /// <param name="result">引用该异步请求的 System.IAsyncResult。</param>
        private static void BeginGetRequestStreamCallback(IAsyncResult result)
        {
            AjaxRequest request = (AjaxRequest)result.AsyncState;
            try
            {
                SetRequestStream(request, result);
                SendGetRequest(request);
            }
            catch (Exception e)
            {
                OnAjaxError(request, null, e);
            }
        }

        /// <summary>
        /// Ajax异步请求对服务端响应后回滚执行调用的方法。
        /// </summary>
        /// <param name="result">引用该异步请求的 System.IAsyncResult。</param>
        private static void BeginGetResponseCallback(IAsyncResult result)
        {
            AjaxRequest request = (AjaxRequest)result.AsyncState;
            AjaxResponse response = null;
            try
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)request.HttpRequest.EndGetResponse(result))
                {
                    request.Stage = AjaxStage.Responsed;
                    response = new AjaxResponse(httpResponse);

                    if (response.OK)
                    {

                        try
                        {
                            request.Options.OnSuccess(request, response);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message, e);
                        }
                    }
                    else
                    {
                        try
                        {
                            OnAjaxError(request, response, null);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message, e);
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)we.Response)
                {
                    request.Stage = AjaxStage.Responsed;
                    response = new AjaxResponse(httpResponse);

                    OnAjaxError(request, response, we);
                }
            }
            catch (Exception e)
            {
                OnAjaxError(request, response, e);
            }
        }

        private static AjaxResponse GetResponse(AjaxRequest request)
        {
            request.Stage = AjaxStage.Prepared;
            using (HttpWebResponse httpResponse = (HttpWebResponse)request.HttpRequest.GetResponse())
            {
                request.Stage = AjaxStage.Responsed;

                AjaxResponse response = new AjaxResponse(httpResponse);

                request.Stage = AjaxStage.Requested;
                return response;
            }
        }

        /// <summary>
        /// Ajax请求响应失败后执行的方法。
        /// </summary>
        /// <param name="request">HTTP请求对象。</param>
        /// <param name="response">HTTP响应对象。</param>
        /// <param name="e">请求处理期间发生的异常对象。</param>
        private static void OnAjaxError(AjaxRequest request, AjaxResponse response, Exception e)
        {
            if (null != request.Options.OnError)
            {
                try
                {
                    request.Options.OnError(request, response, e);

                }
                catch (Exception e1)
                {
                    throw new Exception(e1.Message, e1);
                }
            }
            else if (null != e)
            {
                throw e;
            }
        }

        #endregion

        #region 内部方法

        private static void SetRequestStream(AjaxRequest request, IAsyncResult result = null)
        {
            using (var requestStream = null == result ? request.HttpRequest.GetRequestStream() : request.HttpRequest.EndGetRequestStream(result))
            {
                byte[] bytes = null;

                if (null != request.Data && request.Options.IsPost)
                {
                    string content = JsonConvert.SerializeObject(request.Data);
                    bytes = Encoding.GetEncoding(request.Options.Encoding).GetBytes(content);
                }
                else
                {
                    bytes = new byte[0];
                }
                requestStream.Write(bytes, 0, bytes.Length);

            }
        }

        private static void SetRequestFileStream(AjaxRequest request, IAsyncResult result = null)
        {
            using (var requestStream = null == result ? request.HttpRequest.GetRequestStream() : request.HttpRequest.EndGetRequestStream(result))
            {
                string querystring = "\n";
                IDictionary<object, object> data = (Dictionary<object, object>)request.Data;
                string boundary = request.Options.ContentType.Substring(request.Options.ContentType.IndexOf("=") + 1);

                foreach (var key in data.Keys)
                {
                    foreach (var property in key.GetType().GetProperties())
                    {
                        querystring += "--" + boundary + "\n";
                        querystring += "content-disposition: form-data; name=\"" + System.Uri.EscapeDataString(property.Name) + "\"\n\n";
                        querystring += System.Uri.EscapeDataString(property.GetValue(key, null).ToString());
                        querystring += "\n";
                    }
                }

                byte[] byteArray = Encoding.GetEncoding(request.Options.Encoding).GetBytes(querystring);
                requestStream.Write(byteArray, 0, byteArray.Length);
                byte[] closing = Encoding.GetEncoding(request.Options.Encoding).GetBytes("\n--" + boundary + "--\n");

                foreach (NamedFileStream[] files in data.Values)
                {
                    foreach (NamedFileStream file in files)
                    {
                        Stream outBuffer = new MemoryStream();
                        string qsAppend;
                        qsAppend = "--" + boundary + "\ncontent-disposition: form-data; name=\"" + file.Name + "\"; filename=\"" + file.Filename + "\"\r\nContent-Type: " + file.ContentType + "\r\n\r\n";

                        StreamReader sr = new StreamReader(file.Stream);
                        outBuffer.Write(Encoding.GetEncoding(request.Options.Encoding).GetBytes(qsAppend), 0, qsAppend.Length);

                        int bytesRead = 0;
                        byte[] buffer = new byte[file.Stream.Length];

                        while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outBuffer.Write(buffer, 0, bytesRead);

                        }
                        outBuffer.Write(closing, 0, closing.Length);

                        outBuffer.Position = 0;
                        byte[] tempBuffer = new byte[outBuffer.Length];
                        outBuffer.Read(tempBuffer, 0, tempBuffer.Length);
                        requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                    }
                }
            }
        }

        private static Uri CreateNewUriForGetMethod(Uri oldUri, object data)
        {
            string query = null;
            if (data is string)
            {
                query = (string)data;
            }
            else
            {
                query = BuildQueryString((IDictionary<string, object>)data);
            }

            string oldUriString = oldUri.OriginalString;
            string newUriString = oldUriString + (oldUriString.IndexOf("?") > 0 ? "&" : "?") + query;

            return new Uri(newUriString);
        }

        /// <summary>
        /// 处理当前请求传输的数据。
        /// </summary>
        /// <param name="data">当前请求传输的数据。</param>
        /// <returns>处理后的数据。</returns>
        private static string BuildQueryString(IDictionary<string, object> data)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string key in data.Keys)
            {
                object value = data[key];
                string param = null == value ? "" : (value is string ? (string)value : JsonConvert.SerializeObject(value));

                builder.Append("&").Append(key).Append("=").Append(param);
            }
            return builder.Remove(0, 1).ToString();
        }

        /// <summary>
        /// 创建Ajax全局选项设置。
        /// </summary>
        /// <param name="method">Ajax请求的请求方法。</param>
        /// <param name="contentType">Content-typeHTTP 标头。</param>
        /// <param name="async">标识是否为异步请求。</param>
        /// <param name="encoding">请求的编码。</param>
        /// <param name="options">要进行合并的全局选项设置。</param>
        /// <returns>返回创建或者合并选项的Ajax全局选项设置对象。</returns>
        private static AjaxOptions CreateOptions(string method = null, string contentType = null, bool? async = null, string encoding = null, AjaxOptions? options = null)
        {
            return CreateOptions(method, contentType, null, null, async, encoding, options);
        }

        /// <summary>
        /// 创建Ajax全局选项设置。
        /// </summary>
        /// <param name="method">Ajax请求的请求方法。</param>
        /// <param name="contentType">Content-typeHTTP 标头。</param>
        /// <param name="onSuccess">请求成功时调用的方法。</param>
        /// <param name="onError">请求失败时调用的方法。</param>
        /// <param name="async">标识是否为异步请求。</param>
        /// <param name="encoding">请求的编码。</param>
        /// <param name="options">要进行合并的全局选项设置。</param>
        /// <returns>返回创建或者合并选项的Ajax全局选项设置对象。</returns>
        private static AjaxOptions CreateOptions(string method = null, string contentType = null, Action<AjaxRequest, AjaxResponse> onSuccess = null, Action<AjaxRequest, AjaxResponse, Exception> onError = null, bool? async = null, string encoding = null, AjaxOptions? options = null)
        {
            AjaxOptions opt = options.HasValue ? options.Value : new AjaxOptions();

            if (async.HasValue)
            {
                opt.Async = async.Value;
            }

            if (!string.IsNullOrEmpty(method))
            {
                opt.Method = method;
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                opt.ContentType = contentType;
            }

            if (!string.IsNullOrEmpty(encoding))
            {
                opt.Encoding = encoding;
            }

            if (null != onSuccess)
            {
                opt.OnSuccess = onSuccess;
            }

            if (null != onError)
            {
                opt.OnError = onError;
            }

            //合并全局的选项设置
            opt.merge(Options);

            return opt;
        }

        #endregion
    }
}
