using System;
using System.Reflection;
using System.Collections.Generic;
using Frame.Core.Extensions;

namespace Frame.Core.Reflection
{
    public class AssemblyBuilder
    {
        private static AssemblyFastCache _Assemblyer;
        private static AssemblyBuilder _Builder;

        static AssemblyBuilder()
        {
            _Assemblyer = new AssemblyFastCache();
            _Builder = new AssemblyBuilder();
        }

        private AssemblyBuilder()
        {
        }

        /// <summary>
        /// 生成一个对象生成器。
        /// </summary>
        /// <returns>返回一个对象生成器。</returns>
        public static AssemblyBuilder Build()
        {
            if (null == _Builder)
                _Builder = new AssemblyBuilder();
            if (null == _Assemblyer)
                _Assemblyer = new AssemblyFastCache();

            return _Builder;
        }

        /// <summary>
        /// 获取指定程序集文件中的指定类型对象实例。
        /// </summary>
        /// <param name="dll">程序集完全路径的限定名称。</param>
        /// <param name="cls">要获取的对象的类型的程序集限定名称。</param>
        /// <returns>对新创建对象的引用。</returns>
        public object FastGetValue(string dll, string cls)
        {
            return FastGetValue(dll, cls, null);
        }

        /// <summary>
        /// 获取指定程序集文件中的指定类型对象实例，并转换为指定泛型类型参数所指定类型的实例。
        /// </summary>
        /// <typeparam name="T">要转换的类型。</typeparam>
        /// <param name="dll">程序集完全路径的限定名称。</param>
        /// <param name="cls">要获取的对象的类型的程序集限定名称。</param>
        /// <returns>对新创建对象的引用。</returns>
        public T FastGetValue<T>(string dll, string cls)
        {
            return FastGetValue<T>(dll, cls, null);
        }

        /// <summary>
        /// 获取指定程序集文件中的指定类型对象实例，并转换为指定泛型类型参数所指定类型的实例。
        /// </summary>
        /// <typeparam name="T">要转换的类型。</typeparam>
        /// <param name="dll">程序集完全路径的限定名称。</param>
        /// <param name="cls">要获取的对象的类型的程序集限定名称。</param>
        /// <param name="args">与要调用的构造函数的参数数量、顺序和类型匹配的参数数组；若为null则表示调用默认构造函数。</param>
        /// <returns>对新创建对象的引用。</returns>
        public T FastGetValue<T>(string dll, string cls, params object[] args)
        {
            return (T)FastGetValue(dll, cls, args);
        }

        /// <summary>
        /// 获取指定程序集文件中的指定类型对象实例。
        /// </summary>
        /// <param name="dll">程序集完全路径的限定名称。</param>
        /// <param name="cls">要获取的对象的类型的程序集限定名称。</param>
        /// <param name="args">与要调用的构造函数的参数数量、顺序和类型匹配的参数数组；若为null则表示调用默认构造函数。</param>
        /// <returns>对新创建对象的引用。</returns>
        public object FastGetValue(string dll, string cls, params object[] args)
        {
            Assembly assembly;
            if (_Assemblyer.TryGetValue(dll, out assembly))
            {
                Type type = assembly.GetType(cls);
                object obj = Activator.CreateInstance(type, args);
                return obj;
            }
            else
                return null;
        }

        /// <summary>
        /// 获取指定程序集文件中的指定类型对象实例的类型声明对象。
        /// </summary>
        /// <param name="dll">程序集完全路径的限定名称。</param>
        /// <param name="cls">要获取的对象的类型的程序集限定名称。</param>
        /// <returns>指定类型对象实例的类型。</returns>
        public Type TryGetType(string dll, string cls)
        {
            Assembly assembly;
            if (_Assemblyer.TryGetValue(dll, out assembly))
            {
                Type type = assembly.GetType(cls);
                return type;
            }
            else
                return null;
        }

        /// <summary>
        /// 卸载应用程序域中指定键关联的程序集对象。
        /// </summary>
        /// <param name="dll">程序集完全路径的限定名称。</param>
        public void UnLoad(string dll)
        {
            _Assemblyer.Unload(dll);
        }

        
    }
}
