using System;
using System.Threading;
using System.Data.Common;

namespace Frame.Data
{
    /// <summary>
    /// 封装对数据库连接池对象的管理，提供对数据库连接池对象管理的属性和方法。
    /// 注意:这里引用计数控制，当不存在有用户使用连接时才进行关闭。
    /// </summary>
    public class ConnectionWrapper : IDisposable
    {
        #region 字段

        /// <summary>
        /// 表示计数，记录当前管理的数据库连接池对象的使用数。
        /// </summary>
        private int _refCount;

        #endregion

        #region 属性

        /// <summary>
        /// 内部管理的数据库连接池对象。
        /// </summary>
        public DbConnection Connection
        {
            get;
            private set;
        }

        /// <summary>
        /// 内部管理的数据库连接池对象是否已被释放。
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return (this._refCount == 0);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建一个新的封装特定连接池对象的管理对象。
        /// </summary>
        /// <param name="connection">打开的连接池对象。</param>
        public ConnectionWrapper(DbConnection connection)
        {
            this.Connection = connection;
            this._refCount = 1;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 进行计数。
        /// </summary>
        /// <returns>返回当前管理的数据库连接池对象。</returns>
        public ConnectionWrapper AddRefCount()
        {
            Interlocked.Increment(ref this._refCount);
            return this;
        }

        #endregion

        #region 释放实现

        /// <summary>
        /// 释放分配的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 释放分配的资源，其中包括关闭数据库连接及垃圾回收。
        /// </summary>
        /// <param name="disposing">标识是否进行释放资源。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (Interlocked.Decrement(ref this._refCount) == 0))
            {
                this.Connection.Dispose();
                this.Connection = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
