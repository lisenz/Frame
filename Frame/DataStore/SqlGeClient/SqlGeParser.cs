using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//----------
using Frame.DataStore.Provider;
using Frame.DataStore.SqlGeClient.Clauses;

namespace Frame.DataStore.SqlGeClient
{
    public class SqlGeParser
    {

        //匹配#ParamName#或者$ParamName$
        private static readonly Regex ParameterPattern =
            new Regex(@"#(?<NamedParam>.+?)#|\$(?<ValueParam>.+?)\$", RegexOptions.Compiled);

        //匹配{?..}
        private static readonly Regex DynamicClausePattern =
            new Regex(@"\{\?(?<DynamicClause>.+?)\}", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static ISqlGeStatement Parse(string sql)
        {
            return Parse(sql, DaoProvider.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ISqlGeStatement Parse(string sql, IDaoProvider provider)
        {
            try
            {
                return new SqlGeStatement(sql, ParseToClauses(sql, provider));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static IList<SqlGeClause> ParseToClauses(string sql)
        {
            return ParseToClauses(sql, DaoProvider.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        internal static IList<SqlGeClause> ParseToClauses(string sql, IDaoProvider provider)
        {
            sql = sql.Trim();

            IList<SqlGeClause> clauses = new List<SqlGeClause>();

            //解析{? .. }语句
            IEnumerable<Match> matches = FindDynamicClauses(sql);

            //解析#ParamName#和$ParamName$语句(不包含在{?..}语句中的参数)
            matches = FindDynamicParameters(sql, matches);

            //解析数据库本身的命名参数，如SqlServer是@ParamName，Oracle是:ParamName
            matches = FindProviderNamedParameters(sql, provider, matches);

            //如果没有发现任何参数和动态子句则整个sql作为一个TextClause);)
            if (matches.Count() == 0)
            {
                clauses.Add(new TextClause(sql));
            }
            else
            {
                //按Index的升序排序
                matches = matches.OrderBy(match => match.Index);

                int lastIndex = 0;
                foreach (Match match in matches)
                {
                    if (match.Index > lastIndex)
                    {
                        string text = sql.Substring(lastIndex, match.Index - lastIndex);
                        clauses.Add(new TextClause(text));
                    }
                    lastIndex = match.Index + match.Length;

                    Group group;

                    if ((group = match.Groups["Name"]).Success)
                    {
                        clauses.Add(new DbNamedParameterClause(match.Value, group.Value));
                    }
                    else if ((group = match.Groups["NamedParam"]).Success)
                    {
                        clauses.Add(new NamedParameterClause(match.Value, group.Value));
                    }
                    else if ((group = match.Groups["ValueParam"]).Success)
                    {
                        clauses.Add(new ValueParameterClause(match.Value, group.Value));
                    }
                    else if ((group = match.Groups["DynamicClause"]).Success)
                    {
                        clauses.Add(new DynamicClause(match.Value, group.Value));
                    }
                }

                if (lastIndex < sql.Length)
                {
                    clauses.Add(new TextClause(sql.Substring(lastIndex)));
                }
            }

            return clauses;
        }

        private static IEnumerable<Match> FindDynamicClauses(string sql)
        {
            IEnumerable<Match> dynamicClauseMatches = DynamicClausePattern.Matches(sql).Cast<Match>();
            return dynamicClauseMatches;
        }

        private static IEnumerable<Match> FindProviderNamedParameters(string sql, IDaoProvider provider)
        {
            if (provider.IsSupportsNamedParameter)
            {
                IEnumerable<Match> dbNamedParameterMatchs = provider.NamedParameterPattern.Matches(sql).Cast<Match>();
                return dbNamedParameterMatchs;
            }
            return null;
        }

        private static IEnumerable<Match> FindDynamicParameters(string sql, IEnumerable<Match> matched)
        {
            IEnumerable<Match> parameterMatches = from pmatch in ParameterPattern.Matches(sql).Cast<Match>()
                                                  where !matched.Any(match => pmatch.Index >= match.Index &&
                                                                     pmatch.Index < match.Index + match.Length)
                                                  select pmatch;

            if (parameterMatches.Count() > 0)
            {
                return matched.Concat(parameterMatches);
            }
            else
            {
                return matched;
            }
        }

        private static IEnumerable<Match> FindProviderNamedParameters(string sql, IDaoProvider provider, IEnumerable<Match> matched)
        {
            if (provider.IsSupportsNamedParameter)
            {
                IEnumerable<Match> dbNamedParameterMatchs = from pmatch in provider.NamedParameterPattern.Matches(sql).Cast<Match>()
                                                            where !matched.Any(match => pmatch.Index >= match.Index &&
                                                                               pmatch.Index < match.Index + match.Length)
                                                            select pmatch;

                if (dbNamedParameterMatchs.Count() > 0)
                {
                    return matched.Concat(dbNamedParameterMatchs);
                }
            }
            return matched;
        }
    }
}
