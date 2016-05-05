using System;
using System.Collections.Generic;
using Frame.DataStore.SqlGeClient.Parameters;

namespace Frame.DataStore.SqlGeClient
{
    internal sealed class ParametersWrapper : BaseGeParameters
    {
        private IDictionary<string, object> _Items;
        private readonly ISqlGeParameters _Params;

        internal ParametersWrapper(ISqlGeParameters parameters)
        {
            this._Params = parameters;
        }

        internal void Add(string name, object value)
        {
            if (null == this._Items)
                this._Items = new Dictionary<string, object>();
            this._Items[name] = value;
        }

        public override bool TryResolve(string name, out object value)
        {
            if (null != this._Items && this._Items.TryGetValue(name, out value))
                return true;
            else if (null != this._Params && this._Params.TryResolve(name, out value))
                return true;

            value = null;
            return false;
        }
    }
}
