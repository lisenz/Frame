using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.DataStore.SqlGeClient.Clauses
{
    /// <summary>
    /// 表示一个数据库参数类型的SQL子句，格式为#ParamName#
    /// </summary>
    internal class NamedParameterClause : ParamClause
    {
        /// <summary>
        /// 构造函数，初始化子句结构信息。
        /// </summary>
        /// <param name="rawText">通过匹配捕获的实际子字符串。</param>
        /// <param name="name">通过匹配捕获的子字符串。</param>
        public NamedParameterClause(string rawText, string name)
            : base(rawText, name)
        {

        }

        /// <summary>
        /// 解析子句并生成指定的命令。
        /// </summary>
        /// <param name="provider">数据源提供程序。</param>
        /// <param name="builder">SqlGeCommand建造器。</param>
        /// <param name="parameters">参数对象。</param>
        public override void ToCommand(IDaoProvider provider, SqlGeCommandBuilder builder, ISqlGeParameters parameters)
        {
            string sqlParamName = provider.EscapeParam(ParamName);
            builder.AppendCommandText(string.Format(builder.Provider.NamedParameterFormat, sqlParamName));
            builder.AddCommandParameter(sqlParamName, parameters.Resolve(ParamName));
        }
    }
}
