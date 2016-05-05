using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Frame.OS.Modularity;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 模块列表项集合。
    /// </summary>
    internal class ModuleCatalogItemCollection : Collection<IModuleCatalogItem>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void InsertItem(int index, IModuleCatalogItem item)
        {
            base.InsertItem(index, item);

            this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, eventArgs);
            }
        }
    }
}
