using System;

namespace Frame.DataStore.SqlGeClient.Clauses
{
    /// <summary>
    /// 表示一个基本的SQL子句基类。
    /// </summary>
    public abstract class SqlGeClause
    {
        /// <summary>
        /// SQL执行文本语句子句或存储过程名称。
        /// </summary>
        public string RawText { get; protected set; }

        /// <summary>
        /// 通过提供的相关信息解析装载指定SQL执行文本语句子句或存储过程到SqlGeCommandBuilder对象中。
        /// </summary>
        /// <param name="provider">数据源提供程序对象。</param>
        /// <param name="bulider">SqlGeCommand的生产对象。</param>
        /// <param name="parameters">SQL参数。</param>
        public abstract void ToCommand(IDaoProvider provider, SqlGeCommandBuilder builder, ISqlGeParameters parameters);
    }
}
