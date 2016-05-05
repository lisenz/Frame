using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Frame.Service.Client
{
    /// <summary>
    /// 通用数据服务请求的返回数据结构。
    /// </summary>
    /// <typeparam name="TValue">结果数据在客户端需要转换的类型。</typeparam>
    public class ReturnResult<TValue>
    {
        /// <summary>
        /// 标识客户端请求不响应的代码。
        /// </summary>
        public const int STATUS_NOT_RETURNED = -1;

        /// <summary>
        /// 标识客户端请求响应失败的代码。
        /// </summary>
        public const int STATUS_CLIENT_ERROR = -2;

        /// <summary>
        /// 标识客户端请求服务失败的代码。
        /// </summary>
        public const int STATUS_ERROR_RETURN = -3;

        /// <summary>
        /// 服务器的内部错误的状态码。
        /// </summary>
        public const int STATUS_SERVER_ERROR = 500;

        /// <summary>
        /// 客户端Ajax请求已成功的状态码。
        /// </summary>
        public const int STATUS_OK = 200;

        /// <summary>
        /// 默认标识客户端请求不响应。
        /// </summary>
        private int _code = STATUS_NOT_RETURNED;

        /// <summary>
        /// 请求服务后返回的结果。
        /// </summary>
        private TValue _value;

        /// <summary>
        /// 获取一个值，该值标识是否请求成功。
        /// </summary>
        public bool IsOk
        {
            get { return STATUS_OK == Code; }
        }

        /// <summary>
        /// 获取或设置请求服务后返回的状态码。
        /// </summary>
        [JsonProperty(PropertyName = "returnCode")]
        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// 获取或设置请求服务后返回的状态字符串。
        /// </summary>
        [JsonProperty(PropertyName = "returnStatus")]
        public string Status { get; set; }

        /// <summary>
        /// 获取或设置请求服务的结果的说明描述。
        /// </summary>
        [JsonProperty(PropertyName = "returnDesc")]
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置请求服务的错误说明。
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        /// <summary>
        /// 获取请求服务执行返回的结果。
        /// </summary>
        public TValue Value
        {
            get
            {
                if (null == _value)
                {
                    _value = ValueFor<TValue>();
                }
                return _value;
            }
        }

        /// <summary>
        /// 获取或设置请求服务执行返回的结果字符串。
        /// </summary>
        [JsonProperty(PropertyName = "returnValue")]
        [JsonConverter(typeof(ReturnValueConverter))]
        public string RawValue { get; set; }

        /// <summary>
        /// 从HTTP响应中获取到的最原始的内容。
        /// </summary>
        public string OriginalContent { get; internal set; }

        /// <summary>
        /// 客户端异常。
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// 将服务请求的结果序列化转换为泛型参数指定类型的对象。
        /// </summary>
        /// <typeparam name="T">要转换序列化的类型。</typeparam>
        /// <returns>转换后的结果。</returns>
        public T ValueFor<T>()
        {
            return (T)DeserialzeValue(typeof(T));
        }

        /// <summary>
        /// 根据类型参数进行序列化。
        /// </summary>
        /// <param name="type">要序列化转换的类型。</param>
        /// <returns>转换后的结果。</returns>
        private object DeserialzeValue(Type type)
        {
            if (type == typeof(string) || type == typeof(object))
            {
                return RawValue;
            }

            if (string.IsNullOrEmpty(RawValue))
            {
                return null;
            }

            //TODO : Exception Handling
            return JsonConvert.DeserializeObject(RawValue, type);
        }
    }
}
