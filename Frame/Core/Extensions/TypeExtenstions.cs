using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Core.Extensions
{
    public static class TypeExtenstions
    {
        /// <summary>
        /// 创建指定名称的 System.Type 对象的指定泛型类型参数所指定类型的实例。
        /// </summary>
        /// <typeparam name="TType">要创建的实例对象的类型。</typeparam>
        /// <param name="typeName">要创建的实例对象所属的程序集实例中指定的 System.Type 对象的名称，其字符串格式为["Namespace.Class,Dll文件名称"]。</param>
        /// <returns>对新创建对象的引用。</returns>
        public static TType CreateInstance<TType>(string typeName)
        {
            return GetType(typeName).CreateInstance<TType>();
        }

        /// <summary>
        /// 创建继承于指定泛型类型参数所指定类型的实例。
        /// </summary>
        /// <typeparam name="TType">要创建的实例对象的类型，也可以是父类类型。</typeparam>
        /// <param name="type">继承于指定泛型类型的派生类类型的 System.Type 对象。</param>
        /// <returns>对新创建对象的引用。</returns>
        public static TType CreateInstance<TType>(this Type type)
        {
            object obj = Activator.CreateInstance(type);
            if (!(obj is TType))
            {
                throw new InvalidCastException(string.Format("[{0}]类型的对象无法转换为类型 [{1}].",
                    obj.GetType().FullName, type.FullName));
            }

            return (TType)obj;
        }

        /// <summary>
        /// 获取具有指定名称的 System.Type 对象。
        /// </summary>
        /// <param name="typeName">要获取的类型的程序集限定名称，其字符串格式为["Namespace.Class,Dll文件名称"]。</param>
        /// <returns>具有指定名称的 System.Type（如果找到的话）；否则抛出异常信息。</returns>
        public static Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (null == type)
            {
                throw new Exception(string.Format("Type[{0}]无法检索到,请确保类型名称的正确性,例如:命名空间.类", typeName));
            }

            return type;
        }

        /// <summary>
        ///  搜索获取指定 System.Type 成员对象中标识指定泛型类型参数T所指定类型的特性成员数组序列中的第一个特性对象。
        /// </summary>
        /// <typeparam name="T">要搜索的特性类型。</typeparam>
        /// <param name="type">要获取特性的 System.Type 对象。</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性。</param>
        /// <returns>返回特性成员数组序列中的第一个元素；如果序列中不包含任何元素，则返回default(object)。</returns>
        public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            return (T)type.GetCustomAttributes(typeof(T), inherit).FirstOrDefault<object>();
        }
    }
}
