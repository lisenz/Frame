using System;
using System.Linq;
using System.Configuration;
//-------------------------
using Frame.Core;
using Frame.Data;
using Frame.Data.SqlGe;
using Frame.Data.SqliteGe;

namespace Frame.Data
{
    /// <summary>
    /// 数据库容器。
    /// </summary>
    internal class DataBaseLibraryContainer
    {
        /// <summary>
        /// 表示一个数据库容器对象。
        /// </summary>
        private static DataBaseLibraryContainer _Container;

        /// <summary>
        /// 构造函数
        /// </summary>
        private DataBaseLibraryContainer()
        {
        }

        /// <summary>
        /// 静态构造函数。初始化一个数据库容器对象。
        /// </summary>
        static DataBaseLibraryContainer()
        {
            _Container = new DataBaseLibraryContainer();
        }

        /// <summary>
        /// 获取一个当前可用的数据库容器对象。
        /// </summary>
        public static DataBaseLibraryContainer Current
        {
            get
            {
                if (null == _Container)
                    _Container = new DataBaseLibraryContainer();

                return _Container;
            }
        }

        /// <summary>
        /// 获取指定名称的数据库访问对象。
        /// </summary>
        /// <param name="name">配置文件中连接串的名称。</param>
        /// <returns>返回数据库访问对象。</returns>
        public DataBase GetDataBase(string name)
        {
            try
            {
                DataBase db;
                if (!App.ObjectContainer.TryGetObject<DataBase>(name, out db))
                {
                    string connectString = App.ConnectionStrings[name].ConnectionString;
                    string providerName = App.ConnectionStrings[name].ProviderName;
                    switch (providerName)
                    {
                        case "Sqlite":
                            if (connectString.StartsWith("connect:"))
                                connectString = string.Format("Data Source={0}{1}", App.BaseDirectory, connectString.Substring(8));
                            db = RegisterSqliteGe(connectString);
                            break;
                        default: 
                            db = RegisterSqlGe(connectString);
                            break;
                    }
                    App.ObjectContainer.Register<DataBase>(name, db);
                    return db;
                }

                return db;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 实例化一个SqlServer的数据库访问框架对象。
        /// </summary>
        /// <param name="connectString">数据库的连接字符串。</param>
        /// <returns>返回一个SqlGeClient对象。</returns>
        private DataBase RegisterSqlGe(string connectString)
        {
            DataBase db = new SqlGeClient(connectString);
            return db;
        }

        /// <summary>
        /// 实例化一个Sqlite的数据库访问框架对象。
        /// </summary>
        /// <param name="connectString">数据库的连接字符串。</param>
        /// <returns>返回一个SqliteGeClient。</returns>
        private DataBase RegisterSqliteGe(string connectString)
        {
            DataBase db = new SqliteGeClient(connectString);
            return db;
        }
    }
}
