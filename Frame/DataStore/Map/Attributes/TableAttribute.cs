using System;

namespace Frame.DataStore.Map.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : NamedAttribute
    {
        public TableAttribute(string name)
            : base(name)
        {

        }
    }
}
