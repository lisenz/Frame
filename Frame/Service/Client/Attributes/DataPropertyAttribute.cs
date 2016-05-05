using System;

namespace Frame.Service.Client.Attributes
{
    /// <summary>
    /// 该标注描述了Property与结果集中的列的对应关系，可以被标注多次，因为不同
    /// 的命令中的列名称未必相同。
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class DataPropertyAttribute : Attribute
    {
        /// <summary>
        /// 被标注的Property对应的命令的名字, 如果不设置，则表示对于所有命令都适用，如果设置，必须与ColumnName配合使用
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 被标注的Property对应的列名字
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 被标注的Property是否为包含超过一列数据的复杂类型
        /// </summary>
        public bool ComplexType { get; set; }

        /// <summary>
        /// 解析string到被标注的Property类型的静态方法名称
        /// </summary>
        public string StaticParseMethod { get; set; }

        /// <summary>
        /// 被标注的Property在作为到Request的参数时可以自动解析出命令的参数，必须与Command配合使用
        /// </summary>
        public string ParamName { get; set; }
    }
}
