using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
//---------------
using Frame.Core;
using Frame.Core.Extensions;
using Frame.DataStore.Extensions;
using Frame.DataStore.SqlGeClient;
using Frame.DataStore.SqlGeClient.Parameters;

namespace Frame.DataStore
{
    /// <summary>
    /// 实现数据库访问框架基类的一个常规基类，提供了用于数据库访问的常规方法。
    /// </summary>
    public abstract class GenericDao : BaseDao
    {
        /// <summary>
        /// 
        /// </summary>
        protected string _Name;

        /// <summary>
        /// 
        /// </summary>
        protected IDaoProvider _Provider;
        
        /// <summary>
        /// 获取指定的数据源提供程序对象。
        /// </summary>
        public override IDaoProvider Provider
        {
            get { return _Provider; }
        }

        protected GenericDao(string name)
        {
            this._Name = name;
            this._Provider = DaoFactory.GetProvider(App.ConnectionStrings[name].ProviderName);

            if (null == _Provider)
            {
                throw new Exception(string.Format("没有找到对应的DaoProvider ：'{0}'。", App.ConnectionStrings[name].ProviderName));
            }
        }

        #region 内置实现接口方法[提供给DataBaseDao类进行重写]
        
        /// <summary>
        /// 根据SQL执行文本创建对应的SQL 语句对象。
        /// </summary>
        /// <param name="sql">SQL执行文本。</param>
        /// <returns>一个连接到数据源时执行的 SQL 语句对象。</returns>
        protected abstract DbCommand CreateDbCommand(string sql);

        /// <summary>
        /// 根据存储过程创建对应的执行命令对象。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="parameters">存储过程的参数。</param>
        /// <returns>一个连接到数据源时执行的执行命令对象。</returns>
        protected abstract DbCommand CreateDbCommand(string procName, params object[] parameters);

        /// <summary>
        /// 执行数据源的SQL语句并返回结果集合。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回一个结果集合。</returns>
        protected abstract DataSet ExecuteDataSet(DbCommand command);

        /// <summary>
        /// 执行数据源的SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回受影响的行数。</returns>
        protected abstract int ExecuteNonQuery(DbCommand command);

        /// <summary>
        /// 执行数据源的SQL语句并返回一个只进结果集流。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回一个只进结果集流。</returns>
        protected abstract IDataReader ExecuteReader(DbCommand command);

        /// <summary>
        /// 执行数据源的SQL语句并返回结果集中第一行第一列的值。
        /// </summary>
        /// <param name="command">一个对数据源执行的SQL语句对象。</param>
        /// <returns>返回结果集中第一行第一列的值。</returns>
        protected abstract object ExecuteScalar(DbCommand command);

        #endregion

        //#region 重写BaseDao基类--内置实现方法接口的方法

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回受影响的行数。
        ///// </summary>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>受影响的行数。</returns>
        //internal override int ExecuteNonQuery(ISqlGeCommand command)
        //{
        //    return ExecuteNonQuery(CreateDbCommand(command));
        //}

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回一个新的已加载数据的DataSet对象。
        ///// </summary>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>加载数据的DataSet对象。</returns>
        //internal override DataSet QueryDataSet(ISqlGeCommand command)
        //{
        //    return ExecuteDataSet(CreateDbCommand(command));
        //}

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回一个只进结果集流。
        ///// </summary>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>一个只进结果集流。</returns>
        //internal override IDataReader QueryReader(ISqlGeCommand command)
        //{
        //    return ExecuteReader(CreateDbCommand(command));
        //}

        ///// <summary>
        ///// 执行一个连接到数据源时执行的SQL语句对象，并返回一个泛型参数类型指定类型的结果对象。
        ///// </summary>
        ///// <typeparam name="T">返回的结果对象的类型。</typeparam>
        ///// <param name="command">一个连接到数据源时执行的SQL语句对象。</param>
        ///// <returns>一个泛型参数类型指定类型的结果对象。</returns>
        //internal override T QueryScalar<T>(ISqlGeCommand command)
        //{
        //    return Convertor.Convert<T>(ExecuteScalar(CreateDbCommand(command)));
        //}

        //#endregion

        #region 重写BaseDao基类--存储过程实现方法

        public override int DoNonQuery(string procName, object parameters = null)
        {
            return DoExecute<int>(procName, parameters, ExecuteNonQuery);
        }

        public override IDataReader DoQueryReader(string procName, object parameters = null)
        {
            return DoExecute<IDataReader>(procName, parameters, ExecuteReader);
        }

        public override DataSet DoQueryDataSet(string procName, object parameters = null)
        {
            return DoExecute<DataSet>(procName, parameters, ExecuteDataSet);
        }

        public override IList<IDictionary<string, object>> DoQueryDictionaries(string procName, object parameters = null)
        {
            return DoExecuteReader<IList<IDictionary<string, object>>>(procName, parameters, (reader) => reader.ReadDataToDictionarys());
        }

        public override IDictionary<string, object> DoQueryDictionary(string procName, object parameters = null)
        {
            return DoExecuteReader<IDictionary<string, object>>(procName, procName, (reader) => reader.ReadDataToDictionary());
        }

        public override T DoQueryScalar<T>(string procName, object parameters = null)
        {
            return Convertor.Convert<T>(DoExecute<object>(procName, parameters, ExecuteScalar));
        }

        public override IList<T> DoQueryScalarList<T>(string procName, object parameters = null)
        {
            return DoExecuteReader<IList<T>>(procName, parameters, (reader) =>
            {
                List<T> list = new List<T>();
                while (reader.Read())
                {
                    list.Add(Convertor.Convert<T>(reader[0]));
                }

                return list;
            });
        }

        public override T DoQueryEntity<T>(string procName, object parameters = null)
        {
            return DoQueryEntity<T>(typeof(T), procName, parameters);
        }

        public override T DoQueryEntity<T>(Type type, string procName, object parameters = null)
        {
            return DoExecuteReader<T>(procName, parameters, (reader) =>
            {
                T entity = reader.Read<T>(type);
                if (reader.Read())
                {
                    throw new Exception("reader只读器具有多行数据行!");
                }

                return entity;
            });
        }

        public override IList<T> DoQueryEntities<T>(string procName, object parameters = null)
        {
            return DoQueryEntities<T>(typeof(T), procName, parameters);
        }

        public override IList<T> DoQueryEntities<T>(Type type, string procName, object parameters = null)
        {
            return DoExecuteReader<IList<T>>(procName, parameters, (reader) => reader.ReadList<T>(type));
        }

        #endregion

        #region 重写BaseDao基类--执行文本(执行SQL文本源)实现方法

        #region 基本实现方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回受影响的行数。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>受影响的行数。</returns>
        public override int ExecuteNonQuery(string sql, object parameters = null)
        {
            return Execute<int>(sql, parameters, ExecuteNonQuery);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回一个结果只读器。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果只读器。</returns>
        public override IDataReader QueryReader(string sql, object parameters = null)
        {
            return Execute<IDataReader>(sql, parameters, ExecuteReader);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果集合。</returns>
        public override DataSet QueryDataSet(string sql, object parameters = null)
        {
            return Execute<DataSet>(sql, parameters, ExecuteDataSet);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回指定泛型类型对象。
        /// </summary>
        /// <typeparam name="T">返回的结果的类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定泛型类型对象。</returns>
        public override T QueryScalar<T>(string sql, object parameters = null)
        {
            return Convertor.Convert<T>(Execute<object>(sql, parameters, ExecuteScalar));
        }

        #endregion
        
        #region 分页查询方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果集合。</returns>
        public override DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null)
        {
            if (maximumRows <= 0)
            {
                ISqlGeStatement statement;
                string text = FindText(sql, out statement);
                string newText = _Provider.WrapCountSql(text);
                int totalRowCount = QueryScalar<int>(newText, parameters);
                maximumRows = totalRowCount;
            }

            object outParams;
            string sqlText = GetPageQuery(sql, startRowIndex, maximumRows, sortExpression, parameters, out outParams);

            return QueryDataSet(sqlText, outParams);
        }

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
        public override DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null)
        {
            ISqlGeStatement statement;
            string text = FindText(sql, out statement);
            string newText = _Provider.WrapCountSql(text);
            totalRowCount = QueryScalar<int>(newText, parameters);
            return PageQueryDataSet(text, startRowIndex, maximumRows <= 0 ? totalRowCount : maximumRows, sortExpression, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的IDictionary键/值对类型对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果对象集合。</returns>
        public override IList<IDictionary<string, object>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null)
        {
            if (maximumRows <= 0)
            {
                ISqlGeStatement statement;
                string text = FindText(sql, out statement);
                string newText = _Provider.WrapCountSql(text);
                int totalRowCount = QueryScalar<int>(newText, parameters);
                maximumRows = totalRowCount;
            }

            object outParams;
            string sqlText = GetPageQuery(sql, startRowIndex, maximumRows, sortExpression, parameters, out outParams);

            return QueryDictionaries(sqlText, outParams);
        }

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
        public override IList<IDictionary<string, object>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null)
        {
            ISqlGeStatement statement;
            string text = FindText(sql, out statement);
            string newText = _Provider.WrapCountSql(text);
            totalRowCount = QueryScalar<int>(newText, parameters);
            return PageQueryDictionaries(text, startRowIndex, maximumRows <= 0 ? totalRowCount : maximumRows, sortExpression, parameters);
        }

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
        public override IList<T> PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null)
        {
            if (maximumRows <= 0)
            {
                ISqlGeStatement statement;
                string text = FindText(sql, out statement);
                string newText = _Provider.WrapCountSql(text);
                int totalRowCount = QueryScalar<int>(newText, parameters);
                maximumRows = totalRowCount;
            }

            object outParams;
            string sqlText = GetPageQuery(sql, startRowIndex, maximumRows, sortExpression, parameters, out outParams);

            return QueryEntities<T>(sqlText, outParams);
        }

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
        public override IList<T> PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null)
        {
            ISqlGeStatement statement;
            string text = FindText(sql, out statement);
            string newText = _Provider.WrapCountSql(text);
            totalRowCount = QueryScalar<int>(newText, parameters);
            return PageQueryEntities<T>(text, startRowIndex, maximumRows <= 0 ? totalRowCount : maximumRows, sortExpression, parameters);
        }

        /// <summary>
        /// 获取包装的分页查询的语句
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="inParmas">参数集合。</param>
        /// <param name="outParams">进行输出的参数集合。</param>
        /// <returns>返回包装后的新的分页查询语句。</returns>
        protected virtual string GetPageQuery(string sql, int startRowIndex, int maximumRows, String sortExpression, object inParmas, out object outParams)
        {
            ISqlGeStatement statement;
            string text = FindText(sql, out statement);
            ParametersWrapper param = new ParametersWrapper(SqlGeParameters.GetParameters(inParmas));

            string newText = _Provider.WrapPageSql(text, sortExpression);
            param.Add("__RowBegin__", startRowIndex);
            param.Add("__RowEnd__", this._Provider.ProviderName.Equals("SqlServer") ? startRowIndex + maximumRows - 1 : maximumRows);

            outParams = param;
            return newText;
        }

        #endregion

        #region 查询多行数据方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回IDictionary键/值对类型的对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>IDictionary键/值对类型的对象结果集合。</returns>
        public override IList<IDictionary<string, object>> QueryDictionaries(string sql, object parameters = null)
        {
            return ExecuteReader<IList<IDictionary<string, object>>>(sql, parameters, (reader) => reader.ReadDataToDictionarys());
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的数据结果集合。
        /// </summary>
        /// <typeparam name="T">指定返回的数据结果类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的数据结果集合。</returns>
        public override IList<T> QueryScalarList<T>(string sql, object parameters = null)
        {
            return ExecuteReader<IList<T>>(sql, parameters, (reader) =>
            {
                IList<T> list = new List<T>();
                while (reader.Read())
                {
                    list.Add(Convertor.Convert<T>(reader[0]));
                }
                return list;
            });
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的对象集合。</returns>
        public override IList<T> QueryEntities<T>(string sql, object parameters = null)
        {
            return ExecuteReader<IList<T>>(sql, parameters, (reader) => reader.ReadList<T>());
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="type">与指定返回的对象类型一致的映射类型。</param>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的对象集合。</returns>
        public override IList<T> QueryEntities<T>(Type type, string sql, object parameters = null)
        {
            return ExecuteReader<IList<T>>(sql, parameters, (reader) => reader.ReadList<T>(type));
        }

        #endregion

        #region 查询单行单列数据方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行Dictionary键/值对类型的对象结果。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>Dictionary键/值对类型的对象</returns>
        public override IDictionary<string, object> QueryDictionary(string sql, object parameters = null)
        {
            return ExecuteReader<IDictionary<string, object>>(sql, parameters,
                                                              (reader) =>
                                                              {
                                                                  var dict = reader.ReadDataToDictionary();
                                                                  if (reader.Read())
                                                                  {
                                                                      throw new Exception("reader只读器具有多行数据行!");
                                                                  }
                                                                  return dict;
                                                              });
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行指定类型的对象结果。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>单行指定类型的对象结果。</returns>
        public override T QueryEntity<T>(string sql, object parameters = null)
        {
            return QueryEntity<T>(typeof(T), sql, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行指定类型的对象结果。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="type">与指定返回的对象类型一致的映射类型。</param>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>单行指定类型的对象结果。</returns>
        public override T QueryEntity<T>(Type type, string sql, object parameters = null)
        {
            return ExecuteReader<T>(sql, parameters, (reader) =>
            {
                T entity = reader.Read<T>(type);
                if (reader.Read())
                {
                    throw new Exception("reader只读器具有多行数据行!");
                }

                return entity;
            });
        }

        #endregion

        #endregion

        #region 内置实现逻辑基本方法

        protected virtual T DoExecute<T>(string procName, object parameters, Func<DbCommand, T> func)
        {
            IDictionary<string, object> param = Convertor.Convert<IDictionary<string, object>>(parameters);

            T TValue = func(this.CreateDbCommand(procName, null != param ? param.Values.ToArray() : new object[]{}));
            return TValue;
        }

        /// <summary>
        /// 通过指定的SQL语句以及语句参数执行一个DbCommand对象，并返回泛型参数类型指定类型的结果对象。
        /// </summary>
        /// <typeparam name="T">返回的结果对象的类型。</typeparam>
        /// <param name="sql">执行的SQL语句。</param>
        /// <param name="parameters">执行的SQL语句的参数。</param>
        /// <param name="func">一个具有一个DbCommand类型参数并返回泛型参数类型指定类型结果对象的委托对象。</param>
        /// <returns>返回泛型参数类型指定类型的结果对象。</returns>
        protected virtual T Execute<T>(string sql, object parameters, Func<DbCommand, T> func)
        {
            ISqlGeStatement statement;
            this.CheckArguments(sql, parameters);
            bool isKey = FindStatement(sql = sql.Trim(), out statement);
            if (!isKey)
                statement = SqlGeClient.SqlGeParser.Parse(sql);
            if (!string.IsNullOrEmpty(statement.Connection))
                this.Database = DaoFactory.GetDatabase(statement.Connection);
            T TValue = func(this.CreateDbCommand(statement, parameters));
            return TValue;
        }

        /// <summary>
        /// 检查验证SQL文本命令和对应参数。
        /// </summary>
        /// <param name="sql">SQL文本命令。</param>
        /// <param name="parameters">文本命令执行所需的对应的参数。</param>
        protected virtual void CheckArguments(string sql, object parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(sql);
            }
            if (null != parameters)
            {
                Type type = parameters.GetType();
                if (type.IsPrimitive || type.IsValueType)
                {
                    throw new InvalidOperationException(string.Format("参数'object parameters'不支持类型'{0}'", type.FullName));
                }
            }
        }

        /// <summary>
        /// 创建一个连接到数据源时执行的 SQL 语句对象。
        /// </summary>
        /// <param name="command">一个最终被数据库访问框架执行的数据库SQL命名对象。</param>
        /// <returns>连接到数据源时执行的 SQL 语句对象。</returns>
        protected virtual DbCommand CreateDbCommand(ISqlGeCommand command)
        {
            DbCommand dbCommand = this.CreateDbCommand(command.CommandText);
            foreach (KeyValuePair<string, object> parameter in command.Parameters)
            {
                if (this._Provider.IsSupportsNamedParameter && dbCommand.Parameters.Contains(parameter.Key))
                    continue;
                else
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.ParameterName = parameter.Key;
                    object value = parameter.Value;
                    dbParameter.Value = value ?? DBNull.Value;
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            return dbCommand;
        }

        /// <summary>
        /// 通过一个SQL语句声明对象以及对应参数创建一个连接到数据源时执行的 SQL 语句对象。
        /// </summary>
        /// <param name="statement">一个SQL语句声明对象。</param>
        /// <param name="parameters">执行SQL命令的参数。</param>
        /// <returns>一个连接到数据源时执行的 SQL 语句对象。</returns>
        protected virtual DbCommand CreateDbCommand(ISqlGeStatement statement, object parameters)
        {
            return this.CreateDbCommand(statement.CreateCommand(parameters));
        }

        /// <summary>
        /// 查找SQL语句源中指定SQL执行文本对应生成的SQL语句声明对象。
        /// TODO：该方法主用于通用Execute泛型方法。
        /// </summary>
        /// <param name="sql">SQL执行文本命令。</param>
        /// <param name="statement">对应生成的SQL语句声明对象。</param>
        /// <returns>返回一个值，该值标识是否有查找到对应的SQL语句声明对象。</returns>
        protected virtual bool FindStatement(string sql, out ISqlGeStatement statement)
        {
            if (!DaoFactory.GetSqlSource().IsValidKey(sql))
            {
                statement = null;
                return false;
            }
            else
            {
                return null != (statement = DaoFactory.GetSqlSource().Find(sql));
            }
        }

        /// <summary>
        /// 查找获取SQL执行文本命令。
        /// TODO：该方法主用于分页查询。
        /// </summary>
        /// <param name="sql">查找对应SQL执行文本命令的源命令。</param>
        /// <param name="statement">一个SQL语句声明对象。</param>
        /// <returns>SQL执行文本命令。若语句源中有对应SQL语句声明对象，则返回该对象解析后的SQL执行文本命令，否则返回进行查找的原文本命令。</returns>
        protected virtual string FindText(string sql, out ISqlGeStatement statement)
        {
            return FindStatement(sql, out statement) ? statement.Text : sql;
        }

        /// <summary>
        /// 通过提供的parameters，执行针对 System.Data.IDbCommand.Connection 的sql文本命令，并生成System.Data.IDataReader。
        /// </summary>
        /// <typeparam name="T">结果对象的类型。</typeparam>
        /// <param name="sql">一个SQL执行文本命令。</param>
        /// <param name="parameters">执行文本命令的参数。</param>
        /// <param name="func">一个具有IDataReader类型的参数并返回泛型参数类型指定类型的结果对象的委托对象。</param>
        /// <returns>通过在数据源执行命令获取泛型参数指定类型的结果对象。</returns>
        public virtual T ExecuteReader<T>(string sql, object parameters, Func<IDataReader, T> func)
        {
            using (IDataReader reader = QueryReader(sql, parameters))
            {
                return func(reader);
            }
        }

        /// <summary>
        /// 通过提供的parameters，执行针对 System.Data.IDbCommand.Connection 的sql文本命令，并生成System.Data.IDataReader。
        /// </summary>
        /// <typeparam name="T">结果对象的类型。</typeparam>
        /// <param name="sql">一个SQL执行文本命令。</param>
        /// <param name="parameters">执行文本命令的参数。</param>
        /// <param name="func">一个具有IDataReader类型的参数并返回泛型参数类型指定类型的结果对象的委托对象。</param>
        /// <returns>通过在数据源执行命令获取泛型参数指定类型的结果对象。</returns>
        public virtual T DoExecuteReader<T>(string procName, object parameters, Func<IDataReader, T> func)
        {
            using (IDataReader reader = DoQueryReader(procName, parameters))
            {
                return func(reader);
            }
        }

        #endregion

    }
}
