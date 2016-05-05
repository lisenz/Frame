using System;
using System.Reflection;
using System.Collections.Specialized;

namespace Frame.Core.Collection
{
    public class NameObjectCollection<T> : NameObjectCollectionBase
    {
        public void Add(string name, T value)
        {
            base.BaseAdd(name, value);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public T this[string name]
        {
            get
            {
                return (T)base.BaseGet(name);
            }
            set
            {
                base.BaseSet(name, value);
            }
        }
    }
}
