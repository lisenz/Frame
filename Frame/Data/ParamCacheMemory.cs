using System;
using System.Data;

namespace Frame.Data
{
    internal class ParamCacheMemory : CacheMemory
    {

        /// <summary>
        /// 添加一个参数集合到缓存中。
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串。</param>
        /// <param name="command">执行命令，这里指存储过程名称。</param>
        /// <param name="parameters">参数集合。</param>
        public void AddParametersToCache(string connectionString, IDbCommand command, IDataParameter[] parameters)
        {
            base.AddValueToCache(connectionString, command.CommandText, parameters);
        }

        /// <summary>
        /// 判断缓存中是否已存在指定参数集合。
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串。</param>
        /// <param name="command">执行命令，这里指存储过程名称。</param>
        /// <returns>是否存在参数集合。true表示已有集合，否则表示不存在。</returns>
        public bool IsParametersExistsCache(string connectionString, IDbCommand command)
        {
            return base.IsExistsCache(connectionString, command.CommandText);
        }

        /// <summary>
        /// 从缓存中获取指定的参数集合。
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串。</param>
        /// <param name="command">执行命令，这里指存储过程名称。</param>
        /// <returns>参数集合。</returns>
        public IDataParameter[] GetParametersFromCache(string connectionString, IDbCommand command)
        {
            IDataParameter[] parameters = (IDataParameter[])base.GetValueFromCache(connectionString,command.CommandText);
            return CloneParameters(parameters);
        }

        /// <summary>
        /// 克隆参数集合。
        /// </summary>
        /// <param name="parameters">参数集合。</param>
        /// <returns>克隆的参数集合。</returns>
        public static IDataParameter[] CloneParameters(IDataParameter[] parameters)
        {
            IDataParameter[] cloneParameters = new IDataParameter[parameters.Length];
            for (int i = 0, count = parameters.Length; i < count; i++)
            {
                cloneParameters[i] = (IDataParameter)((ICloneable)parameters[i]).Clone();
            }

            return cloneParameters;
        }
    }
}
