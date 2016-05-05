using System;
//--------------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 创建对构造函数元数据访问的访问器的工厂对象。
    /// </summary>
    internal class ConstructorAccessorFactory : IFastReflectionFactory<ConstructorInfo, IConstructorAccessor>
    {
        /// <summary>
        /// 创建以ConstructorInfo类型为键的字段访问器对象。
        /// </summary>
        /// <param name="key">要获取的字段访问器的ConstructorInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的构造函数访问器对象。</returns>
        public IConstructorAccessor Create(ConstructorInfo key)
        {
            return new ConstructorAccessor(key);
        }

        /// <summary>
        /// 创建以ConstructorInfo类型为键的字段访问器对象。
        /// </summary>
        /// <param name="key">要获取的字段访问器的ConstructorInfo类型的键值。</param>
        /// <returns>返回与指定的键相关联的构造函数访问器对象。</returns>
        IConstructorAccessor IFastReflectionFactory<ConstructorInfo, IConstructorAccessor>.Create(ConstructorInfo key)
        {
            return this.Create(key);
        }
    }
}
