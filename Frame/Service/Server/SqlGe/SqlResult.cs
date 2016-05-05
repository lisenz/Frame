using System;
using System.Collections.Generic;

namespace Frame.Service.Server.SqlGe
{
    /// <summary>
    /// 表示SQL执行命令返回结果对象。
    /// </summary>
    public class SqlResult
    {
        /// <summary>
        /// 执行命令影响的数据行数
        /// </summary>
        public int AffectRows { get; set; }

        /// <summary>
        /// 命令所有的行数，分页用
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string[] ColumnNames { get; set; }

        /// <summary>
        /// 对应字段的数据类型
        /// </summary>
        public Type[] ColumnTypes { get; set; }

        /// <summary>
        /// 行数据结果集
        /// </summary>
        public IList<object[]> Rows { get; set; }

        /// <summary>
        /// 其他输出参数
        /// </summary>
        public IDictionary<string, object> OutParams { get; set; }
    }
}
