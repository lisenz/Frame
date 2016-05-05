using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Frame.Core;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using Commons.Collections;
using System.Web;
using System.IO;

namespace Frame.Service.Server.Core
{
    internal class PageResult
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

        internal string Template { get; set; }
        internal bool IsAjax { get; set; }

        /// <summary>
        /// 构造函数，初始化页面输出结果对象。
        /// </summary>
        internal PageResult()
        {

        }

        internal PageResult(object returnValue, string template, bool isAjax = false)
        {
            ReturnCode = Ok;
            ReturnDesc = OkDesc;
            Error = string.Empty;
            Template = template;
            IsAjax = isAjax;
            ReturnValue = returnValue;
        }

        /// <summary>
        /// 构造函数，设置页面输出结果对象的服务请求异常。
        /// </summary>
        /// <param name="e">处理服务请求过程中的异常对象。</param>
        internal PageResult(ServiceException e)
        {
            ReturnCode = e.Code;
            ReturnDesc = e.Description;
            Error = e.ToString();
            IsAjax = true;
        }

        /// <summary>
        /// 构造函数，设置页面输出结果对象的服务请求异常。
        /// </summary>
        /// <param name="e">处理服务请求过程中的系统异常错误对象。</param>
        internal PageResult(Exception e)
        {
            ReturnCode = ServerError;
            ReturnDesc = ServerErrorDesc;
            Error = e.ToString();
            IsAjax = true;
        }

        public string ToHtml()
        {
            VelocityEngine ve = SingleProvider<VelocityEngine>.Instance;//模板引擎实例化
            ExtendedProperties ep = new ExtendedProperties();//模板引擎参数实例化
            ep.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");//指定资源的加载类型
            ep.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, HttpRuntime.AppDomainAppPath);//指定资源的加载路径
            //ep.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");//输入格式
            //ep.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");//输出格式

            //模板的缓存设置
            //ep.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, true); //是否缓存
            //ep.AddProperty("file.resource.loader.modificationCheckInterval", (Int64)300); //缓存时间(秒)
            ve.Init(ep);

            Template t = ve.GetTemplate(this.Template);//加载模板
            VelocityContext vc = new VelocityContext();
            object o = vc.Put("PageResult", this.ReturnValue);
            
            StringWriter writer = new StringWriter();
            t.Merge(vc, writer);
            
            string result = this.IsAjax ? JsonConvert.SerializeObject(writer.GetStringBuilder().ToString(), Formatting.None) : writer.GetStringBuilder().ToString();
            return result;
        }

        /// <summary>
        /// 创建并返回当前对象的JSON格式的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToHtml();
        }
    }
}
