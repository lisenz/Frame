using System;
using System.Data;

namespace Frame.DataStore.Map.Attributes
{
    /// <summary>
    /// 属性列特性，映射数据库字段。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : NamedAttribute
    {
        /// <summary>
        /// 构造函数,初始化属性列。
        /// </summary>
        /// <param name="name">属性特性名称，映射数据库中字段的名称。</param>
        public ColumnAttribute(string name)
            : base(name)
        {

        }

        /// <summary>
        /// 映射数据库字段数据类型。
        /// </summary>
        public DbType DataType { get; set; }
    }
}
