using System;
//----------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 属性访问器的缓存类，提供内部创建PropertyInfo为键值的访问器。
    /// </summary>
    internal class PropertyAccessorCache : FastReflectionCache<PropertyInfo, IPropertyAccessor>
    {
        /// <summary>
        /// 创建PropertyInfo为键值的属性访问器。
        /// </summary>
        /// <param name="key">要创建的属性访问器的键值。</param>
        /// <returns>从属性访问器工厂创建的属性访问器。</returns>
        protected override IPropertyAccessor Create(PropertyInfo key)
        {
            return FastReflectionFactories.PropertyAccessorFactory.Create(key);
        }
    }
}
