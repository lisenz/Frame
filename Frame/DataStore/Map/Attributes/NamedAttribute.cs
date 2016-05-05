using System;

namespace Frame.DataStore.Map.Attributes
{
    /// <summary>
    /// 命名属性特性对象的抽象基类。
    /// </summary>
    public abstract class NamedAttribute : Attribute
    {
        /// <summary>
        /// 构造函数,初始化属性列。
        /// </summary>
        /// <param name="name">属性特性名称。</param>
        protected NamedAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取属性特性名称。
        /// </summary>
        public string Name { get; private set; }
    }
}
