using System;
using System.Data;
using System.Data.Common;

namespace Frame.Data
{
    internal class ParameterCache
    {
        /// <summary>
        /// 表示一个管理数据参数的缓存。
        /// </summary>
        private ParamCacheMemory _Cache = new ParamCacheMemory();


        /// <summary>
        /// 设置参数信息。
        /// </summary>
        /// <param name="command">表示要对数据源执行的 SQL 语句或存储过程的命令对象。</param>
        /// <param name="db">指定的数据库访问对象。</param>
        public void SetParameters(DbCommand command, DataBase db)
        {
            if (IsAlreadyCache(command, db))
            {
                AddParameterFormCache(command, db);
            }
            else
            {
                db.DiscoverParameters(command);
                IDataParameter[] copyOfParameters = CreateParameterCopy(command);

                this._Cache.AddParametersToCache(db.ConnectionString, command, copyOfParameters);
            }
        }

        /// <summary>
        /// 将指定Command命令对象中的参数从映射集合中复制到缓存管理器中。
        /// </summary>
        /// <param name="command">表示要对数据源执行的 SQL 语句或存储过程的命令对象。</param>
        /// <returns>返回一个参数集合数组。</returns>
        private static IDataParameter[] CreateParameterCopy(DbCommand command)
        {
            IDataParameterCollection parameters = command.Parameters;
            IDataParameter[] parameterArray = new IDataParameter[parameters.Count];
            parameters.CopyTo(parameterArray, 0);

            return ParamCacheMemory.CloneParameters(parameterArray);
        }

        /// <summary>
        /// 从缓存中获取指定参数集合添加到DbCommand对象中。
        /// </summary>
        /// <param name="command">执行操作对象。</param>
        /// <param name="db">数据库对象。</param>
        protected virtual void AddParameterFormCache(IDbCommand command, DataBase db)
        {
            IDataParameter[] parameters = this._Cache.GetParametersFromCache(db.ConnectionString, command);
            foreach (IDataParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// 判断缓存器中是否存在对应的参数缓存集合。
        /// </summary>
        /// <param name="command">执行命令的操作对象。</param>
        /// <param name="db">数据库对象。</param>
        /// <returns>是否存在参数缓存。</returns>
        private bool IsAlreadyCache(IDbCommand command, DataBase db)
        {
            return this._Cache.IsParametersExistsCache(db.ConnectionString, command);
        }

        /// <summary>
        /// 清空缓存。
        /// </summary>
        protected internal void Clear()
        {
            this._Cache.Clear();
        }
    }
}
