using System;
using System.Web;
using System.Collections.Generic;

namespace Frame.Core.Threading
{
    /// <summary>
    /// 获取和设置可用于在HTTP请求过程中在IHttpModule和IHttpHandler接口之间组织和共享的数据。
    /// </summary>
    /// <typeparam name="T">组织和共享的数据的数据类型。</typeparam>
    public sealed class ThreadContext<T> : IDisposable
    {
        [ThreadStatic]
        private static IDictionary<object, T> _Items;
        private static bool _IsAspNet;
        
        private object _Key;
        private Func<T> _ValueFactory;

        static ThreadContext()
        {
            ThreadContext<T>._IsAspNet = (HttpContext.Current != null);
        }

        public ThreadContext()
            : this(null)
        {
        }

        public ThreadContext(Func<T> valueFactory)
        {
            this._Key = ThreadContext<T>._IsAspNet ? new object() : this;
            //this._Key = new object();
            this._ValueFactory = valueFactory;
        }

        private static IDictionary<object, T> Items
        {
            get
            {
                return (ThreadContext<T>._Items ?? (ThreadContext<T>._Items = new Dictionary<object, T>()));
            }
        }

        public T ContextValue
        {
            get
            {
                T local;
                if (ThreadContext<T>._IsAspNet)
                {
                    local = (T)HttpContext.Current.Items[this._Key];
                }
                else
                {
                    ThreadContext<T>.Items.TryGetValue(this._Key, out local);
                }

                if ((null == local) && (null != this._ValueFactory))
                {
                    local = this._ValueFactory();
                    if (ThreadContext<T>._IsAspNet)
                    {
                        HttpContext.Current.Items[this._Key] = local;
                        return local;
                    }
                    ThreadContext<T>.Items[this._Key] = local;
                }
                return local;
            }
            set
            {
                if (ThreadContext<T>._IsAspNet)
                {
                    HttpContext.Current.Items[this._Key] = value;
                }
                else
                {
                    ThreadContext<T>.Items[this._Key] = value;
                }
            }
        }

        public void Dispose()
        {
            if (null != ThreadContext<T>._Items)
            {
                ThreadContext<T>._Items.Clear();
                ThreadContext<T>._Items = null;
            }
            if ((null != HttpContext.Current) && (null != this._Key))
            {
                HttpContext.Current.Items.Remove(this._Key);
            }
        }
    }
}
