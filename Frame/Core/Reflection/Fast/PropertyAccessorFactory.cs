using System;
//--------------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 创建对属性元数据访问的访问器的工厂对象。
    /// </summary>
    internal class PropertyAccessorFactory : IFastReflectionFactory<PropertyInfo, IPropertyAccessor>
    {
        /// <summary>
        /// 创建以PropertyInfo类型为键的属性访问器对象。
        /// </summary>
        /// <param name="key">要获取的属性访问器的PropertyInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的属性访问器对象。</returns>
        public IPropertyAccessor Create(PropertyInfo key)
        {
            return new PropertyAccessor(key);
        }

        /// <summary>
        /// 创建以PropertyInfo类型为键的属性访问器对象。
        /// </summary>
        /// <param name="key">要获取的属性访问器的PropertyInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的属性访问器对象。</returns>
        IPropertyAccessor IFastReflectionFactory<PropertyInfo, IPropertyAccessor>.Create(PropertyInfo key)
        {
            return this.Create(key);
        }
    }
}
