using System;
//----------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 构造函数访问器的缓存类，提供内部创建ConstructorInfo为键值的访问器。
    /// </summary>
    internal class ConstructorAccessorCache : FastReflectionCache<ConstructorInfo, IConstructorAccessor>
    {
        /// <summary>
        /// 创建ConstructorInfo为键值的属性访问器。
        /// </summary>
        /// <param name="key">要创建的构造函数访问器的键值。</param>
        /// <returns>从构造函数访问器工厂创建的构造函数访问器。</returns>
        protected override IConstructorAccessor Create(ConstructorInfo key)
        {
            return FastReflectionFactories.ConstructorAccessorFactory.Create(key);
        }
    }
}
