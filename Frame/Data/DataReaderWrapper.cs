using System;
using System.Data;

namespace Frame.Data
{
    /// <summary>
    /// 封装对DataReader管理的基类，该类为抽象类，提供管理DataReader只读器的属性和方法。
    /// </summary>
    internal class DataReaderWrapper : MarshalByRefObject, IDataReader
    {
        #region 字段

        /// <summary>
        /// 表示一个内部的只进数据读取器。
        /// </summary>
        private readonly IDataReader _InnerReader;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造一个新的数据只进读取器赋给内部的只进数据读取器。
        /// </summary>
        /// <param name="innerReader">新建的数据只进读取器。</param>
        protected DataReaderWrapper(IDataReader innerReader)
        {
            this._InnerReader = innerReader;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取数据读取器对象。
        /// </summary>
        public IDataReader InnerReader
        {
            get
            {
                return this._InnerReader;
            }
        }

        /// <summary>
        /// 获取只读器中当前行的列数。
        /// </summary>
        public virtual int FieldCount
        {
            get { return this._InnerReader.FieldCount; }
        }

        /// <summary>
        /// 获取一个值，该值指示当前行的嵌套深度。
        /// </summary>
        public virtual int Depth
        {
            get { return this._InnerReader.Depth; }
        }

        /// <summary>
        /// 获取一个值，该值指示读取器是否已关闭。
        /// </summary>
        public virtual bool IsClosed
        {
            get { return this._InnerReader.IsClosed; }
        }

        /// <summary>
        /// 通过执行SQL语句获取更改、插入或删除的行数。
        /// </summary>
        public virtual int RecordsAffected
        {
            get { return this._InnerReader.RecordsAffected; }
        }

        /// <summary>
        /// 内部读取器的索引器，访问每一行中指定索引的列值。
        /// </summary>
        /// <param name="i">列的索引值。</param>
        /// <returns>指定索引的列值</returns>
        object IDataRecord.this[int i]
        {
            get { return this._InnerReader[i]; }
        }

        /// <summary>
        /// 内部读取器的索引器，访问每一行中指定名称的列值。
        /// </summary>
        /// <param name="name">列的名称。</param>
        /// <returns>指定名称的列值。</returns>
        object IDataRecord.this[string name]
        {
            get { return this._InnerReader[name]; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 根据要查找的字段的索引获取要查找的字段的名称。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>字段的名称。</returns>
        public virtual string GetName(int i)
        {
            return this._InnerReader.GetName(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取要查找的字段的数据类型信息。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>字段的数据类型信息。</returns>
        public virtual string GetDataTypeName(int i)
        {
            return this._InnerReader.GetDataTypeName(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取要查找的字段的System.Type信息。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>字段的System.Type信息。</returns>
        public virtual Type GetFieldType(int i)
        {
            return this._InnerReader.GetFieldType(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取要查找的字段的值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>字段的值。</returns>
        public virtual object GetValue(int i)
        {
            return this._InnerReader.GetValue(i);
        }

        /// <summary>
        /// 使用当前记录的列值来填充对象数组。
        /// </summary>
        /// <param name="values">要将属性字段复制到的System.Object的数组。</param>
        /// <returns>数组中System.Object的实例的数目。</returns>
        public virtual int GetValues(object[] values)
        {
            return this._InnerReader.GetValues(values);
        }

        /// <summary>
        /// 获取指定名称的字段的索引。
        /// </summary>
        /// <param name="name">字段名称。</param>
        /// <returns>字段的索引。</returns>
        public virtual int GetOrdinal(string name)
        {
            return this._InnerReader.GetOrdinal(name);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的布尔类型的值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>字段的布尔类型的值。</returns>
        public virtual bool GetBoolean(int i)
        {
            return this._InnerReader.GetBoolean(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的8位无符号整数值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>字段的8位无符号整数值。</returns>
        public virtual byte GetByte(int i)
        {
            return this._InnerReader.GetByte(i);
        }

        /// <summary>
        /// 从指定的列偏移量将字节流作为数组从给定的缓冲区偏移量开始读入缓冲区。
        /// </summary>
        /// <param name="i">从零开始的列序号。</param>
        /// <param name="fieldOffset">字段中的索引，从该索引位置开始读取操作。</param>
        /// <param name="buffer">要将字节流读入的缓冲区。</param>
        /// <param name="bufferoffset">开始读取操作的 buffer 索引。</param>
        /// <param name="length">要读取的字节数。</param>
        /// <returns>读取的实际字节数。</returns>
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._InnerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定列的字符值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定列的字符值。</returns>
        public virtual char GetChar(int i)
        {
            return this._InnerReader.GetChar(i);
        }

        /// <summary>
        /// 从指定的列偏移量将字符流作为数组从给定的缓冲区偏移量开始读入缓冲区。
        /// </summary>
        /// <param name="i">从零开始的列序号。</param>
        /// <param name="fieldoffset">行中的索引，从该索引位置开始读取操作。</param>
        /// <param name="buffer">要将字节流读入的缓冲区。</param>
        /// <param name="bufferoffset">开始读取操作的 buffer 索引。</param>
        /// <param name="length">要读取的字节数。</param>
        /// <returns>读取的实际字符数。</returns>
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._InnerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的 GUID 值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的 GUID 值。</returns>
        public virtual Guid GetGuid(int i)
        {
            return this._InnerReader.GetGuid(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的16位有符号整数值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的16位有符号整数值。</returns>
        public virtual short GetInt16(int i)
        {
            return this._InnerReader.GetInt16(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的32位有符号整数值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的32位有符号整数值。</returns>
        public virtual int GetInt32(int i)
        {
            return this._InnerReader.GetInt32(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的64位有符号整数值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的64位有符号整数值。</returns>
        public virtual long GetInt64(int i)
        {
            return this._InnerReader.GetInt64(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的单精度浮点数。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的单精度浮点数。</returns>
        public virtual float GetFloat(int i)
        {
            return this._InnerReader.GetFloat(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的双精度浮点数。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的双精度浮点数。</returns>
        public virtual double GetDouble(int i)
        {
            return this._InnerReader.GetDouble(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的字符串值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的字符串值。</returns>
        public virtual string GetString(int i)
        {
            return this._InnerReader.GetString(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的固定位置的数值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的固定位置的数值。</returns>
        public virtual decimal GetDecimal(int i)
        {
            return this._InnerReader.GetDecimal(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定字段的日期和时间数据值。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段的日期和时间数据值。</returns>
        public virtual DateTime GetDateTime(int i)
        {
            return this._InnerReader.GetDateTime(i);
        }

        /// <summary>
        /// 根据要查找的字段的索引获取指定的列序号的IDataReader。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定的列序号的IDataReader。</returns>
        public virtual IDataReader GetData(int i)
        {
            return this._InnerReader.GetData(i);
        }

        /// <summary>
        /// 返回是否将指定字段设置为空。
        /// </summary>
        /// <param name="i">要查找的字段的索引。</param>
        /// <returns>指定字段设置为空。</returns>
        public virtual bool IsDBNull(int i)
        {
            return this._InnerReader.IsDBNull(i);
        }

        /// <summary>
        /// 返回一个DataTable，它描述 System.Data.IDataReader 的列元数据。
        /// </summary>
        /// <returns>一个描述IDataReader的列元数据的DataTable。</returns>
        public virtual DataTable GetSchemaTable()
        {
            return this._InnerReader.GetSchemaTable();
        }

        /// <summary>
        /// 当读取批处理SQL语句的结果时，使数据读取器前进到下一个结果。
        /// </summary>
        /// <returns>如果存在多个行，则为 true；否则为 false。</returns>
        public virtual bool NextResult()
        {
            return this._InnerReader.NextResult();
        }

        /// <summary>
        /// 使读取器前进到下一条记录。
        /// </summary>
        /// <returns>如果存在多个行，则为 true；否则为 false。</returns>
        public virtual bool Read()
        {
            return this._InnerReader.Read();
        }

        #endregion

        #region 释放对象，关闭读取器

        /// <summary>
        /// 关闭 System.Data.IDataReader 对象。
        /// </summary>
        public virtual void Close()
        {
            if (!this._InnerReader.IsClosed)
            {
                this._InnerReader.Close();
            }
        }

        /// <summary>
        /// 释放分配的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放只读数据读取器。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this._InnerReader.IsClosed)
                {
                    this._InnerReader.Dispose();
                }
            }
        }

        #endregion
    }
}
