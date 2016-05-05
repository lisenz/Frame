using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//---------
using Frame.Core;

namespace Frame.DataStore.SqlGeClient.Clauses
{
    /// <summary>
    /// 表示一个数据库参数类型的SQL子句，格式为$ParamName$。
    /// </summary>
    internal class ValueParameterClause : ParamClause
    {
        private readonly string defaultValue;

        /// <summary>
        /// 构造函数，初始化子句结构信息。
        /// </summary>
        /// <param name="rawText">通过匹配捕获的实际子字符串。</param>
        /// <param name="name">通过匹配捕获的子字符串。</param>
        public ValueParameterClause(string rawText, string name)
            : base(rawText, name)
        {
            int index = name.IndexOf('?');
            if (index > 0)
            {
                ParamName = name.Substring(0, index).Trim();
                defaultValue = name.Length > index + 1 ? name.Substring(index + 1).Trim() : null;
            }
        }

        /// <summary>
        /// 解析子句并生成指定的命令。
        /// </summary>
        /// <param name="provider">数据源提供程序。</param>
        /// <param name="builder">SqlGeCommand建造器。</param>
        /// <param name="parameters">参数对象。</param>
        public override void ToCommand(IDaoProvider provider, SqlGeCommandBuilder builder, ISqlGeParameters parameters)
        {
            object value = parameters.Resolve(ParamName);
            string content = ToSqlString(provider, value);

            if (!string.IsNullOrEmpty(defaultValue) && string.IsNullOrEmpty(content))
            {
                builder.AppendCommandText(defaultValue);
            }
            else
            {
                builder.AppendCommandText(content);
            }
        }

        /// <summary>
        /// 对子句的参数值进行解析。
        /// </summary>
        /// <param name="provider">数据源提供程序。</param>
        /// <param name="value">参数值。</param>
        /// <returns>解析后的符合规则的参数值。</returns>
        private static string ToSqlString(IDaoProvider provider, object value)
        {
            if (null == value || value is DBNull)
            {
                return String.Empty;
            }
            else if (value is string)
            {
                return provider.EscapeText(((string)value).Trim());
            }
            else if (value is IEnumerable)
            {
                bool? quoted = null;
                StringBuilder builder = new StringBuilder();
                foreach (object single in ((IEnumerable)value))
                {
                    if (!quoted.HasValue)
                    {
                        quoted = IsQuoted(single.GetType());
                    }

                    builder.Append(",");

                    if (quoted.Value)
                    {
                        builder.Append("'");
                    }

                    builder.Append(ToSqlString(provider, single));

                    if (quoted.Value)
                    {
                        builder.Append("'");
                    }
                }
                return builder.Length > 0 ? builder.Remove(0, 1).ToString() : string.Empty;
            }
            else
            {
                return provider.EscapeText(Convertor.Convert<string>(value).Trim());
            }
        }

        /// <summary>
        /// 判断该类型的值是否需要加单引号。
        /// </summary>
        /// <param name="type">参数值的类型。</param>
        /// <returns>返回一个值，该标识是否需要加单引号。</returns>
        private static bool IsQuoted(Type type)
        {
            //非数字，布尔值之外的都需要加单引号
            return typeof(char).Equals(type) || !type.IsPrimitive;
        }

    }
}
