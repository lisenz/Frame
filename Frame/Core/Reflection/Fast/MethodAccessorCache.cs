using System;
//----------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 方法访问器的缓存类，提供内部创建MethodInfo为键值的访问器。
    /// </summary>
    internal class MethodAccessorCache : FastReflectionCache<MethodInfo, IMethodAccessor>
    {
        /// <summary>
        /// 创建MethodInfo为键值的方法访问器。
        /// </summary>
        /// <param name="key">要创建的方法访问器的键值。</param>
        /// <returns>从方法访问器工厂创建的方法访问器。</returns>
        protected override IMethodAccessor Create(MethodInfo key)
        {
            return FastReflectionFactories.MethodAccessorFactory.Create(key);
        }
    }
}
