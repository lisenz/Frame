using System;
using System.Collections.Generic;
//-------------
using Frame.Data;
using System.Data;

namespace Frame.DataStore
{
    /// <summary>
    /// 数据库访问框架的入口抽象类，定义了用于数据库访问的常用方法
    /// </summary>
    public abstract class BaseDao
    {
        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        protected BaseDao()
        {
        }

        #endregion

        #region 静态入口方法

        /// <summary>
        /// 获取绑定了默认数据库连接串的BaseDao对象；
        /// 此方法等同于DaoFactory.GetDao()；
        /// 默认数据库连接串的名称约定为"DefaultDB"，必须在配置文件中进行配置。
        /// </summary>
        /// <returns>BaseDao对象。</returns>
        public static BaseDao Get()
        {
            return DaoFactory.GetDao();
        }

        /// <summary>
        /// 获取绑定了指定名称数据库连接串的BaseDao对象；
        /// 此方法等同于DaoFactory.GetDao(string name)；
        /// </summary>
        /// <param name="name">配置文件中连接串的名称。</param>
        /// <returns>BaseDao对象。</returns>
        public static BaseDao Get(string name)
        {
            return DaoFactory.GetDao(name);
        }

        #endregion

        #region 公开属性

        /// <summary>
        /// 获取绑定相同连接串的数据库对象
        /// </summary>
        public abstract DataBase Database { get; protected set; }

        /// <summary>
        /// 获取当前BaseDao实例的IDaoProvider对象
        /// </summary>
        public abstract IDaoProvider Provider { get; }

        #endregion

        //#region 内置实现方法接口[在GenericDao抽象类中实现]

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回受影响的行数。
        ///// </summary>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>受影响的行数。</returns>
        //internal abstract int ExecuteNonQuery(ISqlGeCommand command);

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回一个新的已加载数据的DataSet对象。
        ///// </summary>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>加载数据的DataSet对象。</returns>
        //internal abstract DataSet QueryDataSet(ISqlGeCommand command);

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回一个只进结果集流。
        ///// </summary>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>一个只进结果集流。</returns>
        //internal abstract IDataReader QueryReader(ISqlGeCommand command);

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回一个泛型参数类型指定类型的结果对象。
        ///// </summary>
        ///// <typeparam name="T">返回的结果对象的类型。</typeparam>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>一个泛型参数类型指定类型的结果对象。</returns>
        //internal abstract T QueryScalar<T>(ISqlGeCommand command);

        //#endregion

        #region 存储过程实现方法

        public abstract int DoNonQuery(string procName, object parameters = null);

        public abstract DataSet DoQueryDataSet(string procName, object parameters = null);

        public abstract T DoQueryScalar<T>(string procName, object parameters = null);

        public abstract IDataReader DoQueryReader(string procName, object parameters = null);

        public abstract IList<IDictionary<string, object>> DoQueryDictionaries(string procName, object parameters = null);

        public abstract IList<T> DoQueryScalarList<T>(string procName, object parameters = null);

        public abstract IList<T> DoQueryEntities<T>(string procName, object parameters = null) where T : class,new();

        public abstract IList<T> DoQueryEntities<T>(Type type, string procName, object parameters = null) where T : class;

        public abstract IDictionary<string, object> DoQueryDictionary(string procName, object parameters = null);

        public abstract T DoQueryEntity<T>(string procName, object parameters = null) where T : class,new();

        public abstract T DoQueryEntity<T>(Type type, string procName, object parameters = null) where T : class;

        #endregion

        #region 基本实现方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回受影响的行数。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>受影响的行数。</returns>
        public abstract int ExecuteNonQuery(string sql, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果集合。</returns>
        public abstract DataSet QueryDataSet(string sql, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回一个结果只读器。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果只读器。</returns>
        public abstract IDataReader QueryReader(string sql, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回指定泛型类型对象。
        /// </summary>
        /// <typeparam name="T">返回的结果的类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定泛型类型对象。</returns>
        public abstract T QueryScalar<T>(string sql, object parameters = null);
        
        #endregion
        
        #region 分页查询方法接口

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果集合。</returns>
        public abstract DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="totalRowCount">实际结果集的行数。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果集合。</returns>
        public abstract DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的IDictionary键/值对类型对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果对象集合。</returns>
        public abstract IList<IDictionary<string, object>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的IDictionary键/值对类型对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="totalRowCount">实际结果集的行数。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果对象集合。</returns>
        public abstract IList<IDictionary<string, object>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">返回的对象集合的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>结果对象集合。</returns>
        public abstract IList<T> PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null) where T : class,new();

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">返回的对象集合的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="totalRowCount">实际结果集的行数。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>结果对象集合。</returns>
        public abstract IList<T> PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null) where T : class,new();

        #endregion

        #region 查询多行数据方法接口

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回IDictionary键/值对类型的对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>IDictionary键/值对类型的对象结果集合。</returns>
        public abstract IList<IDictionary<string, object>> QueryDictionaries(string sql, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的数据结果集合。
        /// </summary>
        /// <typeparam name="T">指定返回的数据结果类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的数据结果集合。</returns>
        public abstract IList<T> QueryScalarList<T>(string sql, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的对象集合。</returns>
        public abstract IList<T> QueryEntities<T>(string sql, object parameters = null) where T : class,new();

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="type">与指定返回的对象类型一致的映射类型。</param>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的对象集合。</returns>
        public abstract IList<T> QueryEntities<T>(Type type, string sql, object parameters = null) where T : class;

        #endregion

        #region 查询单行单列数据方法接口

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行Dictionary键/值对类型的对象结果。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>Dictionary键/值对类型的对象</returns>
        public abstract IDictionary<string, object> QueryDictionary(string sql, object parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行指定类型的对象结果。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>单行指定类型的对象结果。</returns>
        public abstract T QueryEntity<T>(string sql, object parameters = null) where T : class,new();

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行指定类型的对象结果。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="type">与指定返回的对象类型一致的映射类型。</param>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>单行指定类型的对象结果。</returns>
        public abstract T QueryEntity<T>(Type type, string sql, object parameters = null) where T : class;

        #endregion

    }
}
