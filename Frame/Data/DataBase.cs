using System;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Frame.Data
{
    /// <summary>
    /// 数据访问的抽象基类，提供访问数据库的统一方法。
    /// </summary>
    public abstract class DataBase
    {
        /// <summary>
        /// 存储过程的参数缓存管理对象。
        /// </summary>
        private static readonly ParameterCache _ParameterCache = new ParameterCache();

        /// <summary>
        /// 数据库连接字符串。
        /// </summary>
        private readonly string _ConnectionString;

        /// <summary>
        /// 提供创建提供程序对数据源类的实现的工厂实例。
        /// </summary>
        private readonly DbProviderFactory _DbProviderFactory;


        /// <summary>
        /// 获取连接字符串。
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this._ConnectionString;
            }
        }

        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的工厂实例。
        /// </summary>
        public DbProviderFactory ProviderFactory
        {
            get
            {
                return this._DbProviderFactory;
            }
        }

        /// <summary>
        /// 构造指定数据库连接字符串和提供程序对数据源类的实现的工厂实例的数据库对象。
        /// </summary>
        /// <param name="connectionString">指定数据库连接字符串。</param>
        /// <param name="provider">提供程序对数据源类的实现的工厂实例。</param>
        protected DataBase(string connectionString, DbProviderFactory provider)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("connectionString");
            if (provider == null) throw new ArgumentNullException("dbProviderFactory");

            this._ConnectionString = connectionString;
            this._DbProviderFactory = provider;
        }

        #region 方法

        #region 基础方法

        /// <summary>
        /// 清空存储过程参数缓存。
        /// </summary>
        public static void ClearParameterCache()
        {
            _ParameterCache.Clear();
        }

        /// <summary>
        /// 根据执行命令类型创建对应的DbCommand执行命令对象。
        /// </summary>
        /// <param name="commandType">执行命令类型。</param>
        /// <param name="commandText">针对数据源执行的文本命令。</param>
        /// <returns>返回一个DbCommand执行命令对象。</returns>
        private DbCommand CreateCommandByCommandType(CommandType commandType, string commandText)
        {
            DbCommand command = this._DbProviderFactory.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

            return command;
        }

        /// <summary>
        /// 获取SQL文本命令类型的DbCommand对象。
        /// </summary>
        /// <param name="query">SQL文本命令。</param>
        /// <returns>DbCommand执行命令对象。</returns>
        public DbCommand GetSqlStringCommand(string query)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("执行的文本命令为空", "query");

            return CreateCommandByCommandType(CommandType.Text, query);
        }

        /// <summary>
        /// 获取存储过程类型的DbCommand对象。
        /// </summary>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <returns>DbCommand执行命令对象。</returns>
        public virtual DbCommand GetProcCommand(string ProcName)
        {
            if (string.IsNullOrEmpty(ProcName))
                throw new ArgumentException("存储过程名称为空值", "storedProcedureName");

            return CreateCommandByCommandType(CommandType.StoredProcedure, ProcName);
        }

        /// <summary>
        /// 获取存储过程类型的带参数的DbCommand对象。
        /// </summary>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程的参数。</param>
        /// <returns>DbCommand执行命令对象。</returns>
        public virtual DbCommand GetProcCommand(string ProcName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(ProcName))
                throw new ArgumentException("存储过程名称为空值", "storedProcedureName");

            DbCommand command = CreateCommandByCommandType(CommandType.StoredProcedure, ProcName);
            AssignParameters(command, parameterValues);

            return command;
        }

        /// <summary>
        /// 为DbCommand对象设置一个DbConnection对象。
        /// </summary>
        /// <param name="command">要设置连接的DbCommand对象。</param>
        /// <param name="connection">数据库连接池对象。</param>
        protected static void PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (connection == null) throw new ArgumentNullException("connection");

            command.Connection = connection;
        }

        /// <summary>
        /// 为DbCommand对象设置要在其中执行的一个DbTransaction事务对象。
        /// </summary>
        /// <param name="command">要设置事务的DbCommand对象。</param>
        /// <param name="transaction">事务对象。</param>
        protected static void PrepareCommand(DbCommand command, DbTransaction transaction)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (transaction == null) throw new ArgumentNullException("transaction");

            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;
        }

        #endregion

        #region 参数实现方法

        /// <summary>
        /// 向一个DbCommand对象中添加一个参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        public void AddInParameter(DbCommand command, string name, DbType dbType)
        {
            this.AddParameter(command, name, dbType, ParameterDirection.Input, string.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="value">参数值。</param>
        public void AddInParameter(DbCommand command, string name, DbType dbType, object value)
        {
            this.AddParameter(command, name, dbType, ParameterDirection.Input, string.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="sourceColumn">映射到DataSet中的源列名称。</param>
        /// <param name="sourceVersion">在加载参数时使用的DataRowVersion。</param>
        public void AddInParameter(DbCommand command, string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            this.AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="direction">参数类型。</param>
        /// <param name="sourceColumn">映射到DataSet中的源列名称。</param>
        /// <param name="sourceVersion">在加载参数时使用的DataRowVersion。</param>
        /// <param name="value">参数值。</param>
        public void AddParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            this.AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个输出参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="size">参数大小。</param>
        public void AddOutParameter(DbCommand command, string name, DbType dbType, int size)
        {
            this.AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, string.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// 添加参数对象。可重写。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="size">参数大小。</param>
        /// <param name="direction">参数类型。</param>
        /// <param name="nullable">是否可为空值。</param>
        /// <param name="precision">参数精度。</param>
        /// <param name="scale">比例。</param>
        /// <param name="sourceColumn">映射到DataSet中的源列名称。</param>
        /// <param name="sourceVersion">在加载参数时使用的DataRowVersion。</param>
        /// <param name="value">参数值。</param>
        public virtual void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            DbParameter parameter = this.CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// 创建一个参数对象。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="size">参数大小。</param>
        /// <param name="direction">参数类型。</param>
        /// <param name="nullable">是否可为空值。</param>
        /// <param name="precision">参数精度。</param>
        /// <param name="scale">比例。</param>
        /// <param name="sourceColumn">映射到DataSet中的源列名称。</param>
        /// <param name="sourceVersion">在加载参数时使用的DataRowVersion。</param>
        /// <param name="value">参数值。</param>
        /// <returns>创建的参数对象。</returns>
        protected DbParameter CreateParameter(string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = this.CreateParameter(name);
            this.ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return param;
        }

        /// <summary>
        /// 创建一个参数对象。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>只设置名称的参数对象。</returns>
        protected DbParameter CreateParameter(string name)
        {
            DbParameter param = this._DbProviderFactory.CreateParameter();
            param.ParameterName = this.BuildParameterName(name);
            return param;
        }

        /// <summary>
        /// 设置参数信息。可重写。
        /// </summary>
        /// <param name="param">设置的参数对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="size">参数大小。</param>
        /// <param name="direction">参数类型。</param>
        /// <param name="nullable">参数是否接受 null 值。</param>
        /// <param name="precision">参数精度。</param>
        /// <param name="scale">比例。</param>
        /// <param name="sourceColumn">源列的名称。</param>
        /// <param name="sourceVersion">加载 DbParameter.Value 时使用的 System.Data.DataRowVersion。默认值为 Current。</param>
        /// <param name="value">参数值。</param>
        protected virtual void ConfigureParameter(DbParameter param, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            param.DbType = dbType;
            param.Size = size;
            param.Value = value ?? DBNull.Value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
        }

        /// <summary>
        /// 设置参数的名称。可重写，自定义参数名称。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>重写之后的参数名称。</returns>
        public virtual string BuildParameterName(string name)
        {
            return name;
        }

        /// <summary>
        /// 为DbCommand对象要执行的存储过程所需的参数集合赋值。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="parameterValues">参数值集合。</param>
        public virtual void AssignParameters(DbCommand command, object[] parameterValues)
        {
            _ParameterCache.SetParameters(command, this);
            if (!this.IsSameNumberOfParamAndValues(command, parameterValues))
            {
                throw new InvalidOperationException("参数数目不匹配!");
            }
            this.AssignParameterValues(command, parameterValues);
        }

        /// <summary>
        /// 判断DbCommand对象中的参数数目是否与要分配的参数值集合数目一致。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="parameters">参数值集合。</param>
        /// <returns>是否数目一致。</returns>
        protected virtual bool IsSameNumberOfParamAndValues(DbCommand command, object[] parameters)
        {
            int numberOfParametersToStoredProcedure = command.Parameters.Count;
            int numberOfValuesProvidedForStoredProcedure = parameters.Length;
            return (numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure);
        }

        /// <summary>
        /// 为DbCommand对象中的参数赋值。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="values">参数值集合。</param>
        private void AssignParameterValues(DbCommand command, object[] values)
        {
            int parameterIndexShift = this.UserParametersStartIndex();
            for (int i = 0; i < values.Length; i++)
            {
                IDataParameter parameter = command.Parameters[i + parameterIndexShift];
                this.SetParameterValue(command, parameter.ParameterName, values[i]);
            }
        }

        /// <summary>
        /// 获取参数起始索引值。
        /// TODO:注意，默认为0。支持重写，即起始索引值可变。
        /// </summary>
        /// <returns></returns>
        protected virtual int UserParametersStartIndex()
        {
            return 0;
        }

        /// <summary>
        /// 为DbCommand对象中指定名称的参数赋值。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数值。</param>
        public virtual void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            command.Parameters[this.BuildParameterName(parameterName)].Value = value ?? DBNull.Value;
        }

        /// <summary>
        /// 从指定的DbCommand对象的参数集合和指定存储过程的参数集合中检索参数信息。
        /// </summary>
        /// <param name="discoveryCommand"></param>
        protected abstract void DeriveParameters(DbCommand discoveryCommand);

        /// <summary>
        /// 为DbCommand对象检索并添加参数信息。
        /// </summary>
        /// <param name="command"></param>
        public void DiscoverParameters(DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            using (ConnectionWrapper wrapper = GetOpenConnection())
            {
                using (DbCommand discoveryCommand = CreateCommandByCommandType(command.CommandType, command.CommandText))
                {
                    discoveryCommand.Connection = wrapper.Connection;
                    DeriveParameters(discoveryCommand);

                    foreach (IDataParameter parameter in discoveryCommand.Parameters)
                    {
                        IDataParameter cloneParameter = (IDataParameter)((ICloneable)parameter).Clone();
                        command.Parameters.Add(cloneParameter);
                    }
                }
            }
        }

        #endregion

        #region 数据库连接

        /// <summary>
        /// 创建一个新的数据库连接池对象。
        /// </summary>
        /// <returns>新的数据库连接池对象。</returns>
        public virtual DbConnection CreateConnection()
        {
            DbConnection connection = this._DbProviderFactory.CreateConnection();
            connection.ConnectionString = this._ConnectionString;
            return connection;
        }

        /// <summary>
        /// 获取一个新的已打开的数据库连接池对象。
        /// </summary>
        /// <returns>数据库连接池对象。</returns>
        internal DbConnection GetNewOpenConnection()
        {
            DbConnection connection = null;
            try
            {
                try
                {
                    connection = CreateConnection();
                    connection.Open();
                }
                catch
                {
                    throw;
                }
            }
            catch
            {
                if (connection != null)
                    connection.Close();

                throw;
            }

            return connection;
        }

        /// <summary>
        /// 获取数据库连接池管理对象。
        /// </summary>
        /// <returns>数据库连接池管理对象。</returns>
        protected virtual ConnectionWrapper GetWrapperConnection()
        {
            ConnectionWrapper wrapper = new ConnectionWrapper(GetNewOpenConnection());
            return wrapper;
        }

        /// <summary>
        /// 获取一个已打开的数据库连接池对象。
        /// </summary>
        /// <returns></returns>
        protected ConnectionWrapper GetOpenConnection()
        {
            return (TransConnectionWrapper.GetConnection(this) ?? GetWrapperConnection());
        }

        #endregion

        #region 事务

        private static DbTransaction BeginTransaction(DbConnection connection)
        {
            DbTransaction tran = connection.BeginTransaction();
            return tran;
        }

        private static void RollbackTransaction(IDbTransaction tran)
        {
            tran.Rollback();
        }

        private static void CommitTransaction(IDbTransaction tran)
        {
            tran.Commit();
        }

        #endregion

        #region DataSet

        /// <summary>
        /// 执行DataSet数据加载
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="dataSet">要加载数据的DataSet对象。</param>
        /// <param name="tableNames"></param>
        private void DoLoadDataSet(IDbCommand command, DataSet dataSet, string[] tableNames)
        {
            if (tableNames == null) throw new ArgumentNullException("tableNames");
            if (tableNames.Length == 0)
            {
                throw new ArgumentException("tableNames");
            }
            for (int i = 0; i < tableNames.Length; i++)
            {
                if (string.IsNullOrEmpty(tableNames[i]))
                    throw new ArgumentException(string.Concat("tableNames[", i, "]"));
            }

            using (DbDataAdapter adapter = GetDataAdapter(UpdateBehavior.Standard))
            {
                ((IDbDataAdapter)adapter).SelectCommand = command;

                try
                {
                    string systemCreatedTableNameRoot = "Table";
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string systemCreatedTableName = (i == 0)
                                                            ? systemCreatedTableNameRoot
                                                            : systemCreatedTableNameRoot + i;

                        adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                    }

                    adapter.Fill(dataSet);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 获取新建的一个数据适配器对象。
        /// </summary>
        /// <param name="updateBehavior">操作行为标识。</param>
        /// <returns>数据适配器对象。</returns>
        protected DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior)
        {
            DbDataAdapter adapter = this._DbProviderFactory.CreateDataAdapter();

            if (updateBehavior == UpdateBehavior.Continue)
            {
                SetUpRowUpdatedEvent(adapter);
            }
            return adapter;
        }

        /// <summary>
        /// 为数据适配器对象设置在对数据源执行命令后的 Update(System.Data.DataSet)过程中发生的更新事件。
        /// </summary>
        /// <param name="adapter"></param>
        protected virtual void SetUpRowUpdatedEvent(DbDataAdapter adapter) { }

        /// <summary>
        /// 加载一个DataSet到DbCommand对象中。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableName">要映射的表名。</param>
        public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            LoadDataSet(command, dataSet, new[] { tableName });
        }

        /// <summary>
        /// 加载一个DataSet到DbCommand对象中。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableNames">要映射的表名集合。</param>
        public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            using (ConnectionWrapper wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                DoLoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 在事务中执行DbCommand对象添加一个加载单个DataTable的DataSet。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableName">要映射的表名。</param>
        /// <param name="transaction">事务对象。</param>
        public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, DbTransaction transaction)
        {
            LoadDataSet(command, dataSet, new[] { tableName }, transaction);
        }

        /// <summary>
        /// 在事务中执行DbCommand对象添加一个加载多个DataTable的DataSet。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableNames">要映射的表名。</param>
        /// <param name="transaction">事务对象。</param>
        public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            DoLoadDataSet(command, dataSet, tableNames);
        }

        /// <summary>
        /// 通过执行存储过程返回的结果加载到DataSet中。
        /// </summary>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableNames">要映射的表名集合。</param>
        /// <param name="parameterValues">存储过程的参数集合。</param>
        public virtual void LoadDataSet(string ProcName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 在事务中执行存储过程返回的结果加载到DataSet中。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableNames">要映射的表名集合。</param>
        /// <param name="parameterValues">存储过程的参数集合。</param>
        public virtual void LoadDataSet(DbTransaction transaction, string ProcName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                LoadDataSet(command, dataSet, tableNames, transaction);
            }
        }

        /// <summary>
        /// 根据命令参数类型加载数据到DataSet中。
        /// </summary>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableNames">要映射的表名集合。</param>
        public virtual void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 在事务中根据命令参数类型加载数据到DataSet中。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令。</param>
        /// <param name="dataSet">要加载数据的DataSet。</param>
        /// <param name="tableNames">要映射的表名集合。</param>
        public void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames, transaction);
            }
        }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// 执行DbCommand并返回一个新的已加载数据的DataSet对象。
        /// </summary>
        /// <param name="command">命令执行对象。</param>
        /// <returns>已加载数据的DataSet对象。</returns>
        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table");
            return dataSet;
        }

        /// <summary>
        /// 执行事务中的DbCommand对象，返回一个新的已加载数据的DataSet对象。
        /// </summary>
        /// <param name="command">命令执行对象。</param>
        /// <param name="transaction">事务对象。</param>
        /// <returns>已加载数据的DataSet对象。</returns>
        public virtual DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction)
        {
            var dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table", transaction);
            return dataSet;
        }

        /// <summary>
        /// 执行一个存储过程，并返回加载相应数据的DataSet对象。
        /// </summary>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程所需的参数集合。</param>
        /// <returns>加载相应数据的DataSet对象。</returns>
        public virtual DataSet ExecuteDataSet(string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteDataSet(command);
            }
        }

        /// <summary>
        /// 执行事务中的存储过程，并返回加载数据的DataSet对象。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程所需的参数集合。</param>
        /// <returns>加载数据的DataSet对象。</returns>
        public virtual DataSet ExecuteDataSet(DbTransaction transaction, string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteDataSet(command, transaction);
            }
        }

        /// <summary>
        /// 根据命令参数类型执行文本命令，并返回已加载数据的DataSet对象。
        /// </summary>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>已加载数据的DataSet对象。</returns>
        public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command);
            }
        }

        /// <summary>
        /// 执行事务中指定的文本命令或存储过程，并返回已加载数据的DataSet对象。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>已加载数据的DataSet对象。</returns>
        public virtual DataSet ExecuteDataSet(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command, transaction);
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// 执行继承IDbCommand接口的执行命令对象，并返回执行命令所影响的行数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <returns>执行命令所影响的行数。</returns>
        protected int DoExecuteNonQuery(IDbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 执行一个DbCommand对象，并返回执行命令所影响的行数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <returns>执行命令所影响的行数。</returns>
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            using (ConnectionWrapper wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                return DoExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// 执行事务中的DbCommand对象。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="transaction">事务对象。</param>
        /// <returns>执行命令所影响的行数。</returns>
        public virtual int ExecuteNonQuery(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// 执行存储过程并返回受影响的行数。
        /// </summary>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程的参数集合。</param>
        /// <returns>执行存储过程所影响的行数。</returns>
        public virtual int ExecuteNonQuery(string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// 执行事务中的存储过程并返回受影响的行数。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程的参数集合。</param>
        /// <returns>受影响的行数。</returns>
        public virtual int ExecuteNonQuery(DbTransaction transaction, string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteNonQuery(command, transaction);
            }
        }

        /// <summary>
        /// 根据命令参数类型执行对应的文本命令或存储过程并返回受影响的行数。
        /// </summary>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>受影响的行数。</returns>
        public virtual int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// 在事务中执行与命令参数类型匹配的命令并返回受影响的行数。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>受影响的行数。</returns>
        public virtual int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command, transaction);
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行继承IDbCommand接口的执行命令对象，返回结果集的第一行第一列的值。
        /// </summary>
        /// <param name="command">继承IDbCommand接口的执行命令对象。</param>
        /// <returns>结果集第一行的第一列。</returns>
        private object DoExecuteScalar(IDbCommand command)
        {
            try
            {
                object returnValue = command.ExecuteScalar();
                return returnValue;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 执行一个DbCommand对象，并返回结果集第一行第一列的值。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <returns>结果集第一行的第一列。</returns>
        public virtual object ExecuteScalar(DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                return DoExecuteScalar(command);
            }
        }

        /// <summary>
        /// 执行事务中的DbCommand对象，并返回结果集第一行第一列的值。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="transaction">事务对象。</param>
        /// <returns>结果集第一行的第一列。</returns>
        public virtual object ExecuteScalar(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteScalar(command);
        }

        /// <summary>
        /// 执行一个存储过程，并返回结果集第一行第一列的值。
        /// </summary>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程所需的参数集合。</param>
        /// <returns>结果集第一行的第一列。</returns>
        public virtual object ExecuteScalar(string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteScalar(command);
            }
        }

        /// <summary>
        /// 执行事务中的一个存储过程，并返回结果集第一行第一列的值。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程所需的参数集合。</param>
        /// <returns>结果集第一行的第一列。</returns>
        public virtual object ExecuteScalar(DbTransaction transaction, string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteScalar(command, transaction);
            }
        }

        /// <summary>
        /// 根据命令参数类型执行对应的文本命令或存储过程，并返回结果集第一行第一列的值。
        /// </summary>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>结果集第一行的第一列。</returns>
        public virtual object ExecuteScalar(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command);
            }
        }

        /// <summary>
        /// 执行事务中根据命令参数类型对应的文本命令或存储过程，并返回结果集第一行第一列的值。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>结果集第一行的第一列。</returns>
        public virtual object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command, transaction);
            }
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// 根据执行命令控制指示执行继承IDbCommand接口的执行命令对象。
        /// </summary>
        /// <param name="command">继承IDbCommand接口的执行命令对象。</param>
        /// <param name="cmdBehavior">执行命令控制指示</param>
        /// <returns>只进数据读取器的接口类型对象。</returns>
        private IDataReader DoExecuteReader(IDbCommand command, CommandBehavior cmdBehavior)
        {
            try
            {
                IDataReader reader = command.ExecuteReader(cmdBehavior);
                return reader;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 创建一个封装管理只进的数据读取器和数据库连接池管理对象的管理对象。
        /// </summary>
        /// <param name="connection">数据库连接池的管理对象。</param>
        /// <param name="innerReader">只进的数据读取器。</param>
        /// <returns>封装管理只进的数据读取器对象和数据库连接池管理对象的管理对象。</returns>
        protected virtual IDataReader CreateWrappedReader(ConnectionWrapper connection, IDataReader innerReader)
        {
            return new RefCountingDataReader(connection, innerReader);
        }

        /// <summary>
        /// 执行一个DbCommand对象，并返回已加载数据的只进数据读取器。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <returns>已加载数据的只进数据读取器。</returns>
        public virtual IDataReader ExecuteReader(DbCommand command)
        {
            using (ConnectionWrapper wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                IDataReader realReader = DoExecuteReader(command, CommandBehavior.Default);
                return CreateWrappedReader(wrapper, realReader);
            }
        }

        /// <summary>
        /// 执行事务中的一个DbCommand对象，并返回已加载数据的只进数据读取器。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="transaction">事务对象。</param>
        /// <returns>已加载数据的只进数据读取器。</returns>
        public virtual IDataReader ExecuteReader(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteReader(command, CommandBehavior.Default);
        }

        /// <summary>
        /// 执行一个存储过程，并返回已加载数据的只进数据读取器。
        /// </summary>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程对应所需的参数集合。</param>
        /// <returns>已加载数据的只进数据读取器。</returns>
        public IDataReader ExecuteReader(string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteReader(command);
            }
        }

        /// <summary>
        /// 执行事务中的一个存储过程，并返回已加载数据的只进数据读取器。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="ProcName">存储过程的名称。</param>
        /// <param name="parameterValues">存储过程对应所需的参数集合。</param>
        /// <returns>已加载数据的只进数据读取器。</returns>
        public IDataReader ExecuteReader(DbTransaction transaction, string ProcName, params object[] parameterValues)
        {
            using (DbCommand command = GetProcCommand(ProcName, parameterValues))
            {
                return ExecuteReader(command, transaction);
            }
        }

        /// <summary>
        /// 根据命令参数类型，执行对应的文本命令或存储过程，并返回已加载数据的只进数据读取器。
        /// </summary>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>已加载数据的只进数据读取器。</returns>
        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command);
            }
        }

        /// <summary>
        /// 根据命令参数类型，执行事务中的文本命令或存储过程，并返回已加载数据的只进数据读取器。
        /// </summary>
        /// <param name="transaction">事务对象。</param>
        /// <param name="commandType">命令参数类型。</param>
        /// <param name="commandText">文本命令或存储过程的名称。</param>
        /// <returns>已加载数据的只进数据读取器。</returns>
        public IDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command, transaction);
            }
        }

        #endregion

        #endregion
    }
}
