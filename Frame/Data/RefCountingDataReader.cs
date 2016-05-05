using System;
using System.Data;

namespace Frame.Data
{
    /// <summary>
    /// 提供正确处理数据库连接池对象的计数的数据只读器计数管理的类。
    /// </summary>
    internal class RefCountingDataReader : DataReaderWrapper
    {
        /// <summary>
        /// 表示一个数据库连接池管理对象。
        /// </summary>
        private readonly ConnectionWrapper _ConnectionWrapper;

        /// <summary>
        /// 创建一个内部的只进的数据读取器对象，并且设置对应的进行计数的数据库连接池管理对象。
        /// </summary>
        /// <param name="connection">数据库连接池管理对象。</param>
        /// <param name="innerReader">内部的只进数据读取器对象。</param>
        public RefCountingDataReader(ConnectionWrapper connection, IDataReader innerReader)
            : base(innerReader)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (innerReader == null)
                throw new ArgumentNullException("innerReader");

            this._ConnectionWrapper = connection;
            this._ConnectionWrapper.AddRefCount();
        }

        /// <summary>
        /// 关闭当前的只进数据读取器，并释放对应的数据库连接池管理对象。
        /// </summary>
        public override void Close()
        {
            if (!IsClosed)
            {
                base.Close();
                this._ConnectionWrapper.Dispose();
            }
        }

        /// <summary>
        /// 释放只进数据读取器。
        /// </summary>
        /// <param name="disposing">设置一个值，该值指示是否释放只进数据读取器的资源。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!IsClosed)
                {
                    base.Dispose(true);
                    this._ConnectionWrapper.Dispose();
                }
            }
        }
    }
}
