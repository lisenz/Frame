using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Frame.Core;
using Frame.Core.Extensions;
using Frame.Service.Server.Extensions;

namespace Frame.Service.Server.Core
{
    /// <summary>
    /// 表示服务中的被进行调用的方法。
    /// </summary>
    public class ServiceMethod
    {
        /// <summary>
        /// JSON序列化配置对象。
        /// </summary>
        private static readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// JSON序列化对象。
        /// </summary>
        private static readonly JsonSerializer _jsonSerializer;

        /// <summary>
        /// 方法元数据对象。
        /// </summary>
        private readonly MethodInfo _methodInfo;

        /// <summary>
        /// 静态构造函数，对内部JSON序列化配置的格式初始化以及创建。
        /// </summary>
        static ServiceMethod()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[]{
                        new IsoDateTimeConverter{
                            DateTimeFormat = Constants.DateTimeFormat
                        }
                    }
            };
            _jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
        }

        /// <summary>
        /// 构造函数，初始化服务方法信息。
        /// </summary>
        /// <param name="methodInfo">被调用的服务方法元数据对象。</param>
        public ServiceMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        /// <summary>
        /// 获取一个值，该值标识是否静态方法。
        /// </summary>
        public bool IsStatic { get { return _methodInfo.IsStatic; } }

        /// <summary>
        /// 静态方法调用。
        /// </summary>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>返回方法调用执行后的结果。</returns>
        public object Invoke(IServiceContext context)
        {
            return Invoke(null, context);
        }
        
        /// <summary>
        /// 实例方法调用。
        /// </summary>
        /// <param name="instance">调用方法的实例对象。</param>
        /// <param name="context">服务上下文对象，提供调用方法所需的参数等信息。</param>
        /// <returns>返回方法调用执行后的结果。</returns>
        public object Invoke(object instance, IServiceContext context)
        {
            if (_methodInfo.GetParameters().Length == 0)
            {
                return Invoke(instance, new object[0]);
            }

            return Invoke(instance, ParamsMatch(context));
        }

        #region 内部函数
        
        /// <summary>
        /// 调用执行方法。
        /// </summary>
        /// <param name="instance">调用方法的实例对象。</param>
        /// <param name="paramsArray">方法执行所需的参数数组。</param>
        /// <returns>返回方法调用执行后的结果。</returns>
        protected virtual object Invoke(object instance, params object[] paramsArray)
        {
            if (null == instance)
            {
                return null;
            }
            return _methodInfo.FastInvoke(instance, paramsArray);
        }

        /// <summary>
        /// 进行参数匹配，将从JSON对象转换出来的数据与方法参数进行精确的匹配
        /// </summary>
        /// <param name="context">服务的上下文对象。</param>
        /// <returns>返回匹配完成的参数集合。</returns>
        protected virtual object[] ParamsMatch(IServiceContext context)
        {
            IDictionary<string, object> parameters = context.Params;

            ParameterInfo[] paramInfos = _methodInfo.GetParameters();

            object[] paramsArray = new object[paramInfos.Length];

            foreach (ParameterInfo parameterInfo in paramInfos)
            {
                if (parameterInfo.IsOut || parameterInfo.IsRetval)
                {
                    throw new NotSupportedException(string.Format("方法{0}不支持输出参数.", _methodInfo.Name));
                }

                string paramName = parameterInfo.ParamName();
                Type paramType = parameterInfo.ParameterType;
                object paramValue;

                try
                {
                    if (TryMatch(paramName, paramType, parameters, out paramValue))
                    {
                        //名称匹配，进行对象转换
                        paramsArray[parameterInfo.Position] = paramValue;
                    }
                    else if (paramType.Equals(typeof(IServiceContext)))
                    {
                        paramsArray[parameterInfo.Position] = context;
                    }
                    else if (paramType.Equals(typeof(IDictionary<string, object>)))
                    {
                        //IDictionary类型的参数如果没有匹配的名称，那么使用整个rawParameters TODO : 是否忽略非简单类型的参数
                        paramsArray[parameterInfo.Position] = parameters;
                    }
                    else if (paramType.IsClass && !paramType.FullName.StartsWith("System."))
                    {
                        //实体对象
                        object instance = Activator.CreateInstance(paramType);

                        PropertyInfo[] props =
                            paramType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

                        props.ForEach(prop =>
                        {
                            string name = prop.Name;
                            Type type = prop.PropertyType;
                            object value;

                            if (TryMatch(name, type, parameters, out value) ||
                                TryMatch(name.Substring(0, 1).ToLower() + name.Substring(1), type, parameters, out value))
                            {
                                prop.FastSetValue(instance, value);
                            }

                        });

                        paramsArray[parameterInfo.Position] = instance;
                    }
                    else
                    {
                        if (CreateDefaultValue(paramType, out paramValue))
                        {
                            paramsArray[parameterInfo.Position] = paramValue;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ServiceException(desc: string.Format("绑定参数'{0}'出错:'{1}'。", paramName, e.Message),
                                               innerException: e);
                }
            }

            return paramsArray;
        }

        /// <summary>
        /// 获取捕获与指定参数名称相关联的值，并将其转换为指定类型的对象。
        /// </summary>
        /// <param name="name">参数列表中要获取值的键。</param>
        /// <param name="type">进行捕获配对的类型。</param>
        /// <param name="parameters">进行捕获匹配的参数列表。</param>
        /// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。</param>
        /// <returns>如果对象包含具有指定键的元素，则为true；否则，为 false。</returns>
        protected virtual bool TryMatch(string name, Type type, IDictionary<string, object> parameters, out object value)
        {
            if (parameters.TryGetValue(name, out value))
            {
                //值为空或者类型匹配，直接绑定
                if (null == value || type.IsAssignableFrom(value.GetType()))
                {
                    return true;
                }
                else if (value is JContainer)
                {
                    //如果是Json对象，把Json对象转换为对应的参数值
                    var container = value as JContainer;
                    Type convertType;
                    if (type.IsArray && type.GetElementType().IsAssignableFrom(typeof(IDictionary)))
                    {
                        convertType = typeof(Dictionary<string, object>[]);
                    }
                    else if (type.IsAssignableFrom(typeof(IDictionary)))
                    {
                        convertType = typeof(Dictionary<string, object>);
                    }
                    else
                    {
                        convertType = type;
                    }

                    value = _jsonSerializer.Deserialize(container.CreateReader(), convertType);
                }
                else if (type.IsArray)
                {
                    value = ConvertToArrayObject(name, type, value);
                }
                else
                {
                    //做类型转换
                    //value = TypeHelper.ConvertT(value, type);
                    value = Convertor.Convert(type, value);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将与指定键相关联的值进行类型转换为数组对象。
        /// </summary>
        /// <param name="name">参数列表中要获取值的键。</param>
        /// <param name="type">要进行转换的类型。</param>
        /// <param name="value">要进行转换的值对象。</param>
        /// <returns>若值为null，返回一个0索引的数组对象；若值本身为数组类型，则返回原值；
        /// 若为字符串类型，则进行分割后转换为数组并反射为指定类型的对象赋值；
        /// 若以上情况都不符合，则以object类型的数组进行类型转换为指定类型的对象。</returns>
        protected virtual object ConvertToArrayObject(string name, Type type, object value)
        {
            if (null == value)
            {
                return Array.CreateInstance(type.GetElementType(), 0);
            }
            else if (value.GetType().IsArray)
            {
                if (type.IsAssignableFrom(value.GetType().GetElementType()))
                {
                    return value;
                }
                else
                {
                    throw new ServiceException(desc:
                        string.Format("参数值'{0}'的类型为'{2}',不是预期允许转换的类型'{1}'。",
                            name, type.FullName, value.GetType().GetElementType().FullName));

                }
            }
            else
            {
                object[] values = value is string ? ((string)value).Split(',') : new object[] { value.ToString() };
                Array array = Array.CreateInstance(type.GetElementType(), values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    object obj = values[i];
                    array.SetValue(Convertor.ConvertT(obj, type.GetElementType()), i);
                }

                return array;
            }
        }

        /// <summary>
        /// 创建指定Type的参数值。
        /// </summary>
        /// <param name="type">要创建的参数值的类型。</param>
        /// <param name="value">此方法返回时，若成功创建使用从零开始的索引、具有指定 System.Type 和长度的一维System.Array对象，则将该对象作为默认值返回；否则返回null。</param>
        /// <returns>如果value不为null，返回true；否则返回false。</returns>
        protected bool CreateDefaultValue(Type type, out object value)
        {
            if (type.IsArray)
            {
                value = Array.CreateInstance(type.GetElementType(), 0);

                return true;
            }
            value = null;
            return false;
        }

        #endregion

    }
}
