using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.DataStore.SqlGeClient.Parameters
{
    /// <summary>
    /// 提供操作数据源参数的方法基类。
    /// </summary>
    internal abstract class BaseGeParameters : ISqlGeParameters
    {
        /// <summary>
        /// 解析获取指定名称的参数值。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <returns>参数值。</returns>
        public object Resolve(string name)
        {
            object value;
            return TryResolve(name, out value) ? value : null;
        }

        /// <summary>
        /// 解析与指定的参数名称相关联的值。
        /// </summary>
        /// <param name="name">要获取的值的参数名称。</param>
        /// <param name="value">当此方法返回值时，如果找到该名称，便会返回与指定的名称相关联的值；否则，则会返回 value 参数的类型默认值。该参数未经初始化即被传递。</param>
        /// <returns>如果包含具有指定名称的元素，则为 true；否则为false。</returns>
        public abstract bool TryResolve(string name, out object value);
    }
}
