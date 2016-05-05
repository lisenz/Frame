using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 在指定泛型类型的集合上进行简单迭代，并将循环获取的集合项作为参数传入委托封装的方法。
        /// </summary>
        /// <typeparam name="T">要枚举的对象的类型。</typeparam>
        /// <param name="enumerable">要进行循环访问的集合对象的枚举器。</param>
        /// <param name="action">一个委托对象。</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T local in enumerable)
            {
                action(local);
            }
        }
    }
}
