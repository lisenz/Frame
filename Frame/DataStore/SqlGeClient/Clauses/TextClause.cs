using System;

namespace Frame.DataStore.SqlGeClient.Clauses
{
    /// <summary>
    /// 表示不带有命名参数的SQL文本语句或存储过程。
    /// </summary>
    internal class TextClause : SqlGeClause
    {
        /// <summary>
        /// 初始化对象。
        /// </summary>
        /// <param name="rawText">SQL文本语句。</param>
        public TextClause(string rawText)
        {
            RawText = rawText;
        }

        /// <summary>
        /// 通过提供的相关信息解析装载指定SQL执行文本语句子句或存储过程名称到SqlGeCommandBuilder对象中。
        /// </summary>
        /// <param name="provider">数据源提供程序对象。</param>
        /// <param name="builder">SqlGeCommand的生产对象。</param>
        /// <param name="parameters">SQL参数。</param>
        public override void ToCommand(IDaoProvider provider, SqlGeCommandBuilder builder, ISqlGeParameters parameters)
        {
            builder.AppendCommandText(RawText);
        }
    }
}
