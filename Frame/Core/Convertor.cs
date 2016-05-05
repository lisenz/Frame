using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
//----------
using Frame.Core.Extensions;
using Frame.Core.Reflection.Fast;

namespace Frame.Core
{
    /// <summary>
    /// 表示一组方法，提供将泛型参数指定类型的实体或指定对象转换为另外指定类型的对象。
    /// </summary>
    public sealed class Convertor
    {
        /// <summary>
        /// 将泛型参数指定类型的实体对象转换为键值对集合类型的对象。
        /// </summary>
        /// <typeparam name="T">要转换的类型。</typeparam>
        /// <param name="entity">泛型参数指定类型的要进行转换的实体对象。</param>
        /// <returns>转换后的键值对集合类型的对象。</returns>
        public static IDictionary<string, object> ConvertToDictionary<T>(T entity) where T : class
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            PropertyInfo[] props =
                entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            props.ForEach(prop =>
            {
                string name = prop.Name;
                object value = prop.FastGetValue(entity);
                dictionary.Add(name, value);
            });

            return dictionary;
        }

        /// <summary>
        /// 将指定object对象转换为泛型参数指定类型的对象。
        /// </summary>
        /// <typeparam name="T">要转换的类型。</typeparam>
        /// <param name="value">要转换类型的object对象。</param>
        /// <returns>转换的泛型参数指定类型的对象。</returns>
        public static T Convert<T>(object value)
        {
            if (null == value || value is DBNull)
                return default(T);
            else
                return (T)Convert(typeof(T), value);
        }

        /// <summary>
        /// 将指定object对象转换为指定类型。
        /// </summary>
        /// <param name="type">要转换的类型。</param>
        /// <param name="value">要转换类型的对象。</param>
        /// <returns>转换为指定类型的对象。</returns>
        public static object Convert(Type type, object value)
        {
            if (null == value || value is DBNull)
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            // 注意：Type.IsAssignableFrom()是常用的“类型检测”手段，Type.IsAssignableFrom()支持接口检测。
            //       这里检测type的类型是否与value的类型是否相同或者是继承value的类型。
            if (type.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            if (type.IsPrimitive)
            {
                // 注意：基元类型是 Boolean、Byte、SByte、Int16、UInt16、Int32、UInt32、Int64、UInt64、Char、Double 和 Single
                //       这里检测到value对象要转换的类型为基类型，即将value转换为指定基类型之一
                try
                {
                    return System.Convert.ChangeType(value, type);
                }
                catch
                {
                }
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (null != converter && converter.CanConvertFrom(value.GetType()))
            {
                return converter.ConvertFrom(value);
            }

            converter = TypeDescriptor.GetConverter(value.GetType());
            if (null != converter && converter.CanConvertTo(type))
            {
                return converter.ConvertTo(value, type);
            }

            if (typeof(string).Equals(type))
            {
                return value.ToString();
            }
            throw new InvalidCastException(
                string.Format("无法将类型'{0}' 转换为 '{1}'.",
                                value.GetType().FullName, type.FullName));
        }

        /// <summary>
        /// 将指定值转换为指定的 Type，并允许数据丢失。
        /// </summary>
        /// <param name="src">要转换为新类型的对象。</param>
        /// <param name="targetType">要转换到src的类型。</param>
        /// <returns>转换为类型targetType的值。</returns>
        public static object ConvertT(object src, Type targetType)
        {
            var num = Microsoft.JScript.Convert.CoerceT(src, targetType, true);
            var isNaNMethod = targetType.GetMethod("IsNaN", BindingFlags.Public | BindingFlags.Static);
            if (isNaNMethod == null)
            {
                return num;
            }

            if ((bool)isNaNMethod.FastInvoke(null, num))
            {
                throw new InvalidCastException(string.Format("无法将对象从类型'{0}'转换为'{1}'。", src, targetType.FullName));
            }

            return num;
        }
    }
}
