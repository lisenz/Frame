using System;
using System.Collections.Generic;
//-------------------
using Frame.DataStore.Provider;
using Frame.DataStore.SqlGeClient.Clauses;
using Frame.DataStore.SqlGeClient.Parameters;

namespace Frame.DataStore.SqlGeClient
{
    /// <summary>
    /// 表示一个SQL语句，为表示命令的类提供一切属性与方法。
    /// </summary>
    public class SqlGeStatement : ISqlGeStatement
    {
        /// <summary>
        /// 表示一个SQL文本语句。
        /// </summary>
        private readonly string _Text;

        /// <summary>
        /// 表示一个SQL文本语句或存储过程的子句。
        /// </summary>
        private readonly IList<SqlGeClause> _Clauses;

        /// <summary>
        /// 标识一个值，该值表示该SQL语句是否为查询语句。
        /// </summary>
        private bool? _IsQuery;

        /// <summary>
        /// 构造函数，初始化SQL语句声明对象需要的信息。
        /// </summary>
        /// <param name="text">SQL执行文本命令。</param>
        /// <param name="clauses">经过解析的SQL子句集合列表。</param>
        public SqlGeStatement(string text, IList<SqlGeClause> clauses)
        {
            this._Text = text;
            this._Clauses = clauses;
        }

        #region 属性

        /// <summary>
        /// 标识一个值，该值表示该SQL语句是否为查询语句。
        /// </summary>
        public bool IsQuery
        {
            get { return _IsQuery ?? IsQueryText(); }
            internal set { _IsQuery = value; }
        }

        /// <summary>
        /// 获取SQL文本语句。
        /// </summary>
        public string Text
        {
            get { return _Text; }
        }

        /// <summary>
        /// 获取或设置数据连接。
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 获取该SQL文本语句的参数部分或存储过程的名称。
        /// </summary>
        public IList<SqlGeClause> Clauses
        {
            get
            {
                return _Clauses;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 创建一个SqlGeCommand操作对象。
        /// </summary>
        /// <param name="parameters">SQL 语句或存储过程的参数。</param>
        /// <returns>SqlGeCommand操作对象。</returns>
        public ISqlGeCommand CreateCommand(object parameters)
        {
            return CreateCommand(DaoProvider.Default, parameters);
        }

        /// <summary>
        /// 创建一个SqlGeCommand操作对象。
        /// </summary>
        /// <param name="provider">数据访问提供者对象。</param>
        /// <param name="parameters">SQL 语句或存储过程的参数。</param>
        /// <returns>SqlGeCommand操作对象。</returns>
        public ISqlGeCommand CreateCommand(IDaoProvider provider, object parameters)
        {
            SqlGeCommandBuilder builder = new SqlGeCommandBuilder(provider);
            SqlGeParameters sqlparameters = new SqlGeParameters(parameters);
            foreach (SqlGeClause clause in _Clauses)
            {
                clause.ToCommand(provider, builder, sqlparameters);
            }

            return builder.ToCommand();
        }

        /// <summary>
        /// 标识该SQL文本是否为查询语句。
        /// </summary>
        /// <returns>如果该SQL文本是查询语句，则返回true；否则返回false。</returns>
        private bool IsQueryText()
        {
            if (!string.IsNullOrEmpty(_Text))
            {
                _IsQuery = true;
                return _Text.Trim().ToLower().StartsWith("select ");
            }
            return false;
        }

        #endregion
    }
}
