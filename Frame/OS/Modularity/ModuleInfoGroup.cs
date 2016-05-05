using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 表示可操作的同时加载的一组ModuleInfo对象。
    /// </summary>
    public class ModuleInfoGroup : IModuleCatalogItem, IList<ModuleInfo>, IList
    {
        private readonly Collection<ModuleInfo> modules = new Collection<ModuleInfo>();

        public InitializationMode InitializationMode { get; set; }
        public string Ref { get; set; }

        public int Count
        {
            get { return this.modules.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)this.modules).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((ICollection)this.modules).SyncRoot; }
        }

        public ModuleInfo this[int index]
        {
            get { return this.modules[index]; }
            set { this.modules[index] = value; }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (ModuleInfo)value; }
        }

        public int IndexOf(object value)
        {
            return this.modules.IndexOf((ModuleInfo)value);
        }
        public int IndexOf(ModuleInfo item)
        {
            return this.modules.IndexOf(item);
        }

        int IList.Add(object value)
        {
            this.Add((ModuleInfo)value);
            return 1;
        }        
        public void Add(ModuleInfo item)
        {
            this.ForwardValues(item);
            this.modules.Add(item);
        }
        protected void ForwardValues(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new System.ArgumentNullException("moduleInfo");
            if (moduleInfo.Ref == null)
            {
                moduleInfo.Ref = this.Ref;
            }

            if (moduleInfo.InitializationMode == InitializationMode.WhenAvailable && this.InitializationMode != InitializationMode.WhenAvailable)
            {
                moduleInfo.InitializationMode = this.InitializationMode;
            }
        }

        public void Insert(int index, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            ModuleInfo moduleInfo = value as ModuleInfo;

            if (moduleInfo == null)
                throw new ArgumentException("该值的数据类型必须是ModuleInfo类型。", "value");

            this.Insert(index, moduleInfo);
        }
        public void Insert(int index, ModuleInfo item)
        {
            this.modules.Insert(index, item);
        }

        void IList.Remove(object value)
        {
            this.Remove((ModuleInfo)value);
        }
        public bool Remove(ModuleInfo item)
        {
            return this.modules.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.modules.RemoveAt(index);
        }

        public void Clear()
        {
            this.modules.Clear();
        }

        bool IList.Contains(object value)
        {
            if (value == null) throw new ArgumentNullException("value");
            ModuleInfo moduleInfo = value as ModuleInfo;

            if (moduleInfo == null)
                throw new ArgumentException("该值的数据类型必须是ModuleInfo类型。", "value");

            return this.Contains(moduleInfo);
        }
        public bool Contains(ModuleInfo item)
        {
            return this.modules.Contains(item);
        }

        public void CopyTo(ModuleInfo[] array, int arrayIndex)
        {
            this.modules.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ModuleInfo> GetEnumerator()
        {
            return this.modules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
              
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)this.modules).CopyTo(array, index);
        }

    }
}
