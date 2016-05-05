using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.Collections.Generic;
//-------
using Frame.Core;

namespace Frame.DataStore.SqlGeClient
{
    /// <summary>
    /// 表示SQL文本或存储过程的语句源。
    /// </summary>
    public class SqlGeSource : ISqlGeSource, IDisposable
    {

        #region 字段

        /// <summary>
        /// 表示语句源的文本文件后缀。
        /// </summary>
        private const string CONFIG_FILTER = "*.config";

        /// <summary>
        /// 表示存放语句源文本文件的文件夹的键。
        /// </summary>
        private const string SQL_DIRECTORY_CONFIG_KEY = "Frame.SqlCommandsDirectory";

        /// <summary>
        /// 表示存放语句源文本文件的文件夹名称。
        /// </summary>
        private const string SQL_DIRECTORY_NAME = "SqlCommands";

        /// <summary>
        /// 表示语句源的映射字典。
        /// </summary>
        private Dictionary<string, ISqlGeStatement> _sqlsMap;

        /// <summary>
        /// 表示config文件的监控对象。
        /// </summary>
        private FileSystemWatcher _configWatcher;

        /// <summary>
        /// 表示存放SQL语句源文件的文件夹目录路径。
        /// </summary>
        private string _sqlsDir;

        /// <summary>
        /// 表示用于管理语句源字典访问的锁定状态。
        /// </summary>
        private readonly ReaderWriterLockSlim _sqlsMapLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 标识是否已经将语句源从config文件中加载到语句源的映射字典中。
        /// </summary>
        private bool _loaded = false;

        /// <summary>
        /// 标识是否正在将语句源从config文件中加载到语句源的映射字典中。
        /// </summary>
        private bool _loading = false;
        private bool _disposed = false;

        /// <summary>
        /// 标识用于管理读取config文件操作时的锁定状态。
        /// </summary>
        private readonly object _readloadSyncRoot = new object();

        /// <summary>
        /// 标识用于管理释放config文件操作监控对象操作时的锁定状态。
        /// </summary>
        private readonly object _disposeSyncRoot = new object();

        #endregion

        #region 构造函数

        /// <summary>
        /// 表示SQL文本或存储过程的语句源。
        /// </summary>
        public SqlGeSource()
        {
            _sqlsMap = new Dictionary<string, ISqlGeStatement>();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取存放SQL语句源文件的文件夹目录路径。
        /// </summary>
        public string SqlsDirectory
        {
            get { return _sqlsDir; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 判断Sql语句的唯一Key是否有效
        /// </summary>
        /// <remarks>
        /// 默认规则：key不能包含空格、逗号、括号
        /// </remarks>
        public bool IsValidKey(string key)
        {
            return null != key && key.Trim().IndexOf(' ') <= 0 && key.IndexOf(',') < 0 && key.IndexOf('(') < 0;
        }

        /// <summary>
        /// 添加一个SQL语句对象。
        /// </summary>
        /// <param name="key">在SQL语句映射字典中的键。</param>
        /// <param name="statement">要添加的SQL语句对象。</param>
        /// <param name="overwrite">表示是否覆盖原有对象写入字典中。</param>
        /// <returns>返回一个值，该值标识是否添加成功。</returns>
        public bool Add(string key, ISqlGeStatement statement, bool overwrite = true)
        {
            if (overwrite)
            {
                _sqlsMapLock.EnterWriteLock();
                try
                {
                    _sqlsMap[key] = statement;
                    return true;
                }
                finally
                {
                    _sqlsMapLock.ExitWriteLock();
                }

            }
            else
            {
                _sqlsMapLock.EnterUpgradeableReadLock();
                try
                {
                    if (_sqlsMap.ContainsKey(key))
                    {
                        return false;
                    }
                    else
                    {
                        _sqlsMapLock.EnterWriteLock();
                        try
                        {
                            _sqlsMap[key] = statement;
                            return true;
                        }
                        finally
                        {
                            _sqlsMapLock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _sqlsMapLock.ExitUpgradeableReadLock();
                }
            }
        }

        /// <summary>
        /// 根据唯一Key查找返回一个经过Parse的SQL语句
        /// </summary>
        /// <param name="key">通过IsValidKey检查的唯一标识一条SQL语句的字符串</param>
        /// <returns>没有对应的语句返回null</returns>
        public ISqlGeStatement Find(string key)
        {
            if (!_loaded)
            {
                throw new InvalidOperationException(string.Format("该存放SQL语句源文本文件的文件夹 '{0}' 不存在!", SQL_DIRECTORY_NAME));
            }

            ISqlGeStatement sql = null;
            _sqlsMapLock.EnterReadLock();
            try
            {
                _sqlsMap.TryGetValue(key, out sql);
            }
            finally
            {
                _sqlsMapLock.ExitReadLock();
            }
            return sql;
        }

        /// <summary>
        /// 获取存放SQL语句源的文件夹目录路径。
        /// </summary>
        /// <returns>存放SQL语句源的文件夹目录路径。</returns>
        private static string GetSqlsDirectory()
        {
            DirectoryInfo dir;

            if (!(AppUtility.FindDirectoryOrConfig(SQL_DIRECTORY_CONFIG_KEY, SQL_DIRECTORY_NAME, out dir) ||
                 AppUtility.FindConfigDirectory(SQL_DIRECTORY_NAME, out dir)))
            {
                return null;
            }

            return dir.FullName;
        }

        /// <summary>
        /// 装载SQL语句源。
        /// </summary>
        /// <returns>返回已装载SQL语句源完毕的语句源对象。</returns>
        public SqlGeSource LoadSqls()
        {
            _sqlsDir = GetSqlsDirectory();

            if (null != _sqlsDir)
            {
                LoadSqlStatements();

                _loaded = true;

                //监控文件变化
                _configWatcher = new FileSystemWatcher(_sqlsDir, CONFIG_FILTER)
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true,
                };

                _configWatcher.Changed += new FileSystemEventHandler(OnChanged);
                _configWatcher.Created += new FileSystemEventHandler(OnChanged);
                _configWatcher.Deleted += new FileSystemEventHandler(OnChanged);
                _configWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            }

            return this;
        }

        private void LoadSqlStatements()
        {
            lock (_readloadSyncRoot)
            {
                if (_loading)
                {
                    return;
                }
                _loading = true;
            }

            string[] files = Directory.GetFiles(_sqlsDir, CONFIG_FILTER, SearchOption.AllDirectories);

            _sqlsMapLock.EnterWriteLock();
            try
            {
                _sqlsMap.Clear();
                foreach (string file in files)
                {
                    using (var txtReader = new StreamReader(File.OpenRead(file)))
                    {
                        var root = XElement.Load(txtReader);
                        ParseSqlsFromXml(file, root);
                    }
                }
            }
            finally
            {
                _sqlsMapLock.ExitWriteLock();
                lock (_readloadSyncRoot)
                {
                    _loading = false;
                }
            }
        }

        private void ParseSqlsFromXml(string file, XElement root)
        {
            foreach (XElement element in root.Elements())
            {
                XAttribute attribute = element.Attribute("key");

                if (null == attribute || string.IsNullOrEmpty(attribute.Value))
                {
                    throw new InvalidOperationException(string.Format("在文件'{0}'中必须要有一个属性值'key'!", file));
                }

                string key = attribute.Value;

                if (_sqlsMap.ContainsKey(key))
                {
                    throw new InvalidOperationException(string.Format("在文件{1}中发现存在重复键{0}!", key, file));
                }

                ISqlGeStatement statement = SqlGeParser.Parse(element.Value);

                XAttribute connection = element.Attribute("connection");
                if (null != connection)
                {
                    statement.Connection = connection.Value;
                }

                _sqlsMap[key] = statement;
            }
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            lock (_disposeSyncRoot)
            {
                if (_disposed)
                {
                    return;
                }
            }

            if (null != _configWatcher)
            {
                _configWatcher.Dispose();
            }
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~SqlGeSource()
        {
            this.Dispose();
        }

        #endregion

        #region 事件

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            LoadSqlStatements();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            LoadSqlStatements();
        }

        #endregion
    }
}
