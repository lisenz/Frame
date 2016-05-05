using System;
using System.Text;
using System.Text.RegularExpressions;
//-----------------
using Frame.Core.Extensions;

namespace Frame.DataStore.Provider
{
    /// <summary>
    /// 表示SQLite的数据访问提供者。
    /// </summary>
    public class SqliteProvider : DaoProvider
    {
        private const string _ProviderName = "Sqlite";
        private const string _NamedParameterFormat = "@{0}";
        private const string _DbProvider = "System.Data.SQLite";
        private static readonly Regex _NamedParameterPattern = new Regex("@" + NameParamPatternString, RegexOptions.Compiled);

        /// <summary>
        /// 表示SQLite的数据访问提供者。
        /// </summary>
        public SqliteProvider()
            : base(_ProviderName, _NamedParameterFormat)
        {
        }

        /// <summary>
        /// 获取SQLite的命名参数格式正则表达式。
        /// </summary>
        public override Regex NamedParameterPattern
        {
            get { return _NamedParameterPattern; }
        }

        /// <summary>
        /// 返回一个值，该值标识当前数据访问提供者是否支持System.Data.SQLite。
        /// </summary>
        /// <param name="dbProviderName">DbProvider的名称。</param>
        /// <returns>是否支持System.Data.SQLite。</returns>
        public override bool IsSupportsDbProvider(string dbProviderName)
        {
            if (_DbProvider.EqualsIgnoreCase(dbProviderName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 把指定的SQL语句进行自动包装为分页查询的语句。
        /// </summary>
        /// <param name="sql">要进行包装分页查询的SQL语句。</param>
        /// <param name="orderClause">排序需要的字段列表，以“，”隔开。</param>
        /// <returns>返回包装完毕的分页查询语句。</returns>
        public override string WrapPageSql(string sql, string orderClause)
        {
            sql = sql.Trim();
            StringBuilder pagingSelect = new StringBuilder(sql.Length + 100);
            if (!string.IsNullOrEmpty(orderClause))
            {
                orderClause = " order by " + orderClause;
            }

            //select * from users order by id limit 10 offset 0 
            //offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
            pagingSelect.Append(sql).Append(orderClause).Append(" limit #__RowEnd__# ").Append("offset #__RowBegin__#");
            return pagingSelect.ToString();
        }
    }
}
