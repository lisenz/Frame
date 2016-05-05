using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Frame.Service.Client
{
    /// <summary>
    /// 提供客户端请求调用服务的属性和方法。
    /// </summary>
    public class HttpClient
    {        
        /// <summary>
        /// 私有构造函数。
        /// </summary>
        private HttpClient()
        {
        }

        /// <summary>
        /// 标识启用全局服务URL前缀。
        /// </summary>
        private static bool _globalSerivceUriPrefixEnabled = true;

        /// <summary>
        /// 获取或设置全局服务URL前缀。
        /// </summary>
        public static string GlobalServiceUriPrefix { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值标识是否启用全局服务URL前缀。
        /// </summary>
        public static bool GlobalServiceUriPrefixEnabled
        {
            get { return _globalSerivceUriPrefixEnabled; }
            set { _globalSerivceUriPrefixEnabled = value; }
        }
        
        /// <summary>
        /// 创建服务请求的完全路径。
        /// </summary>
        /// <param name="service">服务URL。</param>
        /// <param name="parameters">请求传输的参数。</param>
        /// <returns>服务请求的完全路径。</returns>
        private static Uri CreateServiceUri(string service, object parameters)
        {
            string uri = null;

            if ((GlobalServiceUriPrefixEnabled && string.IsNullOrEmpty(GlobalServiceUriPrefix)) ||
                service.IndexOf("://") > 0)
            {
                uri = service;
            }
            else if (!service.StartsWith("/"))
            {
                if (!GlobalServiceUriPrefix.EndsWith("/"))
                {
                    GlobalServiceUriPrefix = GlobalServiceUriPrefix + "/";
                }

                uri = GlobalServiceUriPrefix + service;
            }

            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        #region 同步调用

        /// <summary>
        /// 服务同步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static ReturnResult<string> SynCall(string service, AjaxOptions? options = null)
        {
            return SynCall<string>(service, null, options);
        }

        /// <summary>
        /// 服务同步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static ReturnResult<TValue> SynCall<TValue>(string service, AjaxOptions? options = null)
        {
            return SynCall<TValue>(service, null, options);
        }

        /// <summary>
        /// 服务同步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="parameters">服务请求传输的参数。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static ReturnResult<string> SynCall(string service, object parameters, AjaxOptions? options = null)
        {
            return SynCall<string>(service, parameters, options);
        }

        /// <summary>
        /// 服务同步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="parameters">服务请求传输的参数。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static ReturnResult<string> SynCall(string service, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            return SynCall<string>(service, parameters, options);
        }

        /// <summary>
        /// 服务同步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="parameters">服务请求传输的参数。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static ReturnResult<TValue> SynCall<TValue>(string service, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            return SynCall<TValue>(service, (object)parameters, options);
        }

        /// <summary>
        /// 服务同步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="parameters">服务请求传输的参数。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static ReturnResult<TValue> SynCall<TValue>(string service, object parameters, AjaxOptions? options = null)
        {
            if (null == service)
            {
                throw new ArgumentNullException(service);
            }

            Uri serviceUri = CreateServiceUri(service, parameters);

            AjaxResponse response = null;
            try
            {
                response = Ajax.SynRequest(serviceUri, data: parameters,
                    contentType: Ajax.CONTENT_TYPE_APPLICATION_JSON, options: options);

                try
                {
                    ReturnResult<TValue> result = JsonConvert.DeserializeObject<ReturnResult<TValue>>(response.Text);
                    result.OriginalContent = response.Text;

                    return result;
                }
                catch (Exception ex)
                {
                    ReturnResult<TValue> result = CreateResponseErrorResult<TValue>(response, ex, ReturnResult<TValue>.STATUS_ERROR_RETURN);
                    return result;
                }
            }
            catch (Exception e)
            {
                ReturnResult<TValue> result = CreateResponseErrorResult<TValue>(response, e);
                return result;
            }
        }
        

        #endregion

        #region 异步调用

        /// <summary>
        /// 服务异步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="callback">服务请求后对返回信息处理的回调方法。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static void AsynCall<TValue>(string service, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            AsynCall<TValue>(service, null, callback, options);
        }

        /// <summary>
        /// 服务异步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="parameters">服务请求的传输的参数。</param>
        /// <param name="callback">服务请求后对返回信息处理的回调方法。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static void AsynCall<TValue>(string service, IDictionary<string, object> parameters, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            AsynCall<TValue>(service, (object)parameters, callback, options);
        }

        /// <summary>
        /// 服务异步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="callback">服务请求后对返回信息处理的回调方法。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static void AsynCall(string service, Action<ReturnResult<string>> callback, AjaxOptions? options = null)
        {
            AsynCall(service, null, callback, options);
        }

        /// <summary>
        /// 服务异步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="parameters">服务请求的传输的参数。</param>
        /// <param name="callback">服务请求后对返回信息处理的回调方法。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static void AsynCall(string service, IDictionary<string, object> parameters, Action<ReturnResult<string>> callback, AjaxOptions? options = null)
        {
            AsynCall<string>(service, (object)parameters, callback, options);
        }

        /// <summary>
        /// 服务异步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="parameters">服务请求的传输的参数。</param>
        /// <param name="callback">服务请求后对返回信息处理的回调方法。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static void AsynCall(string service, object parameters, Action<ReturnResult<string>> callback, AjaxOptions? options = null)
        {
            AsynCall<string>(service, parameters, callback, options);
        }

        /// <summary>
        /// 服务异步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="parameters">服务请求传输的参数。</param>
        /// <param name="callback">请求调用后执行的回调方法。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static void AsynCall<TValue>(string service, object parameters, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            if (null == service)
            {
                throw new ArgumentNullException(service);
            }

            Uri serviceUri = CreateServiceUri(service, parameters);

            Ajax.AsynRequest(serviceUri, data: parameters, contentType: Ajax.CONTENT_TYPE_APPLICATION_JSON,
                         onSuccess: (request, response) =>
                         {
                             OnServiceCallSuccess(service, callback, request, response);
                         },
                         onError: (request, response, e) =>
                         {
                             OnServiceCallError(service, callback, request, response, e);
                         }, options: options);
        }

        #endregion

        #region 文件同步调用

        public static ReturnResult<string> SynFileCall(string service, IDictionary<object, object> parameters, AjaxOptions? options = null)
        {
            return SynFileCall<string>(service, (object)parameters, options);
        }

        public static ReturnResult<string> SynFileCall(string service, object parameters, AjaxOptions? options = null)
        {
            return SynFileCall<string>(service, parameters, options);
        }

        public static ReturnResult<TValue> SynFileCall<TValue>(string service, IDictionary<object, object> parameters, AjaxOptions? options = null)
        {
            return SynFileCall<TValue>(service, (object)parameters, options);
        }

        /// <summary>
        /// 文件服务同步调用。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务请求的URL字符串。</param>
        /// <param name="parameters">服务请求传输的参数。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        public static ReturnResult<TValue> SynFileCall<TValue>(string service, object parameters, AjaxOptions? options = null)
        {
            if (null == service)
            {
                throw new ArgumentNullException(service);
            }

            Uri serviceUri = new Uri(service, UriKind.RelativeOrAbsolute);

            AjaxResponse response = null;
            try
            {
                response = Ajax.SynFile(serviceUri, data: parameters,
                    contentType: Ajax.CONTENT_TYPE_MULTIPART_FORM_DATA 
                    + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12), options: options);

                try
                {
                    ReturnResult<TValue> result = JsonConvert.DeserializeObject<ReturnResult<TValue>>(response.Text);
                    result.OriginalContent = response.Text;

                    return result;
                }
                catch (Exception ex)
                {
                    ReturnResult<TValue> result = CreateResponseErrorResult<TValue>(response, ex, ReturnResult<TValue>.STATUS_ERROR_RETURN);
                    return result;
                }
            }
            catch (Exception e)
            {
                ReturnResult<TValue> result = CreateResponseErrorResult<TValue>(response, e);
                return result;
            }
        }

        #endregion

        #region 文件异步调用

        public static void AsynFileCall(string service, IDictionary<object, object> parameters, Action<ReturnResult<string>> callback, AjaxOptions? options = null)
        {
            AsynFileCall<string>(service, (object)parameters, callback, options);
        }

        public static void AsynFileCall(string service, object parameters, Action<ReturnResult<string>> callback, AjaxOptions? options = null)
        {
            AsynFileCall<string>(service, parameters, callback, options);
        }

        public static void AsynFileCall<TValue>(string service, IDictionary<object, object> parameters, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            AsynFileCall<TValue>(service, (object)parameters, callback, options);
        }

        public static void AsynFileCall<TValue>(string service, object parameters, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            if (null == service)
            {
                throw new ArgumentNullException(service);
            }

            Uri serviceUri = new Uri(service, UriKind.RelativeOrAbsolute);

            Ajax.AsynFile(serviceUri, data: parameters,
                         contentType: Ajax.CONTENT_TYPE_MULTIPART_FORM_DATA
                            + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12),
                         method: Ajax.METHOD_POST,
                         onSuccess: (request, response) =>
                         {
                             OnServiceCallSuccess(service, callback, request, response);
                         },
                         onError: (request, response, e) =>
                         {
                             OnServiceCallError(service, callback, request, response, e);
                         }, options: options);
        }

        #endregion

        /// <summary>
        /// Ajax请求服务成功后执行的回调方法。
        /// </summary>
        /// <typeparam name="TValue">对返回结果进行处理的回调方法的参数的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="callback">Ajax请求服务成功后对返回结果进行处理的回调方法。</param>
        /// <param name="request">HTTP请求对象。</param>
        /// <param name="response">HTTP响应对象。</param>
        private static void OnServiceCallSuccess<TValue>(string service,
            Action<ReturnResult<TValue>> callback, AjaxRequest request, AjaxResponse response)
        {
            ReturnResult<TValue> result = null;
            try
            {
                result = JsonConvert.DeserializeObject<ReturnResult<TValue>>(response.Text);
                result.OriginalContent = response.Text;
            }
            catch (Exception e)
            {
                OnServiceResultError(service, callback, response.Text, e);
                return;
            }

            callback(result);
        }

        /// <summary>
        /// 服务请求失败后的回调方法。
        /// </summary>
        /// <typeparam name="TValue">传递给对返回信息进行处理的回调方法的参数的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="callback">Ajax请求服务失败后对返回信息进行处理的回调方法。</param>
        /// <param name="request">HTTP请求对象。</param>
        /// <param name="response">HTTP响应对象。</param>
        /// <param name="e">客户端的异常。</param>
        private static void OnServiceCallError<TValue>(string service,
            Action<ReturnResult<TValue>> callback, AjaxRequest request, AjaxResponse response, Exception e)
        {
            ReturnResult<TValue> result = new ReturnResult<TValue>();
            result = CreateResponseErrorResult<TValue>(response, e);
            callback(result);
        }

        private static ReturnResult<TValue> CreateResponseErrorResult<TValue>(AjaxResponse response, Exception e)
        {
            if (null != response)
                return CreateResponseErrorResult<TValue>(response, e, ReturnResult<TValue>.STATUS_SERVER_ERROR);
            else
                return CreateResponseErrorResult<TValue>(response, e, ReturnResult<TValue>.STATUS_CLIENT_ERROR);
        }

        private static ReturnResult<TValue> CreateResponseErrorResult<TValue>(AjaxResponse response, Exception e, int status)
        {
            ReturnResult<TValue> result = new ReturnResult<TValue>();

            if (null != response)
            {
                result.Code = status; //TODO : Set the code more detail.
                result.OriginalContent = response.Text;
            }
            else
            {
                result.Code = status;
            }

            result.Exception = e;
            return result;
        }

        /// <summary>
        /// 服务请求失败后的回调方法。
        /// </summary>
        /// <typeparam name="TValue">传递给对返回信息进行处理的回调方法的参数的类型。</typeparam>
        /// <param name="service">服务的URL字符串。</param>
        /// <param name="callback">Ajax请求服务失败后对返回信息进行处理的回调方法。</param>
        /// <param name="resultContent">Ajax请求服务失败后返回的信息内容。</param>
        /// <param name="e">客户端的异常。</param>
        private static void OnServiceResultError<TValue>(string service,
            Action<ReturnResult<TValue>> callback, string resultContent, Exception e)
        {
            ReturnResult<TValue> result = new ReturnResult<TValue>();
            result.Code = ReturnResult<TValue>.STATUS_ERROR_RETURN;
            result.OriginalContent = resultContent;
            result.Exception = e;
            callback(result);
        }
    }
}
