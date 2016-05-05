using System;
using System.Text;
using System.Collections.Generic;

namespace Frame.DataStore.SqlGeClient
{
    /// <summary>
    /// 提供创建操作SqlGeCommand对象的方法。
    /// </summary>
    public class SqlGeCommandBuilder
    {
        private readonly StringBuilder _Sql = null;
        private readonly IList<KeyValuePair<string, object>> _Parameters;

        /// <summary>
        /// 获取相关联的数据源提供程序。
        /// </summary>
        public IDaoProvider Provider { get; private set; }

        public SqlGeCommandBuilder()
        {
            this.Provider = DataStore.Provider.DaoProvider.Default;
            this._Sql = new StringBuilder();
            this._Parameters = new List<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider">数据源访问提供程序对象。</param>
        internal SqlGeCommandBuilder(IDaoProvider provider)
        {
            this.Provider = provider;
            this._Sql = new StringBuilder();
            this._Parameters = new List<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// 将解析过的子句字符串添加到内置的执行文本命令对象中。
        /// </summary>
        /// <param name="sql">经过解析的子句字符串。</param>
        public void AppendCommandText(string sql)
        {
            this._Sql.Append(sql);
        }

        /// <summary>
        /// 添加执行SQL文本命令所需的参数数据。
        /// </summary>
        /// <param name="name">参数集合的键。</param>
        /// <param name="value">对应键的值。</param>
        public void AddCommandParameter(string name, object value)
        {
            this._Parameters.Add(new KeyValuePair<string, object>(name, value));
        }

        /// <summary>
        /// 创建一个指定的SqlGeCommand。
        /// </summary>
        /// <returns>返回一个SqlGeCommand对象。</returns>
        public SqlGeCommand ToCommand()
        {
            return new SqlGeCommand(this.Provider, this._Sql.ToString(), this._Parameters);
        }

    }
}
