using System.Linq;
using System.Reflection;
using Frame.Service.Server.Attributes;

namespace Frame.Service.Server.Extensions
{
    /// <summary>
    /// 参数属性元数据的拓展类。
    /// </summary>
    public static class ParameterInfoExtensions
    {
        /// <summary>
        /// 获取标记了服务参数特性的属性的名称。
        /// </summary>
        /// <param name="param">要参数属性元数据对象</param>
        /// <returns>参数名称。</returns>
        public static string ParamName(this ParameterInfo param)
        {
            object[] attrs = param.GetCustomAttributes(typeof(ServiceParamAttribute), false);            
            foreach (ServiceParamAttribute attr in attrs.OfType<ServiceParamAttribute>())
            {
                return attr.Name;
            }
            return param.Name;
        }
    }
}
