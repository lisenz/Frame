using System;
using Frame.DataStore;

namespace Frame.Service.Server.SqlGe
{
    /// <summary>
    /// 提供解析SqlCommand的方法。
    /// </summary>
    public class SqlCommandResolver : IServiceCommandResolver
    {
        /// <summary>
        /// 默认文本命令前缀字符串。
        /// </summary>
        public const string DefaultPrefix = "sqlid:";

        /// <summary>
        /// 默认文本命令前缀字符串。
        /// </summary>
        private string _prefix = DefaultPrefix;

        /// <summary>
        /// 获取或设置默认文本命令前缀字符串。
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        /// <summary>
        /// 解析SqlCommand。
        /// </summary>
        /// <param name="name">执行文本命令字符串。</param>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>返回解析构造的SqlCommand对象。若执行文本命令name中没有加入指定前缀，则返回null。</returns>
        public IServiceCommand Resolve(string name, IServiceContext context)
        {
            if (name.StartsWith(Prefix))
            {
                string sqlid = name.Substring(Prefix.Length);

                ISqlGeStatement statement = DaoFactory.GetSqlSource().Find(sqlid);

                if (null == statement)
                {
                    throw ServiceException.NotFound(string.Format("无法检索到sqlid '{0}'", sqlid));
                }

                return new SqlCommand(statement);
            }
            return null;
        }
    }
}
