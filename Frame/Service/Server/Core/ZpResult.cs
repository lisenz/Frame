using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Frame.Service.Server.Core
{
    /// <summary>
    /// 因振潮的项目临时新增的协议类，完成后删除
    /// </summary>
    internal class ZpResult
    {
        /// <summary>
        /// 获取或设置异常信息的状态码。
        /// </summary>
        [JsonProperty(PropertyName = "ResultInt")]
        public int ReturnCode { get; set; }

        /// <summary>
        /// 获取或设置异常信息的状态描述。
        /// </summary>
        [JsonProperty(PropertyName = "Msg")]
        public string ReturnDesc { get; set; }

        /// <summary>
        /// 获取或设置异常信息的错误描述。
        /// </summary>
        [JsonProperty(PropertyName = "d_data")]
        public object ReturnValue { get; set; }

        
        /// <summary>
        /// 初始化设置JSON序列化的信息的转换对象。
        /// </summary>
        [JsonIgnore]
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[]
            {
                new IsoDateTimeConverter{
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                }
            }
        };

        /// <summary>
        /// 构造函数，初始化JSON结果对象。
        /// </summary>
        internal ZpResult()
        {

        }

        /// <summary>
        /// 构造函数，设置JSON结果对象的信息。
        /// </summary>
        /// <param name="returnValue">JSON结果对象的返回结果体。</param>
        internal ZpResult(object returnValue)
        {
            ReturnCode = 0;
            ReturnDesc = "OK";
            ReturnValue = returnValue;
        }

        /// <summary>
        /// 构造函数，设置JSON结果对象的服务请求异常。
        /// </summary>
        /// <param name="e">处理服务请求过程中的异常对象。</param>
        internal ZpResult(ServiceException e)
        {
            ReturnCode = 1;
            ReturnDesc = e.Description;
            ReturnValue = e;
        }

        /// <summary>
        /// 构造函数，设置JSON结果对象的服务请求异常。
        /// </summary>
        /// <param name="e">处理服务请求过程中的系统异常错误对象。</param>
        internal ZpResult(Exception e)
        {
            ReturnCode = 1;
            ReturnDesc = e.Message;
            ReturnValue = e;
        }

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
