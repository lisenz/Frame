using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class SelectorItemsSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public static readonly string BehaviorKey = "SelectorItemsSourceSyncBehavior";
        private bool _UpdatingActiveViewsInHostControlSelectionChanged;
        private Selector hostControl;

        public DependencyObject HostControl
        {
            get
            {
                return this.hostControl;
            }

            set
            {
                this.hostControl = value as Selector;
            }
        }

        protected override void OnAttach()
        {
            bool itemsSourceIsSet = this.hostControl.ItemsSource != null;
            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException("ItemsControl's ItemsSource属性不为空."
                    + "该控件被关联到一个Region中, 但该控件已经被绑定有信息."
                    + "如果没有显式地设置控件的ItemSource属性,这个异常也许是因为继承RegionManager附加属性的值发生改变所导致.");
            }

            this.SynchronizeItems();

            this.hostControl.SelectionChanged += this.HostControlSelectionChanged;
            this.Region.ActiveViews.CollectionChanged += this.ActiveViews_CollectionChanged;
            this.Region.Views.CollectionChanged += this.Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startIndex = e.NewStartingIndex;
                foreach (object newItem in e.NewItems)
                {
                    this.hostControl.Items.Insert(startIndex++, newItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    this.hostControl.Items.Remove(oldItem);
                }
            }
        }

        private void SynchronizeItems()
        {
            List<object> existingItems = new List<object>();

            // 控件在绑定到部件中之前必须为空
            foreach (object childItem in this.hostControl.Items)
            {
                existingItems.Add(childItem);
            }

            foreach (object view in this.Region.Views)
            {
                this.hostControl.Items.Add(view);
            }

            foreach (object existingItem in existingItems)
            {
                this.Region.Add(existingItem);
            }
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._UpdatingActiveViewsInHostControlSelectionChanged)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (this.hostControl.SelectedItem != null
                    && this.hostControl.SelectedItem != e.NewItems[0]
                    && this.Region.ActiveViews.Contains(this.hostControl.SelectedItem))
                {
                    this.Region.Deactivate(this.hostControl.SelectedItem);
                }

                this.hostControl.SelectedItem = e.NewItems[0];
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove &&
                     e.OldItems.Contains(this.hostControl.SelectedItem))
            {
                this.hostControl.SelectedItem = null;
            }
        }

        private void HostControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this._UpdatingActiveViewsInHostControlSelectionChanged = true;

                object source;
                source = e.OriginalSource;
                if (source == sender)
                {
                    foreach (object item in e.RemovedItems)
                    {
                        // 检查视图是否同时在部件的视图集合和活动视图集合中(有可能不同步)
                        if (this.Region.Views.Contains(item) && this.Region.ActiveViews.Contains(item))
                        {
                            this.Region.Deactivate(item);
                        }
                    }

                    foreach (object item in e.AddedItems)
                    {
                        if (this.Region.Views.Contains(item) && !this.Region.ActiveViews.Contains(item))
                        {
                            this.Region.Activate(item);
                        }
                    }
                }
            }
            finally
            {
                this._UpdatingActiveViewsInHostControlSelectionChanged = false;
            }
        }
    }
}
