using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Frame.Service.Client
{
    /// <summary>
    /// 提供将请求服务的返回结果序列化转换的方法。
    /// </summary>
    internal sealed class ReturnValueConverter : JsonConverter
    {
        /// <summary>
        /// 返回一个值，该值指示对象类型是否可以转换序列化。
        /// </summary>
        /// <param name="objectType">要转换的对象类型。</param>
        /// <returns>默认为可以进行转换序列化。</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JRaw raw = (JRaw)serializer.Deserialize(reader, typeof(JRaw));
            return null == raw ? "" : raw.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //不需要实现序列化
        }
    }
}
