using System;
using System.Data;
using System.Collections;

namespace Frame.Data
{
    public class CacheMemory
    {
        /// <summary>
        /// 存储指定存储过程的参数集合的缓存器。
        /// 注意：这里使用Hashtable.Synchronized创建缓存的自动线程同步的实例对象，
        ///       使用这种方式创建的Hashtable对象允许并发读但只能一个线程进行写。
        /// </summary>
        private Hashtable _ParamCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 创建存储器的键名。
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串。</param>
        /// <param name="procName">存储过程名称。</param>
        /// <returns>组合的键名。</returns>
        private static string CreateCacheKey(string connectionString, string procName)
        {
            return string.Format("{0}:{1}", connectionString, procName);
        }

        /// <summary>
        /// 添加一个参数集合到缓存中。
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串。</param>
        /// <param name="command">执行命令，这里指存储过程名称。</param>
        /// <param name="parameters">参数集合。</param>
        public void AddValueToCache(string connectionString, string commandText, object value)
        {
            string key = CreateCacheKey(connectionString, commandText);
            this._ParamCache[key] = value;
        }

        /// <summary>
        /// 判断缓存中是否已存在指定参数集合。
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串。</param>
        /// <param name="command">执行命令，这里指存储过程名称。</param>
        /// <returns>是否存在参数集合。true表示已有集合，否则表示不存在。</returns>
        public bool IsExistsCache(string connectionString, string commandText)
        {
            string key = CreateCacheKey(connectionString, commandText);
            return (this._ParamCache[key] != null);
        }

        /// <summary>
        /// 从缓存中获取指定的参数集合。
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串。</param>
        /// <param name="command">执行命令，这里指存储过程名称。</param>
        /// <returns>参数集合。</returns>
        public object GetValueFromCache(string connectionString, string commandText)
        {
            string key = CreateCacheKey(connectionString, commandText);
            return this._ParamCache[key];
        }

        /// <summary>
        /// 清空缓存。
        /// </summary>
        public void Clear()
        {
            this._ParamCache.Clear();
        }
    }
}
