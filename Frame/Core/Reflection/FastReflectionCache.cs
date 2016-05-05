using System;
using System.Collections.Generic;
//---------------
using System.Threading;

namespace Frame.Core.Reflection
{
    public abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue>
    {
        /// <summary>
        /// 表示缓存反射对象的字典列表。
        /// </summary>
        private readonly Dictionary<TKey, TValue> _Cache;

        /// <summary>
        /// 表示一个同步锁。
        /// </summary>
        private readonly ReaderWriterLockSlim _Lock;

        /// <summary>
        /// 提供快速反射器的缓存控制的方法的基类。
        /// </summary>
        protected FastReflectionCache()
        {
            this._Cache = new Dictionary<TKey, TValue>();
            this._Lock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// 创建指定参数类型的访问器。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract TValue Create(TKey key);

        /// <summary>
        /// 获取与指定类型的键值相关联的值。
        /// </summary>
        /// <param name="key">要获取的值的指定类型的键值。</param>
        /// <returns>如果找到该键，便会返回与指定的键相关联的值；否则，则会返回 TValue 的类型默认值。</returns>
        public TValue Get(TKey key)
        {
            TValue local = default(TValue);
            this._Lock.EnterReadLock();
            bool flag = this._Cache.TryGetValue(key, out local);
            this._Lock.ExitReadLock();

            if (!flag)
            {
                this._Lock.EnterWriteLock();
                try
                {
                    local = this.Create(key);
                    this._Cache[key] = local;
                }
                finally
                {
                    this._Lock.ExitWriteLock();
                }
            }

            return local;
        }

        public bool ContainsKey(TKey key)
        {
            return this._Cache.ContainsKey(key);
        }

        /// <summary>
        /// 从缓存中移除指定键对应的应用程序域。
        /// </summary>
        /// <param name="key">要移除的应用程序域的键。</param>
        public void Remove(TKey key)
        {
            TValue local = default(TValue);
            this._Lock.EnterReadLock();
            bool flag = this._Cache.TryGetValue(key, out local);
            this._Lock.ExitReadLock();

            if (flag)
                this._Cache.Remove(key);
        }
    }
}
