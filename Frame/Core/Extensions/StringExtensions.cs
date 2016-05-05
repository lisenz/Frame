using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 确定此字符串是否与指定的 System.String 对象具有相同的值。
        /// </summary>
        /// <param name="str">源字符串实例。</param>
        /// <param name="compareTo">要与此实例进行比较的字符串。</param>
        /// <returns>如果 compareTo 参数的值与此字符串相同，则为 true；否则为 false。</returns>
        public static bool EqualsIgnoreCase(this string str, string compareTo)
        {
            return str.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
