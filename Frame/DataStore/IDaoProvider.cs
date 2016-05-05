using System.Text.RegularExpressions;

namespace Frame.DataStore
{
    /// <summary>
    /// 数据访问源提供者接口。
    /// </summary>
    public interface IDaoProvider
    {
        /// <summary>
        /// Provider的名称，一般此属性直接映射ConnectionString中的ProviderName。
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 返回此Provider是否支持某个DbProvider。
        /// </summary>
        /// <param name="dbProviderName"></param>
        /// <returns></returns>
        bool IsSupportsDbProvider(string dbProviderName);

        /// <summary>
        /// 是否支持命名参数。
        /// </summary>
        bool IsSupportsNamedParameter { get; }

        /// <summary>
        /// 数据访问提供者的命名参数格式，命名参数前缀 + {0}。
        /// </summary>
        /// <remarks>
        /// 例如Sql Server对应的格式为 "@{0}"，Oracle对应的格式为:{0}。
        /// </remarks>
        string NamedParameterFormat { get; }

        /// <summary>
        /// 数据访问提供者的命名参数格式正则表达式。
        /// </summary>
        Regex NamedParameterPattern { get; }

        /// <summary>
        /// 把指定的SQL语句进行自动包装为分页查询的语句。
        /// </summary>
        string WrapPageSql(string sql, string orderClause);

        /// <summary>
        /// 把指定的SQL语句进行自动包装为查询总数的语句。
        /// </summary>
        string WrapCountSql(string sql);

        /// <summary>
        /// 对SQL文本进行转义处理
        /// </summary>
        string EscapeText(string text);

        /// <summary>
        /// 对SQL中的命名参数进行转义处理
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string EscapeParam(string name);
    }
}
