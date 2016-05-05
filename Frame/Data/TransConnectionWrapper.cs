using System;
using System.Data.Common;
using System.Transactions;
using System.Collections.Generic;

namespace Frame.Data
{
    /// <summary>
    /// 封装数据库连接池的事务管理，提供对数据库连接池对象的事务支持管理的方法。
    /// </summary>
    internal class TransConnectionWrapper
    {
        #region 字段

        /// <summary>
        /// 存储事务的对应数据库连接对象列表。
        /// </summary>
        private static readonly Dictionary<Transaction, Dictionary<string, ConnectionWrapper>> _TransConnections =
            new Dictionary<Transaction, Dictionary<string, ConnectionWrapper>>();

        #endregion

        #region 方法

        /// <summary>
        /// 获取当前事务中指定检索的数据库连接池对象。
        /// </summary>
        /// <param name="db">数据库对象。</param>
        /// <returns>检索到的打开的数据库连接池对象。</returns>
        public static ConnectionWrapper GetConnection(DataBase db)
        {
            Transaction fCurrentTransaction = Transaction.Current;

            if (fCurrentTransaction == null)
                return null;

            Dictionary<string, ConnectionWrapper> connectionList;
            ConnectionWrapper connection;

            lock (_TransConnections)
            {
                if (!_TransConnections.TryGetValue(fCurrentTransaction, out connectionList))
                {
                    connectionList = new Dictionary<string, ConnectionWrapper>();
                    _TransConnections.Add(fCurrentTransaction, connectionList);

                    fCurrentTransaction.TransactionCompleted += OnTransactionCompleted;
                }
            }

            lock (connectionList)
            {
                if (!connectionList.TryGetValue(db.ConnectionString, out connection))
                {
                    var dbConnection = db.GetNewOpenConnection();
                    connection = new ConnectionWrapper(dbConnection);
                    connectionList.Add(db.ConnectionString, connection);
                }
                connection.AddRefCount();
            }

            return connection;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 事务完成。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            Dictionary<string, ConnectionWrapper> connectionList;

            lock (_TransConnections)
            {
                if (!_TransConnections.TryGetValue(e.Transaction, out connectionList))
                {
                    return;
                }

                _TransConnections.Remove(e.Transaction);
            }

            lock (connectionList)
            {
                foreach (var connectionWrapper in connectionList.Values)
                {
                    connectionWrapper.Dispose();
                }
            }
        }

        #endregion

    }
}
