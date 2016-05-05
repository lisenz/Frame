using System;
using System.Collections.Generic;

namespace Frame.DataStore.SqlGeClient.Parameters
{
    /// <summary>
    /// 表示IDictionary泛型类型数据源参数。
    /// </summary>
    internal sealed class GenericGeParameters : BaseGeParameters
    {
        private readonly IDictionary<string, object> _Params;

        internal GenericGeParameters(IDictionary<string, object> parameters)
        {
            this._Params = parameters;
        }

        public override bool TryResolve(string name, out object value)
        {
            return this._Params.TryGetValue(name, out value);
        }
    }
}
