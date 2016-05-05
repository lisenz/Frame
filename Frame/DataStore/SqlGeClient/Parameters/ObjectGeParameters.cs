using System;
using System.Reflection;
using Frame.Core.Extensions;

namespace Frame.DataStore.SqlGeClient.Parameters
{
    internal class ObjectGeParameters : BaseGeParameters
    {
        private readonly Type _Type;
        private readonly object _Params;

        internal ObjectGeParameters(object parameters)
        {
            this._Type = parameters.GetType();
            this._Params = parameters;
        }

        public override bool TryResolve(string name, out object value)
        {
            PropertyInfo prop = this._Type.GetProperty(name, 
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            if (null != prop)
            {
                value = prop.FastGetValue(this._Params);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
