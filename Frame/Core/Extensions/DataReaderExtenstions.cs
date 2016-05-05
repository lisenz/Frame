using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace Frame.Core.Extensions
{
    public static class DataReaderExtenstions
    {
        /// <summary>
        /// 将通过在数据源执行命令所获得的只进结果集流的多行数据转换为键/值对集合类型的集合对象。
        /// </summary>
        /// <param name="reader">在数据源执行命令所获得的只进结果集流。</param>
        /// <returns>转换为键/值对集合类型的集合对象，若结果集中不存在数据，则返回空集合。</returns>
        public static IList<IDictionary<string, object>> ReadDataToDictionarys(this IDataReader reader)
        {
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("读取器已经被关闭。");
            }

            List<IDictionary<string, object>> rows = new List<IDictionary<string, object>>();
            IDictionary<string, object> row;
            while (null != (row = ReadDataToDictionary(reader)))
            {
                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// 将通过在数据源执行命令所获得的只进结果集流的单条数据转换为单个字典类型的对象。
        /// </summary>
        /// <param name="reader">在数据源执行命令所获得的只进结果集流。</param>
        /// <returns>转换为字典集合类型的对象，若结果集中不存在数据，则返回null。</returns>
        public static IDictionary<string, object> ReadDataToDictionary(this IDataReader reader)
        {
            if (reader.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int index = 0; index < reader.FieldCount; index++)
                {
                    row.Add(reader.GetName(index), reader.GetValue(index));
                }

                return row;
            }

            return null;
        }

        /// <summary>
        /// 将通过在数据源执行命令所获得的只进结果集流的数据转换为DataTable。
        /// </summary>
        /// <param name="reader">在数据源执行命令所获得的只进结果集流。</param>
        /// <returns>转换为DataTable类型的对象，若结果集中不存在数据，则返回null。</returns>
        public static DataTable ReadDataToTable(this IDataReader reader)
        {
            bool isFirst = true;
            DataTable table = new DataTable();

            while (reader.Read())
            {
                if (isFirst)
                {
                    isFirst = false;
                    for (int index = 0; index < reader.FieldCount; index++)
                    {
                        table.Columns.Add(reader.GetName(index), reader.GetValue(index).GetType());
                    }
                }

                DataRow row = table.NewRow();
                for (int index = 0; index < reader.FieldCount; index++)
                {
                    row[reader.GetName(index)] = reader.GetValue(index);
                }
                table.Rows.Add(row.ItemArray);
            }


            return table;
        }
        
    }
}
