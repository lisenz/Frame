using System;

namespace Frame.DataStore.SqlGeClient.Clauses
{
    /// <summary>
    /// 表示一个带有命名参数的SQL子句基类。
    /// </summary>
    public abstract class ParamClause : SqlGeClause
    {
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParamName { get; protected set; }

        /// <summary>
        /// 初始化参数信息。
        /// </summary>
        /// <param name="rawText">SQL语句片段。</param>
        /// <param name="name">参数名称。</param>
        protected ParamClause(string rawText, string name)
        {
            this.RawText = rawText;
            this.ParamName = name;
        }
    }
}
