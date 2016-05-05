using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Collections.Generic;
//-------------
using Frame.Data;

namespace Frame.DataStore
{
    /// <summary>
    /// 公开的可用的数据库访问框架业务类，提供了用于数据库访问的常用方法。
    /// </summary>
    public class DataBaseDao : GenericDao
    {
        /// <summary>
        /// 数据库访问框架的业务对象。
        /// </summary>
        private DataBase _Database;

        /// <summary>
        /// 获取数据库访问框架的业务对象。
        /// </summary>
        public override DataBase Database
        {
            get { return this._Database; }
            protected set { this._Database = value; }
        }

        /// <summary>
        /// 构造函数，初始化默认数据库连接。
        /// </summary>
        public DataBaseDao()
            : this("DefaultDB")
        {
        }

        /// <summary>
        /// 构造函数，通过指定配置节的键初始化映射的数据源提供程序以及数据库访问框架的业务对象。
        /// </summary>
        /// <param name="name">配置节中映射数据库连接的键值。</param>
        public DataBaseDao(string name)
            : base(name)
        {
            this._Database = DaoFactory.GetDatabase(name);
        }

        /// <summary>
        /// 构造函数，通过指定配置节的键初始化映射的数据源提供程序。
        /// </summary>
        /// <param name="name">配置节中映射数据库连接的键值。</param>
        /// <param name="db">数据库访问框架的业务对象。</param>
        public DataBaseDao(string name, DataBase db)
            : base(name)
        {
            this._Database = db;
        }

        /// <summary>
        /// 根据SQL执行文本创建对应的SQL 语句对象。
        /// </summary>
        /// <param name="sql">SQL执行文本。</param>
        /// <returns>一个连接到数据源时执行的 SQL 语句对象。</returns>
        protected override DbCommand CreateDbCommand(string sql)
        {
            return this._Database.GetSqlStringCommand(sql);
        }

        /// <summary>
        /// 根据存储过程的名称及其参数创建对应的执行命令对象。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="parameters">存储过程所需的参数。</param>
        /// <returns>一个连接到数据源时执行的 SQL 语句对象。</returns>
        protected override DbCommand CreateDbCommand(string procName, params object[] parameters)
        {
            return this._Database.GetProcCommand(procName, parameters);
        }

        /// <summary>
        /// 执行数据源的SQL语句或存储过程并返回结果集合。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回一个结果集合。</returns>
        protected override DataSet ExecuteDataSet(DbCommand command)
        {
            return this._Database.ExecuteDataSet(command);
        }

        /// <summary>
        /// 执行数据源的SQL语句或存储过程并返回受影响的行数。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回受影响的行数。</returns>
        protected override int ExecuteNonQuery(DbCommand command)
        {
            return this._Database.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 执行数据源的SQL语句或存储过程并返回一个只进结果集流。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回一个只进结果集流。</returns>
        protected override IDataReader ExecuteReader(DbCommand command)
        {
            return this._Database.ExecuteReader(command);
        }

        /// <summary>
        /// 执行数据源的SQL语句或存储过程并返回结果集中第一行第一列的值。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回结果集中第一行第一列的值。</returns>
        protected override object ExecuteScalar(DbCommand command)
        {
            return this._Database.ExecuteScalar(command);
        }
    }
}
