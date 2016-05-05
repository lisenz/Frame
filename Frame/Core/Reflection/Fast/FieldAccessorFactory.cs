using System;
//--------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 创建对字段元数据访问的访问器的工厂对象。
    /// </summary>
    internal class FieldAccessorFactory : IFastReflectionFactory<FieldInfo, IFieldAccessor>
    {
        /// <summary>
        /// 创建以FieldInfo类型为键的字段访问器对象。
        /// </summary>
        /// <param name="key">要获取的字段访问器的FieldInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的字段访问器对象。</returns>
        public IFieldAccessor Create(FieldInfo key)
        {
            return new FieldAccessor(key);
        }

        /// <summary>
        /// 创建以FieldInfo类型为键的字段访问器对象。
        /// </summary>
        /// <param name="key">要获取的字段访问器的FieldInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的字段访问器对象。</returns>
        IFieldAccessor IFastReflectionFactory<FieldInfo, IFieldAccessor>.Create(FieldInfo key)
        {
            return this.Create(key);
        }
    }
}
