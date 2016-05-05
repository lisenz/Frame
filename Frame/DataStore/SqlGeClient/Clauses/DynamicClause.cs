using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.DataStore.SqlGeClient.Clauses
{
    /// <summary>
    /// 表示一个数据库参数类型的SQL子句，格式为{? .. #ParamName# ..} 或者 {? .. $ParamName$ ..}。
    /// </summary>
    internal class DynamicClause : SqlGeClause
    {

        private IList<SqlGeClause> _childs;
        private ParamClause _paramClause;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawText"></param>
        /// <param name="content"></param>
        public DynamicClause(string rawText, string content)
        {
            RawText = rawText;
            Content = content;
            _childs = ParseChilds();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="builder"></param>
        /// <param name="parameters"></param>
        public override void ToCommand(IDaoProvider provider, SqlGeCommandBuilder builder, ISqlGeParameters parameters)
        {
            if (_paramClause != null)
            {
                object value = parameters.Resolve(_paramClause.ParamName);

                if (null == value || (value is string && string.IsNullOrEmpty(value as string)))
                {
                    return;
                }
            }

            foreach (SqlGeClause clause in _childs)
            {
                clause.ToCommand(provider, builder, parameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IList<SqlGeClause> ParseChilds()
        {
            /*
             * 一个动态子句的形式为：{? .. #ParamName# ..} 或者 {? .. $ParamName$ ..}.
             * 
             * 必须出现一个而且只能是一个参数子句，可以是命名参数或者值替换参数
             */

            IList<SqlGeClause> clauses = SqlGeParser.ParseToClauses(Content);

            if (!FindSingleParameterClause(clauses))
            {
                //如果不符合格式要求，则把原始内容直接输出
                clauses.Clear();
                clauses.Add(new TextClause(RawText));
            }

            return clauses;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clauses"></param>
        /// <returns></returns>
        private bool FindSingleParameterClause(IList<SqlGeClause> clauses)
        {
            ParamClause paramClause = null;

            int count = 0;
            foreach (SqlGeClause clause in clauses)
            {
                if (clause is ParamClause)
                {
                    count++;
                    paramClause = clause as ParamClause;
                }

                if (count > 1)
                {
                    break;
                }
            }

            if (count == 1)
            {
                _paramClause = paramClause;
            }
            return count == 1;
        }
    }
}
