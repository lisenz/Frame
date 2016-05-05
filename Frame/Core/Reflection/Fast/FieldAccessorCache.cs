using System;
//-----------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 字段访问器的缓存类，提供内部创建FieldInfo为键值的访问器。
    /// </summary>
    internal class FieldAccessorCache : FastReflectionCache<FieldInfo, IFieldAccessor>
    {
        /// <summary>
        /// 创建FieldInfo为键值的方法访问器。
        /// </summary>
        /// <param name="key">要创建的字段访问器的键值。</param>
        /// <returns>从字段访问器工厂创建的字段访问器。</returns>
        protected override IFieldAccessor Create(FieldInfo key)
        {
            return FastReflectionFactories.FieldAccessorFactory.Create(key);
        }
    }
}
