using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Frame.Data.SqlGe
{
    /// <summary>
    /// 提供访问SqlServer数据库的统一方法。该类实现数据访问基类。
    /// </summary>
    public class SqlGeClient : DataBase
    {
        
        #region 属性

        /// <summary>
        /// 获取参数的标识@
        /// </summary>
        protected char ParameterToken
        {
            get { return '@'; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造访问SqlServer数据库的对象。
        /// </summary>
        /// <param name="connectionString">SqlServer数据库连接字符串。</param>
        public SqlGeClient(string connectionString)
            : base(connectionString, SqlClientFactory.Instance)
        {
        }

        #endregion

        #region 方法

        #region 参数重写方法

        /// <summary>
        /// 添加参数对象。
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
        public virtual void AddParameter(DbCommand command, string name, SqlDbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            DbParameter parameter = CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
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
        public void AddParameter(DbCommand command, string name, SqlDbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个输出参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="size">参数大小。</param>
        public void AddOutParameter(DbCommand command, string name, SqlDbType dbType, int size)
        {
            AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        public void AddInParameter(DbCommand command, string name, SqlDbType dbType)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="value">参数值。</param>
        public void AddInParameter(DbCommand command, string name, SqlDbType dbType, object value)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// 向一个DbCommand对象中添加一个参数。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="sourceColumn">映射到DataSet中的源列名称。</param>
        /// <param name="sourceVersion">在加载参数时使用的DataRowVersion。</param>
        public void AddInParameter(DbCommand command, string name, SqlDbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
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
        protected DbParameter CreateParameter(string name, SqlDbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter param = CreateParameter(name) as SqlParameter;
            ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return param;
        }

        /// <summary>
        /// 设置参数信息。
        /// </summary>
        /// <param name="param">设置的参数对象。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="dbType">参数数据类型。</param>
        /// <param name="size">参数大小。</param>
        /// <param name="direction">参数类型。</param>
        /// <param name="nullable">参数是否接受 null 值。</param>
        /// <param name="precision">参数精度。</param>
        /// <param name="scale">比例。</param>
        /// <param name="sourceColumn">映射到DataSet中的源列名称。</param>
        /// <param name="sourceVersion">加载 SqlParameter.Value 时使用的 System.Data.DataRowVersion。默认值为 Current。</param>
        /// <param name="value">参数值。</param>
        protected virtual void ConfigureParameter(SqlParameter param, string name, SqlDbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            param.SqlDbType = dbType;
            param.Size = size;
            param.Value = value ?? DBNull.Value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
        }

        /// <summary>
        /// 根据执行命令类型创建对应的SqlCommand执行命令对象。
        /// </summary>
        /// <param name="commandType">执行命令类型。</param>
        /// <param name="commandText">针对数据源执行的文本命令。</param>
        /// <returns>返回一个SqlCommand执行命令对象。</returns>
        private static SqlCommand CreateSqlCommandByCommandType(CommandType commandType, string commandText)
        {
            return new SqlCommand(commandText)
            {
                CommandType = commandType
            };
        }

        #endregion

        #region 基础实现方法

        /// <summary>
        /// 从指定的DbCommand对象的参数集合和指定存储过程的参数集合中检索参数信息。
        /// </summary>
        /// <param name="discoveryCommand">进行参数填充的执行命令对象。</param>
        protected override void DeriveParameters(DbCommand discoveryCommand)
        {
            SqlCommandBuilder.DeriveParameters((SqlCommand)discoveryCommand);
        }

        /// <summary>
        /// 获取参数起始索引值。
        /// </summary>
        /// <returns>起始索引值，默认指定为1。</returns>
        protected override int UserParametersStartIndex()
        {
            return 1;
        }

        /// <summary>
        /// 设置参数的名称。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>设置之后的参数名称。</returns>
        public override string BuildParameterName(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            if (name[0] != ParameterToken)
            {
                return name.Insert(0, new string(ParameterToken, 1));
            }
            return name;
        }

        /// <summary>
        /// 为数据适配器对象设置在对数据源执行命令后的 Update(DataSet)过程中发生的更新事件。
        /// </summary>
        /// <param name="adapter">指定的数据适配器对象。</param>
        protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter)
        {
            ((SqlDataAdapter)adapter).RowUpdated += OnSqlRowUpdated;
        }

        /// <summary>
        /// 数据源执行System.Data.Common.DbDataAdapter.Update命令过程激发事件的方法。
        /// </summary>
        /// <param name="sender">事件源对象。</param>
        /// <param name="rowThatCouldNotBeWritten">包含事件数据的 System.Data.SqlClient.SqlRowUpdatedEventArgs。</param>
        private void OnSqlRowUpdated(object sender, SqlRowUpdatedEventArgs rowThatCouldNotBeWritten)
        {
            if (rowThatCouldNotBeWritten.RecordsAffected == 0)
            {
                if (rowThatCouldNotBeWritten.Errors != null)
                {
                    rowThatCouldNotBeWritten.Row.RowError = "更新行数据失败!";
                    rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
                }
            }
        }

        /// <summary>
        /// 判断DbCommand对象中的参数数目是否与要分配的参数值集合数目一致。
        /// </summary>
        /// <param name="command">执行命令对象。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>是否数目一致。</returns>
        protected override bool IsSameNumberOfParamAndValues(DbCommand command, object[] values)
        {
            int returnParameterCount = 1;
            int numberOfParametersToStoredProcedure = command.Parameters.Count - returnParameterCount;
            int numberOfValuesProvidedForStoredProcedure = values.Length;
            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        #endregion

        #endregion

    }
}
