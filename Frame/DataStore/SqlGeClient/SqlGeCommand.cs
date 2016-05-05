using System;
using System.Text;
using System.Collections.Generic;

namespace Frame.DataStore.SqlGeClient
{
    /// <summary>
    /// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个操作类。
    /// </summary>
    public class SqlGeCommand : ISqlGeCommand
    {
        private IDaoProvider _Provider;
        private string _CommandText;
        private IList<KeyValuePair<string, object>> _Parameters;

        /// <summary>
        /// 表示要对数据源执行的 SQL 语句或存储过程。
        /// </summary>
        protected SqlGeCommand()
        {
        }

        /// <summary>
        /// 初始化SqlGeCommand。
        /// </summary>
        /// <param name="commandText">针对数据源运行的文本命令。</param>
        public SqlGeCommand(string commandText)
            : this(commandText, new List<KeyValuePair<string, object>>())
        {
        }

        /// <summary>
        /// 初始化SqlGeCommand。
        /// </summary>
        /// <param name="commandText">针对数据源运行的文本命令。</param>
        /// <param name="parameters">SQL语句或存储过程的参数集合。</param>
        public SqlGeCommand(string commandText, IList<KeyValuePair<string, object>> parameters)
            : this(DataStore.Provider.DaoProvider.Default, commandText, parameters)
        {
        }

        /// <summary>
        /// 初始化SqlGeCommand。
        /// </summary>
        /// <param name="provider">数据访问提供者对象。</param>
        /// <param name="commandText">针对数据源运行的文本命令。</param>
        /// <param name="parameters">SQL语句或存储过程的参数集合。</param>
        public SqlGeCommand(IDaoProvider provider, string commandText, IList<KeyValuePair<string, object>> parameters)
        {
            this._Provider = provider;
            this._CommandText = commandText;
            this._Parameters = parameters;
        }

        /// <summary>
        /// 获取或设置SQL语句或存储过程的参数集合。
        /// </summary>
        public IList<KeyValuePair<string, object>> Parameters
        {
            get { return this._Parameters; }
            protected set { this._Parameters = value; }
        }

        /// <summary>
        /// 获取或设置针对数据源运行的文本命令。
        /// </summary>
        public string CommandText
        {
            get { return _CommandText; }
            protected set { _CommandText = value; }
        }

        /// <summary>
        /// 获取或设置对应的数据访问提供者对象。
        /// </summary>
        public IDaoProvider Provider
        {
            get { return _Provider; }
            protected set { _Provider = value; }
        }
    }
}
