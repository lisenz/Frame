using System;
//--------------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 创建对方法元数据访问的访问器的工厂对象。
    /// </summary>
    internal class MethodAccessorFactory : IFastReflectionFactory<MethodInfo, IMethodAccessor>
    {
        /// <summary>
        /// 创建以MethodInfo类型为键的方法访问器对象。
        /// </summary>
        /// <param name="key">要获取的方法访问器的MethodInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的方法访问器对象。</returns>
        public IMethodAccessor Create(MethodInfo key)
        {
            return new MethodAccessor(key);
        }

        /// <summary>
        /// 创建以MethodInfo类型为键的方法访问器对象。
        /// </summary>
        /// <param name="key">要获取的方法访问器的MethodInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的方法访问器对象。</returns>
        IMethodAccessor IFastReflectionFactory<MethodInfo, IMethodAccessor>.Create(MethodInfo key)
        {
            return this.Create(key);
        }
    }
}
