using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace Frame.DataStore.Utility
{
    /// <summary>
    /// 提供执行数据库访问的方法。
    /// </summary>
    public class DaoCommand
    {
        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回受影响的行数。
        /// </summary>
        /// <param name="key">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>受影响的行数。</returns>
        public static int ExecuteNonQuery(string key, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.ExecuteNonQuery(executor.Command);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回受影响的行数。
        /// </summary>
        /// <param name="sql">一个SQL语句对象。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>受影响的行数。</returns>
        public static int ExecuteNonQuery(ISqlGeStatement sql, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(sql, parameters);
            return executor.Dao.ExecuteNonQuery(executor.Command);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回一个结果只读器。
        /// </summary>
        /// <param name="key">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果只读器。</returns>
        internal static IDataReader QueryReader(string key, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.QueryReader(executor.Command);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回一个结果只读器。
        /// </summary>
        /// <param name="sql">一个SQL语句对象。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果只读器。</returns>
        public static IDataReader QueryReader(ISqlGeStatement sql, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(sql, parameters);
            return executor.Dao.QueryReader(executor.Command);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回结果集合。
        /// </summary>
        /// <param name="key">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果集合。</returns>
        public static DataSet QueryDataSet(string key, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.QueryDataSet(executor.Command);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回指定泛型类型对象。
        /// </summary>
        /// <typeparam name="T">返回的结果的类型。</typeparam>
        /// <param name="key">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定泛型类型对象。</returns>
        public static T QueryScalar<T>(string key, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.QueryScalar<T>(executor.Command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static DaoExecutor CreateCommand(string key, object parameters)
        {
            ISqlGeStatement sql = DaoFactory.GetSqlSource().Find(key);
            if (null == sql)
            {
                throw new Exception(string.Format("Command操作对象'{0}'未找到。", key));
            }

            return CreateCommand(sql, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static DaoExecutor CreateCommand(ISqlGeStatement sql, object parameters)
        {
            BaseDao dao = string.IsNullOrEmpty(sql.Connection) ? BaseDao.Get() : BaseDao.Get(sql.Connection);

            ISqlGeCommand command = sql.CreateCommand(dao.Provider, parameters);

            return new DaoExecutor() { Command = command, Dao = dao };
        }
    }
}
