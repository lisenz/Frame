using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
//----------------
using Frame.Data;
using Frame.Core.Extensions;
using Frame.DataStore.Provider;

namespace Frame.DataStore
{
    /// <summary>
    /// 数据库访问框架生产工厂类。
    /// </summary>
    public sealed class DaoFactory
    {
        /// <summary>
        /// 默认数据库连接名称。
        /// </summary>
        public const string DefaultConnectionName = "DefaultDB";

        private static ISqlGeSource _SqlSource;
        private static IDictionary<string, BaseDao> _Daos;
        private static readonly ReaderWriterLockSlim _DaosLock = new ReaderWriterLockSlim();
        private static IDictionary<string, IDaoProvider> _Providers;
        private static readonly ReaderWriterLockSlim _ProviderLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静态构造函数，初始化相关配置以及数据源信息。
        /// </summary>
        static DaoFactory()
        {
            Initialize();
        }

        /// <summary>
        /// 初始化加载相关配置以及数据源信息。
        /// </summary>
        private static void Initialize()
        {
            _Daos = new Dictionary<string, BaseDao>();
            _SqlSource = (new SqlGeClient.SqlGeSource()).LoadSqls();

            InitializeProviders();
        }

        /// <summary>
        /// 获取预加载的SQL语句源。
        /// </summary>
        /// <returns>SQL语句源。</returns>
        public static ISqlGeSource GetSqlSource()
        {
            return _SqlSource;
        }

        /// <summary>
        /// 获取默认的数据库访问框架业务对象。
        /// </summary>
        /// <returns>默认的数据库访问框架业务对象。</returns>
        public static BaseDao GetDao()
        {
            return GetDao(DefaultConnectionName);
        }

        /// <summary>
        /// 获取与指定键相关联的数据库访问框架业务对象。
        /// </summary>
        /// <param name="name">要获取数据库访问框架业务对象的键。</param>
        /// <returns>当此方法返回时，如果找到指定键，则返回与该键相关联的数据库访问框架业务对象；否则，将返回一个空的BaseDao类型对象。</returns>
        public static BaseDao GetDao(string name)
        {
            BaseDao dao;

            _DaosLock.EnterUpgradeableReadLock();
            try
            {
                if (!_Daos.TryGetValue(name, out dao))
                {
                    _DaosLock.EnterWriteLock();
                    try
                    {
                        dao = new DataBaseDao(name, CreateDatabase(name));
                        _Daos.Add(name, dao);
                    }
                    finally
                    {
                        _DaosLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _DaosLock.ExitUpgradeableReadLock();
            }

            return dao;
        }

        /// <summary>
        /// 获取默认数据库连接的企业库对象。
        /// </summary>
        /// <returns>默认数据库连接的企业库对象。</returns>
        public static DataBase GetDatabase()
        {
            return GetDatabase(DefaultConnectionName);
        }

        /// <summary>
        /// 获取与指定键相关联的数据库连接字符串所生成的企业库对象。
        /// </summary>
        /// <param name="name">要获取企业库对象的数据库连接字符串的键。</param>
        /// <returns>当此方法返回时，如果找到指定键，则返回与该键相关联的企业库对象；否则，返回null。</returns>
        public static DataBase GetDatabase(string name)
        {
            return GetDao(name).Database;
        }

        /// <summary>
        /// 创建一个企业库对象。
        /// </summary>
        /// <param name="name">配置文件中映射数据库连接字符串的配置节的键。</param>
        /// <returns>一个企业库对象。</returns>
        private static DataBase CreateDatabase(string name)
        {
            try
            {
                return DataBaseFactory.CreateDatabase(name);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("请检查连接的数据库名称是否存在或者拼写是否正确...", name), e);
            }
        }

        /// <summary>
        /// 初始加载数据源提供程序。
        /// </summary>
        private static void InitializeProviders()
        {
            _Providers = new Dictionary<string, IDaoProvider>();

            //内置的Provider
            DaoProvider.Providers.ForEach(provider => _Providers.Add(provider.ProviderName, provider));
        }
        
        /// <summary>
        /// 获取与指定键相关联的数据源提供程序。
        /// </summary>
        /// <param name="dbProviderName">要获取的数据源提供程序的键。</param>
        /// <returns>数据源提供程序。</returns>
        internal static IDaoProvider GetProvider(string dbProviderName)
        {
            IDaoProvider provider;

            _ProviderLock.EnterUpgradeableReadLock();
            try
            {
                if (!_Providers.TryGetValue(dbProviderName, out provider))
                {
                    provider = _Providers.Values.Single(p => p.IsSupportsDbProvider(dbProviderName));

                    if (null != provider)
                    {
                        _ProviderLock.EnterWriteLock();
                        try
                        {
                            _Providers.Add(dbProviderName, provider);
                        }
                        finally
                        {
                            _ProviderLock.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _ProviderLock.ExitUpgradeableReadLock();
            }

            return provider;
        }
    }
}
