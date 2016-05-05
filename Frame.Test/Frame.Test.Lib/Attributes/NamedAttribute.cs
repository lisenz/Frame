using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Test.Lib.Attributes
{
    public class NamedAttribute : Attribute
    {
        /// <summary>
        /// 构造函数,初始化属性列。
        /// </summary>
        /// <param name="name">属性特性名称。</param>
        public NamedAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取属性特性名称。
        /// </summary>
        public string Name { get; private set; }
    }
}
