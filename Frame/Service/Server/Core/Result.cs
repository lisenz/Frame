using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Frame.Service.Server.Core
{
    internal class Result
    {
        /// <summary>
        /// 表示客户端请求已成功的状态码。
        /// </summary>
        public const int Ok = 200;

        /// <summary>
        /// 表示客户端请求已成功的反馈信息。
        /// </summary>
        public const string OkDesc = "OK";

        /// <summary>
        /// 表示错误请求的状态码。
        /// </summary>
        public const int BadRequest = 400;

        /// <summary>
        /// 表示错误请求的反馈信息。
        /// </summary>
        public const string BadRequestDesc = "语法格式有误，服务器无法理解此请求。";

        /// <summary>
        /// 表示未找到的状态码。
        /// </summary>
        public const int NotFound = 404;

        /// <summary>
        /// 表示未找到的反馈信息。
        /// </summary>
        public const string NotFoundDesc = "链接指向的网页不存在。";

        /// <summary>
        /// 表示服务器的内部错误的状态码。
        /// </summary>
        public const int ServerError = 500;

        /// <summary>
        /// 表示服务器的内部错误的反馈信息。
        /// </summary>
        public const string ServerErrorDesc = "内部服务器错误。";

        /// <summary>
        /// 初始化设置JSON序列化的信息的转换对象。
        /// </summary>
        [JsonIgnore]
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[]
            {
                new IsoDateTimeConverter{
                    DateTimeFormat = Constants.DateTimeFormat
                }
            }
        };

        /// <summary>
        /// 构造函数，初始化JSON结果对象。
        /// </summary>
        internal Result()
        {

        }

        /// <summary>
        /// 构造函数，设置JSON结果对象的信息。
        /// </summary>
        /// <param name="returnValue">JSON结果对象的返回结果体。</param>
        internal Result(object returnValue)
        {
            ReturnCode = Ok;
            ReturnDesc = OkDesc;
            Error = string.Empty;
            ReturnValue = returnValue;
        }

        /// <summary>
        /// 构造函数，设置JSON结果对象的服务请求异常。
        /// </summary>
        /// <param name="e">处理服务请求过程中的异常对象。</param>
        internal Result(ServiceException e)
        {
            ReturnCode = e.Code;
            ReturnDesc = e.Description;
            Error = e.ToString();
        }

        /// <summary>
        /// 构造函数，设置JSON结果对象的服务请求异常。
        /// </summary>
        /// <param name="e">处理服务请求过程中的系统异常错误对象。</param>
        internal Result(Exception e)
        {
            ReturnCode = ServerError;
            ReturnDesc = ServerErrorDesc;
            Error = e.ToString();
        }

        /// <summary>
        /// 获取或设置异常信息的状态码。
        /// </summary>
        [JsonProperty(PropertyName = "returnCode")]
        public int ReturnCode { get; set; }

        /// <summary>
        /// 获取或设置异常信息的状态描述。
        /// </summary>
        [JsonProperty(PropertyName = "returnStatus")]
        public string ReturnStatus { get; set; }

        /// <summary>
        /// 获取或设置异常信息的错误描述。
        /// </summary>
        [JsonProperty(PropertyName = "returnDesc")]
        public string ReturnDesc { get; set; }

        /// <summary>
        /// 获取或设置异常信息的描述。
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        /// <summary>
        /// 获取或设置请求数据结果。
        /// </summary>
        [JsonProperty(PropertyName = "returnValue")]
        public object ReturnValue { get; set; }

        /// <summary>
        /// 转换为JSON格式的字符串形式进行输出。
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, _jsonSerializerSettings);
        }

        /// <summary>
        /// 创建并返回当前对象的JSON格式的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToJson();
        }
    }
}
