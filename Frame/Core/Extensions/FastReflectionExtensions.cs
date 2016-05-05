using System;
//----------
using System.Reflection;
using Frame.Core.Reflection.Fast;

namespace Frame.Core.Extensions
{
    /// <summary>
    /// 快速反射访问器的拓展类，提供访问指定对象的对应元数据的快速访问方法。
    /// </summary>
    public static class FastReflectionExtensions
    {
        /// <summary>
        /// 获取给定对象支持的字段的值。
        /// </summary>
        /// <param name="fieldInfo">给定对象支持的元数据字段对象。</param>
        /// <param name="instance">其字段值所属的对象。</param>
        /// <returns>instance参数的字段值。</returns>
        public static object FastGetValue(this FieldInfo fieldInfo, object instance)
        {
            return FastReflectionCaches.FieldAccessorCache.Get(fieldInfo).GetValue(instance);
        }

        /// <summary>
        /// 获取给定对象支持的属性的值。
        /// </summary>
        /// <param name="propertyInfo">给定对象支持的元数据属性对象。</param>
        /// <param name="instance">其属性值所属的对象。</param>
        /// <returns>instance参数的属性值。</returns>
        public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
        {
            return FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).GetValue(instance);
        }

        /// <summary>
        /// 设置给定对象支持的该属性的值。
        /// </summary>
        /// <param name="propertyInfo">给定对象支持的元数据属性对象，</param>
        /// <param name="instance">将设置其属性值的对象。</param>
        /// <param name="value">此属性的新值。</param>
        public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
        {
            FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).SetValue(instance, value);
        }

        /// <summary>
        /// 使用指定的参数调用当前实例所表示的方法。
        /// </summary>
        /// <param name="methodInfo">当前实例所表示的元数据方法对象。</param>
        /// <param name="instance">对其调用方法的对象。</param>
        /// <param name="parameters">调用的方法的参数列表。这是一个对象数组，
        /// 这些对象与要调用的方法的参数具有相同的数量、顺序和类型。如果没有任何参数，则 parameters应为 null。</param>
        /// <returns>被调用方法的返回值。</returns>
        public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            return FastReflectionCaches.MethodAccessorCache.Get(methodInfo).Invoke(instance, parameters);
        }

        /// <summary>
        /// 调用具有指定参数的实例所反映的构造函数。
        /// </summary>
        /// <param name="constructorInfo">具有指定参数的实例所反映的构造函数元数据对象。</param>
        /// <param name="parameters">与此构造函数的参数的个数、顺序和类型（受默认联编程序的约束）相匹配的值数组。
        /// 如果此构造函数没有参数，则像 Object[] parameters new Object[0] 中那样，使用包含零元素或 null 的数组。</param>
        /// <returns>与构造函数关联的类的实例。</returns>
        public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters)
        {
            return FastReflectionCaches.ConstructorAccessorCache.Get(constructorInfo).Invoke(parameters);
        }
    }
}
