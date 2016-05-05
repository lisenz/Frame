using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Frame.OS.WPF.Regions
{
    public class ViewsCollection : IViewsCollection
    {
        private Comparison<object> _Sort;
        private List<object> _FilteredItems = new List<object>();
        private readonly Predicate<ItemMetadata> _Filter;
        private readonly ObservableCollection<ItemMetadata> _SubjectCollection;
        private readonly Dictionary<ItemMetadata, MonitorInfo> _MonitoredItems = 
            new Dictionary<ItemMetadata, MonitorInfo>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ViewsCollection(ObservableCollection<ItemMetadata> list, Predicate<ItemMetadata> filter)
        {
            this._SubjectCollection = list;
            this._Filter = filter;
            this.MonitorAllMetadataItems();
            this._SubjectCollection.CollectionChanged += this.SourceCollectionChanged;
            this.UpdateFilteredItemsList();
        }

        public Comparison<object> SortComparison
        {
            get { return this._Sort; }
            set
            {
                if (value != this._Sort)
                {
                    this._Sort = value;
                    this.UpdateFilteredItemsList();
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        private IEnumerable<object> FilteredItems
        {
            get { return this._FilteredItems; }
        }

        private void NotifyAdd(object item)
        {
            int newIndex = this._FilteredItems.IndexOf(item);
            this.NotifyAdd(new[] { item }, newIndex);
        }
        private void NotifyAdd(IList items, int newStartingIndex)
        {
            if (items.Count > 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, 
                    items, 
                    newStartingIndex));
            }
        }

        private void NotifyRemove(IList items, int originalIndex)
        {
            if (items.Count > 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    items,
                    originalIndex));
            }
        }

        private void NotifyReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        private void MonitorAllMetadataItems()
        {
            foreach (var item in this._SubjectCollection)
            {
                this.AddMetadataMonitor(item, this._Filter(item));
            }
        }

        private void AddMetadataMonitor(ItemMetadata itemMetadata, bool isInList)
        {
            itemMetadata.MetadataChanged += this.OnItemMetadataChanged;
            this._MonitoredItems.Add(
                itemMetadata,
                new MonitorInfo
                {
                    IsInList = isInList
                });
        }

        private void RemoveMetadataMonitor(ItemMetadata itemMetadata)
        {
            itemMetadata.MetadataChanged -= this.OnItemMetadataChanged;
            this._MonitoredItems.Remove(itemMetadata);
        }

        private void UpdateFilteredItemsList()
        {
            this._FilteredItems = this._SubjectCollection.Where(i => this._Filter(i)).Select(i => i.Item)
                .OrderBy<object, object>(o => o, new RegionItemComparer(this.SortComparison)).ToList();
        }

        private void RemoveFromFilteredList(object item)
        {
            int index = this._FilteredItems.IndexOf(item);
            this.UpdateFilteredItemsList();
            this.NotifyRemove(new[] { item }, index);
        }

        private void ResetAllMonitors()
        {
            this.RemoveAllMetadataMonitors();
            this.MonitorAllMetadataItems();
        }

        private void RemoveAllMetadataMonitors()
        {
            foreach (var item in this._MonitoredItems)
            {
                item.Key.MetadataChanged -= this.OnItemMetadataChanged;
            }

            this._MonitoredItems.Clear();
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = this.CollectionChanged;
            if (null != handler)
                handler(this, e);
        }

        private void OnItemMetadataChanged(object sender, EventArgs e)
        {
            ItemMetadata itemMetadata = (ItemMetadata)sender;

            MonitorInfo monitorInfo;
            bool foundInfo = this._MonitoredItems.TryGetValue(itemMetadata, out monitorInfo);
            if (!foundInfo) return;

            if (this._Filter(itemMetadata))
            {
                if (!monitorInfo.IsInList)
                {
                    monitorInfo.IsInList = true;
                    this.UpdateFilteredItemsList();
                    NotifyAdd(itemMetadata.Item);
                }
            }
            else
            {
                monitorInfo.IsInList = false;
                this.RemoveFromFilteredList(itemMetadata.Item);
            }
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.UpdateFilteredItemsList();
                    foreach (ItemMetadata itemMetadata in e.NewItems)
                    {
                        bool isInFilter = this._Filter(itemMetadata);
                        this.AddMetadataMonitor(itemMetadata, isInFilter);
                        if (isInFilter)
                            NotifyAdd(itemMetadata.Item);
                    }

                    if (this._Sort != null)
                    {
                        this.NotifyReset();
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ItemMetadata itemMetadata in e.OldItems)
                    {
                        this.RemoveMetadataMonitor(itemMetadata);
                        if (this._Filter(itemMetadata))
                            this.RemoveFromFilteredList(itemMetadata.Item);
                    }

                    break;

                default:
                    this.ResetAllMonitors();
                    this.UpdateFilteredItemsList();
                    this.NotifyReset();

                    break;
            }
        }

        #region 实现接口IViewsCollection

        public bool Contains(object value)
        {
            return this._FilteredItems.Contains(value);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return this._FilteredItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        private class MonitorInfo
        {
            public bool IsInList { get; set; }
        }

        private class RegionItemComparer : Comparer<object>
        {
            private readonly Comparison<object> comparer;

            public RegionItemComparer(Comparison<object> comparer)
            {
                this.comparer = comparer;
            }

            public override int Compare(object x, object y)
            {
                if (this.comparer == null)
                {
                    return 0;
                }

                return this.comparer(x, y);
            }
        }

    }
}
