using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Service.Client
{
    /// <summary>
    /// 通用服务，提供调用服务的属性和方法。
    /// </summary>
    public class DataService
    {
        /// <summary>
        /// 默认服务。
        /// </summary>
        private static string _globalServiceUri = "";

        /// <summary>
        /// 私有构造函数。
        /// </summary>
        private DataService()
        {

        }

        /// <summary>
        /// 获取或设置全局服务URL路径。
        /// </summary>
        public static string GlobalServiceUri
        {
            get { return _globalServiceUri; }
            set { _globalServiceUri = value; }
        }

        #region 同步调用

        public static ReturnResult<DataValue> SynExcute(string command)
        {
            return Execute<DataValue>(command, null, options: null);
        }

        public static ReturnResult<DataValue> SynExcute(string command, IDictionary<string, object> parameters)
        {
            return Execute<DataValue>(command, parameters, options: null);
        }

        public static ReturnResult<DataValue> SynExcute(string command, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            return Execute<DataValue>(command, parameters, options: options);
        }

        public static ReturnResult<TValue> SynExcute<TValue>(string command, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            return Execute<TValue>(command, parameters, options: options);
        }

        public static ReturnResult<DataValue> SynQuery(string command)
        {
            return SynQuery(command, null);
        }

        public static ReturnResult<DataValue> SynQuery(string command, IDictionary<string, object> parameters)
        {
            return SynQuery(command, parameters, options: null);
        }

        public static ReturnResult<DataValue> SynQuery(string command, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            return SynQuery<DataValue>(command, parameters, options);
        }

        public static ReturnResult<TValue> SynQuery<TValue>(string command, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            return Execute<TValue>(command, parameters, options: options);
        }

        #endregion

        #region 异步调用

        public static void AsynExcute(string command, Action<ReturnResult<DataValue>> callback)
        {
            AsynExcute(command, null, callback);
        }

        public static void AsynExcute(string command, IDictionary<string, object> parameters, Action<ReturnResult<DataValue>> callback)
        {
            AsynExcute<DataValue>(command, parameters, callback);
        }

        public static void AsynExcute<TValue>(string command, IDictionary<string, object> parameters, Action<ReturnResult<TValue>> callback)
        {
            Execute<TValue>(command, parameters, callback, options: null);
        }

        public static void AsynQuery(string command, Action<ReturnResult<DataValue>> callback)
        {
            AsynQuery(command, null, callback);
        }

        public static void AsynQuery(string command, IDictionary<string, object> parameters, Action<ReturnResult<DataValue>> callback)
        {
            AsynQuery(command, parameters, callback, options: null);
        }

        public static void AsynQuery(string command, IDictionary<string, object> parameters, Action<ReturnResult<DataValue>> callback, AjaxOptions? options = null)
        {
            AsynQuery<DataValue>(command, parameters, callback, options);
        }

        public static void AsynQuery<TValue>(string command, IDictionary<string, object> parameters, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            Execute<TValue>(command, parameters, callback, options);
        }

        #endregion

        private static ReturnResult<TValue> Execute<TValue>(string command, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            if (string.IsNullOrEmpty(GlobalServiceUri))
            {
                throw new InvalidOperationException("'GlobalServiceUri' 不能为空.");
            }
            return Execute<TValue>(GlobalServiceUri, command, parameters, options);
        }

        private static void Execute<TValue>(string command, IDictionary<string, object> parameters, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            if (string.IsNullOrEmpty(GlobalServiceUri))
            {
                throw new InvalidOperationException("'GlobalServiceUri' 不能为空.");
            }
            Execute<TValue>(GlobalServiceUri, command, parameters,callback, options);
        }

        /// <summary>
        /// 内置通用同步调用服务执行入口方法。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务URL完全路径。</param>
        /// <param name="command">服务指令。</param>
        /// <param name="parameters">服务指令的参数。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        /// <returns>返回指定类型的请求结果对象。</returns>
        private static ReturnResult<TValue> Execute<TValue>(string service, string command, IDictionary<string, object> parameters, AjaxOptions? options = null)
        {
            object parames = new { CommandName = command, Params = parameters };

            return HttpClient.SynCall<TValue>(service, parames, options);
        }

        /// <summary>
        /// 内置通用异步调用服务执行入口方法。
        /// </summary>
        /// <typeparam name="TValue">返回结果的类型。</typeparam>
        /// <param name="service">服务URL完全路径。</param>
        /// <param name="command">服务指令。</param>
        /// <param name="parameters">服务指令的参数。</param>
        /// <param name="callback">调用服务执行后对返回结果进行处理的回调方法。</param>
        /// <param name="options">Ajax全局选项设置。</param>
        private static void Execute<TValue>(string service, string command, IDictionary<string, object> parameters, Action<ReturnResult<TValue>> callback, AjaxOptions? options = null)
        {
            object parames = new { CommandName = command, Params = parameters };

            HttpClient.AsynCall<TValue>(service, parames,
                                     (result) => { OnExecute(command, result, callback); },
                                     options: options);
        }

        /// <summary>
        /// 统一对调用服务执行后对返回结果进行处理的回调方法。
        /// </summary>
        /// <typeparam name="TValue">结果的类型。</typeparam>
        /// <param name="command">服务指令。</param>
        /// <param name="result">请求服务执行返回的结果对象。</param>
        /// <param name="callback">对结果进行最后处理的回调方法。</param>
        private static void OnExecute<TValue>(string command, ReturnResult<TValue> result, Action<ReturnResult<TValue>> callback)
        {
            TValue value = result.Value;
            if (value is DataValue)
            {
                (value as DataValue).Command = command;
            }
            callback(result);
        }
    }
}
