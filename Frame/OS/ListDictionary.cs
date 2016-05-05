using System;
using System.Collections;
using System.Collections.Generic;

namespace Frame.OS
{
    public sealed class ListDictionary<TKey, TValue> : IDictionary<TKey, IList<TValue>>
    {
        private Dictionary<TKey, IList<TValue>> innerValues = new Dictionary<TKey, IList<TValue>>();


        #region 公开方法

        public void Add(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CreateNewList(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)

                throw new ArgumentNullException("key");
            if (value == null)

                throw new ArgumentNullException("value");

            if (innerValues.ContainsKey(key))
            {
                innerValues[key].Add(value);
            }
            else
            {
                List<TValue> values = CreateNewList(key);
                values.Add(value);
            }
        }

        private List<TValue> CreateNewList(TKey key)
        {
            List<TValue> values = new List<TValue>();
            innerValues.Add(key, values);

            return values;
        }

        public void Clear()
        {
            innerValues.Clear();
        }

        public bool ContainsValue(TValue value)
        {
            foreach (KeyValuePair<TKey, IList<TValue>> pair in innerValues)
            {
                if (pair.Value.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return innerValues.ContainsKey(key);
        }

        public IEnumerable<TValue> FindAllValuesByKey(Predicate<TKey> keyFilter)
        {
            foreach (KeyValuePair<TKey, IList<TValue>> pair in this)
            {
                if (keyFilter(pair.Key))
                {
                    foreach (TValue value in pair.Value)
                    {
                        yield return value;
                    }
                }
            }
        }

        public IEnumerable<TValue> FindAllValues(Predicate<TValue> valueFilter)
        {
            foreach (KeyValuePair<TKey, IList<TValue>> pair in this)
            {
                foreach (TValue value in pair.Value)
                {
                    if (valueFilter(value))
                    {
                        yield return value;
                    }
                }
            }
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return innerValues.Remove(key);
        }

        public void Remove(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null)
                throw new ArgumentNullException("value");

            if (innerValues.ContainsKey(key))
            {
                List<TValue> innerList = (List<TValue>)innerValues[key];
                innerList.RemoveAll(delegate(TValue item)
                {
                    return value.Equals(item);
                });
            }
        }

        public void Remove(TValue value)
        {
            foreach (KeyValuePair<TKey, IList<TValue>> pair in innerValues)
            {
                Remove(pair.Key, value);
            }
        }

        #endregion

        #region 属性

        public IList<TValue> Values
        {
            get
            {
                List<TValue> values = new List<TValue>();
                foreach (IEnumerable<TValue> list in innerValues.Values)
                {
                    values.AddRange(list);
                }

                return values;
            }
        }

        public ICollection<TKey> Keys
        {
            get { return innerValues.Keys; }
        }

        public IList<TValue> this[TKey key]
        {
            get
            {
                if (innerValues.ContainsKey(key) == false)
                {
                    innerValues.Add(key, new List<TValue>());
                }
                return innerValues[key];
            }
            set { innerValues[key] = value; }
        }

        public int Count
        {
            get { return innerValues.Count; }
        }

        #endregion

        #region IDictionary<TKey,List<TValue>>字段

        void IDictionary<TKey, IList<TValue>>.Add(TKey key, IList<TValue> value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null)
                throw new ArgumentNullException("value");

            innerValues.Add(key, value);
        }

        bool IDictionary<TKey, IList<TValue>>.TryGetValue(TKey key, out IList<TValue> value)
        {
            value = this[key];
            return true;
        }

        ICollection<IList<TValue>> IDictionary<TKey, IList<TValue>>.Values
        {
            get { return innerValues.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,List<TValue>>>字段

        void ICollection<KeyValuePair<TKey, IList<TValue>>>.Add(KeyValuePair<TKey, IList<TValue>> item)
        {
            ((ICollection<KeyValuePair<TKey, IList<TValue>>>)innerValues).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, IList<TValue>>>.Contains(KeyValuePair<TKey, IList<TValue>> item)
        {
            return ((ICollection<KeyValuePair<TKey, IList<TValue>>>)innerValues).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, IList<TValue>>>.CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, IList<TValue>>>)innerValues).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, IList<TValue>>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, IList<TValue>>>)innerValues).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<TKey, IList<TValue>>>.Remove(KeyValuePair<TKey, IList<TValue>> item)
        {
            return ((ICollection<KeyValuePair<TKey, IList<TValue>>>)innerValues).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,List<TValue>>>字段

        IEnumerator<KeyValuePair<TKey, IList<TValue>>> IEnumerable<KeyValuePair<TKey, IList<TValue>>>.GetEnumerator()
        {
            return innerValues.GetEnumerator();
        }

        #endregion

        #region IEnumerable字段

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerValues.GetEnumerator();
        }

        #endregion
    }
}
