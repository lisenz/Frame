using System;
using System.Collections;
using System.Collections.Generic;

namespace Frame.DataStore.SqlGeClient.Parameters
{
    /// <summary>
    /// 表示执行SQL命令所需的参数对象。
    /// </summary>
    internal class SqlGeParameters : BaseGeParameters
    {
        private readonly ISqlGeParameters _Params;

        /// <summary>
        /// 构造函数，初始化参数对象。
        /// </summary>
        /// <param name="parameters">执行SQL命令所需的参数。</param>
        public SqlGeParameters(object parameters)
        {
            this._Params = GetParameters(parameters);
        }

        /// <summary>
        /// 获取相关联类型的参数对象。
        /// </summary>
        /// <param name="parameters">执行SQL命令所需的参数对象。</param>
        /// <returns>返回参数对象映射的类型的参数对象。</returns>
        internal static ISqlGeParameters GetParameters(object parameters)
        {
            if (null == parameters)
                return null;
            else if (parameters is ISqlGeParameters)
                return (ISqlGeParameters)parameters;
            else if (parameters is IDictionary<string, object>)
                return new GenericGeParameters((IDictionary<string, object>)parameters);
            else if (parameters is IDictionary)
                return new DictionaryGeParameters((IDictionary)parameters);
            else
            {
                Type type = parameters.GetType();
                if (!parameters.GetType().IsPrimitive && !parameters.GetType().IsValueType)
                    return new ObjectGeParameters(parameters);
                else
                    throw new InvalidOperationException(
                        string.Format("该参数类型不提供支持 : '{0}'", type.FullName));
            }
        }

        /// <summary>
        /// 解析获取与指定的参数名称相关联的值。
        /// </summary>
        /// <param name="name">要获取的值的参数名称。</param>
        /// <param name="value">当此方法返回值时，如果找到该名称，便会返回与指定的名称相关联的值；否则，则会返回 value 参数的类型默认值。该参数未经初始化即被传递。</param>
        /// <returns>如果包含具有指定名称的元素，则为 true；否则为false。</returns>
        public override bool TryResolve(string name, out object value)
        {
            bool resolved = false;
            if (null != this._Params)
                resolved = this._Params.TryResolve(name, out value);
            else
                value = null;
            return resolved;
        }
    }
}
