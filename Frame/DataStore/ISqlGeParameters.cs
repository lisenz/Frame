using System;

namespace Frame.DataStore
{
    /// <summary>
    /// 提供操作数据参数的方法。
    /// </summary>
    public interface ISqlGeParameters
    {
        /// <summary>
        /// 解析获取指定名称的参数值。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <returns>参数值。</returns>
        object Resolve(string name);

        /// <summary>
        /// 解析获取与指定的参数名称相关联的值。
        /// </summary>
        /// <param name="name">要获取的值的参数名称。</param>
        /// <param name="value">当此方法返回值时，如果找到该名称，便会返回与指定的名称相关联的值；否则，则会返回 value 参数的类型默认值。该参数未经初始化即被传递。</param>
        /// <returns>如果包含具有指定名称的元素，则为 true；否则为false。</returns>
        bool TryResolve(string name, out object value);
    }
}
