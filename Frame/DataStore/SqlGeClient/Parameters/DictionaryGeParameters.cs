using System;
using System.Collections;

namespace Frame.DataStore.SqlGeClient.Parameters
{
    /// <summary>
    /// 表示IDictionary类型数据源参数。
    /// </summary>
    internal class DictionaryGeParameters : BaseGeParameters
    {
        private readonly IDictionary _Params;

        internal DictionaryGeParameters(IDictionary parameters)
        {
            this._Params = parameters;
        }

        public override bool TryResolve(string name, out object value)
        {
            return (null != (value = this._Params[name]));
        }
    }
}
