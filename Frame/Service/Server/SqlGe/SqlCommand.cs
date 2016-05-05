using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frame.DataStore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Data;
//using Frame.DataStore.Utility;

namespace Frame.Service.Server.SqlGe
{
    /// <summary>
    /// 提供执行数据库服务请求的操作方法。
    /// </summary>
    internal class SqlCommand : IServiceCommand
    {
        /// <summary>
        /// 表示一个SQL语句对象。
        /// </summary>
        private readonly ISqlGeStatement _sql;

        /// <summary>
        /// 构造函数，初始化SQL语句对象。
        /// </summary>
        /// <param name="sql"></param>
        public SqlCommand(ISqlGeStatement sql)
        {
            _sql = sql;
        }

        /// <summary>
        /// 执行SQL命令，并返回执行结果。
        /// </summary>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>执行结果。</returns>
        public object Execute(IServiceContext context)
        {
            var converter = new KeyValuePairConverter();
            var jsonParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                   Convert.ToString(context.Params["Params"]), converter);
            BaseDao dao = string.IsNullOrEmpty(_sql.Connection) ? BaseDao.Get() : BaseDao.Get(_sql.Connection);
            if (_sql.IsQuery)
            {
                using (IDataReader reader = dao.QueryReader(_sql.Text, jsonParams))
                {
                    return GetResultFromReader(reader);
                }
            }
            else
            {
                //int affectRows = DaoCommand.ExecuteNonQuery(_sql, jsonParams);
                //return new SqlResult() { AffectRows = affectRows };
                int affectRows = dao.ExecuteNonQuery(_sql.Text, jsonParams);
                return new SqlResult() { AffectRows = affectRows };
            }
        }

        /// <summary>
        /// 从只进结果集将数据转换为SqlResult结构返回。
        /// </summary>
        /// <param name="reader">执行SQL语句获得的只进结果集。</param>
        /// <returns>转换后执行类型格式的结果对象。</returns>
        private static SqlResult GetResultFromReader(IDataReader reader)
        {
            var result = new SqlResult();

            string[] columns = new string[reader.FieldCount];
            Type[] types = new Type[reader.FieldCount];
            bool isSetTypes = false;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns[i] = reader.GetName(i);
            }

            result.ColumnNames = columns;
            result.AffectRows = reader.RecordsAffected;

            var rows = new List<object[]>();
            result.Rows = rows;
            while (reader.Read())
            {
                if (!isSetTypes)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        types[i] = reader.GetFieldType(i);
                    }
                    result.ColumnTypes = types;
                    isSetTypes = true;
                }
                var row = result.ColumnNames
                                    .Select(name =>
                                    {
                                        var value = reader[name];
                                        return value == DBNull.Value ? null : value;
                                    }).ToArray();
                rows.Add(row);
            }

            result.TotalRows = rows.Count;

            return result;
        }
    }
}
