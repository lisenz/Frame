using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Core.Session
{
    public class SessionState : ISessionState, IDisposable
    {
        private IDictionary<string, object> _items = new Dictionary<string, object>();

        public bool Remove(string name)
        {
            lock (this)
            {
                return this._items.Remove(name);
            }
        }

        public object this[string name]
        {
            get
            {
                lock (this)
                {
                    object obj;
                    return (this._items.TryGetValue(name, out obj) ? obj : null);
                }
            }
            set
            {
                lock (this)
                {
                    this._items[name] = value;
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                this._items.Clear();
            }
        }
    }
}
