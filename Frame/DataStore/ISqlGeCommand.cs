using System;
using System.Collections.Generic;

namespace Frame.DataStore
{
    /// <summary>
    /// 表示连接到数据源时执行的 SQL 语句。
    /// </summary>
    public interface ISqlGeCommand
    {
        /// <summary>
        /// 获取或设置针对数据源运行的文本命令。
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// SQL 语句或存储过程的参数。
        /// </summary>
        IList<KeyValuePair<string, object>> Parameters { get; }
    }
}
