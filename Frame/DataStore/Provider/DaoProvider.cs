using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Frame.DataStore.Provider
{
    /// <summary>
    /// 提供数据访问提供者的属性和方法的基类。
    /// </summary>
    public abstract class DaoProvider : IDaoProvider
    {
        /// <summary>
        /// 默认命名参数正则表达式。
        /// </summary>
        internal const string NameParamPatternString = @"(?<Name>[a-zA-Z0-9_:-]+)";

        /// <summary>
        /// 表示一个Provider列表。
        /// </summary>
        private static readonly IList<IDaoProvider> _Providers;

        /// <summary>
        /// 表示是否支持命名参数。
        /// </summary>
        private bool _IsSupportsNamedParameter = true;

        /// <summary>
        /// 表示数据访问提供者的命名参数格式，命名参数前缀 + {0}。
        /// </summary>
        private string _NamedParameterFormat;

        /// <summary>
        /// 表示Provider的名称，一般此属性直接映射ConnectionString中的ProviderName。
        /// </summary>
        private string _ProviderName;

        /// <summary>
        /// SQL-Server的数据访问提供者。
        /// </summary>
        public static readonly IDaoProvider SqlServer = new SqlServerProvider();

        /// <summary>
        /// SQLite的数据访问提供者。
        /// </summary>
        public static readonly IDaoProvider Sqlite = new SqliteProvider();

        /// <summary>
        /// 获取数据源提供程序集合列表。
        /// </summary>
        public static IList<IDaoProvider> Providers
        {
            get
            {
                return _Providers;
            }
        }

        /// <summary>
        /// 获取默认的数据访问提供者。
        /// </summary>
        public static IDaoProvider Default
        {
            get
            {
                return SqlServer;
            }
        }

        /// <summary>
        /// 获取或设置Provider的名称，一般此属性直接映射ConnectionString中的ProviderName。
        /// </summary>
        public virtual string ProviderName
        {
            get
            {
                return _ProviderName;
            }
            set
            {
                _ProviderName = value;
            }
        }

        /// <summary>
        /// 提供一个值，该值获取或设置是否支持命名参数。
        /// </summary>
        public virtual bool IsSupportsNamedParameter
        {
            get
            {
                return _IsSupportsNamedParameter;
            }
            protected set
            {
                _IsSupportsNamedParameter = value;
            }

        }

        /// <summary>
        /// 获取或设置数据访问提供者的命名参数格式，命名参数前缀 + {0}。
        /// </summary>
        public virtual string NamedParameterFormat
        {
            get
            {
                return _NamedParameterFormat;
            }
            protected set
            {
                _NamedParameterFormat = value;
            }

        }

        /// <summary>
        ///  获取数据访问提供者的命名参数格式正则表达式。
        /// </summary>
        public abstract Regex NamedParameterPattern { get; }

        /// <summary>
        /// 静态构造函数，初始化相关数据源提供程序到列表。
        /// </summary>
        static DaoProvider()
        {
            _Providers = new List<IDaoProvider>();
            _Providers.Add(SqlServer);
            _Providers.Add(Sqlite);
        }

        /// <summary>
        /// 初始化DaoProvider。
        /// </summary>
        protected DaoProvider()
        {
        }

        /// <summary>
        /// 初始化DaoProvider。
        /// </summary>
        /// <param name="providerName">表示Provider的名称，一般此属性直接映射ConnectionString中的ProviderName。</param>
        /// <param name="namedParamterFormat">命名参数格式，命名参数前缀。</param>
        protected DaoProvider(string providerName, string namedParamterFormat)
        {
            this._ProviderName = providerName;
            this._NamedParameterFormat = namedParamterFormat;
        }

        /// <summary>
        /// 初始化DaoProvider。
        /// </summary>
        /// <param name="providerName">表示Provider的名称，一般此属性直接映射ConnectionString中的ProviderName。</param>
        /// <param name="namedParameterFormat">命名参数格式，命名参数前缀。</param>
        /// <param name="isSupportsNamedParameter">表示是否支持命名参数。</param>
        protected DaoProvider(string providerName, string namedParameterFormat, bool isSupportsNamedParameter)
            : this(providerName, namedParameterFormat)
        {
            this._IsSupportsNamedParameter = isSupportsNamedParameter;
        }


        /// <summary>
        /// 返回一个值，该值标识当前数据访问提供者是否支持指定某个DbProvider。
        /// </summary>
        /// <param name="dbProviderName">DbProvider的名称。</param>
        /// <returns>是否支持指定某个DbProvider。</returns>
        public virtual bool IsSupportsDbProvider(string dbProviderName)
        {
            return dbProviderName.Equals(this._ProviderName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 把指定的SQL语句进行自动包装为分页查询的语句。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="orderClause"></param>
        /// <returns></returns>
        public abstract string WrapPageSql(string sql, string orderClause);

        /// <summary>
        /// 把指定的SQL语句进行自动包装为查询总数的语句。
        /// </summary>
        /// <param name="sql">要包装为查询总数的SQL文本。</param>
        /// <returns>包装为查询总数的语句。</returns>
        public virtual string WrapCountSql(string sql)
        {
            sql = sql.Trim();
            int begin = sql.ToLower().LastIndexOf("order by");
            if (begin > 0)
            {
                int end = sql.ToLower().LastIndexOf(")");
                if (begin > end)
                {
                    sql = sql.Substring(0, begin);
                }
            }

            return "select count(1) from (\n" + sql + "\n) tt";
        }

        /// <summary>
        /// 对指定文本进行转义处理。
        /// </summary>
        /// <param name="text">进行转义处理的文本。</param>
        /// <returns>转义处理后的文本。</returns>
        public virtual string EscapeText(string text)
        {
            return null == text ? text : text.Replace("'", "''");
        }

        /// <summary>
        /// 对指定参数名称进行转义处理。
        /// </summary>
        /// <param name="name">参数名称字符串。</param>
        /// <returns>转义后的参数字符串。</returns>
        public string EscapeParam(string name)
        {
            return name.Replace(":", "_").Replace(".", "__");
        }
    }
}
