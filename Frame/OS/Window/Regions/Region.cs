using System;
using System.Linq;
using System.Windows;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;

namespace Frame.OS.Window.Regions
{
    public class Region : IRegion
    {
        private ObservableCollection<ItemMetadata> _ItemMetadataCollection;
        private string _Name;
        private ViewsCollection _Views;
        private ViewsCollection _ActiveViews;
        private object _Context;
        private IRegionManager _RegionManager;
        private Comparison<object> _Sort;

        public Region()
        {
            this._Sort = Region.DefaultSortComparison;
        }

        protected virtual ObservableCollection<ItemMetadata> ItemMetadataCollection
        {
            get
            {
                if (null == this._ItemMetadataCollection)
                    this._ItemMetadataCollection = new ObservableCollection<ItemMetadata>();

                return this._ItemMetadataCollection;
            }
        }

        #region 实现IRegion接口

        public virtual IViewsCollection Views
        {
            get
            {
                if (null == this._Views)
                {
                    this._Views = new ViewsCollection(this._ItemMetadataCollection, x => true);
                    this._Views.SortComparison = this._Sort;
                }

                return this._Views;
            }
        }

        public virtual IViewsCollection ActiveViews
        {
            get
            {
                if (null == this._ActiveViews)
                {
                    this._ActiveViews = new ViewsCollection(this._ItemMetadataCollection, x => x.IsActive);
                    this._ActiveViews.SortComparison = this._Sort;
                }

                return this._ActiveViews;
            }
        }

        public object Context
        {
            get { return this._Context; }
            set { this._Context = value; }
        }

        public string Name
        {
            get{ return this._Name;}
            set
            {
                if (null != this._Name && value != this._Name)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        "部件名称一旦被设置就不可更改. 当前部件名称为 '{0}'.",
                        this._Name));
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("部件名称不能为空.");
                }

                this._Name = value;
            }
        }

        public Comparison<object> SortComparison
        {
            get { return this._Sort; }
            set
            {
                this._Sort = value;

                if (null != this._ActiveViews)
                    this._ActiveViews.SortComparison = this._Sort;

                if (null != this._Views)
                    this._Views.SortComparison = this._Sort;
            }
        }

        public IRegionManager RegionManager
        {
            get { return this._RegionManager; }
            set { this._RegionManager = value; }
        }

        public IRegionManager Add(object view)
        {
            return this.Add(view, null, false);
        }

        public IRegionManager Add(object view, string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "提供的字符参数 {0} 不能为空.",
                    "viewName"));
            }

            return this.Add(view, viewName, false);
        }

        public virtual IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            IRegionManager manager = createRegionManagerScope ? this.RegionManager.CreateRegionManager() : this.RegionManager;
            this.InnerAdd(view, viewName, manager);
            return manager;
        }

        public virtual void Remove(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);
            this.ItemMetadataCollection.Remove(itemMetadata);
        }

        public virtual void Activate(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);

            if (!itemMetadata.IsActive)
            {
                itemMetadata.IsActive = true;
            }
        }

        public virtual void Deactivate(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);

            if (itemMetadata.IsActive)
            {
                itemMetadata.IsActive = false;
            }
        }

        public virtual object GetView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "提供的字符参数{0}不能为空或为null.", "viewName"));
            }

            ItemMetadata metadata = this._ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName);

            if (null != metadata)
            {
                return metadata.Item;
            }

            return null;
        }

        #endregion

        private void InnerAdd(object view, string viewName, IRegionManager scopeRegionManager)
        {
            if (this._ItemMetadataCollection.FirstOrDefault(x => x.Item == view) != null)
            {
                throw new InvalidOperationException("该视图或模块已存在部件中.");
            }

            ItemMetadata itemMetadata = new ItemMetadata(view);
            if (!string.IsNullOrEmpty(viewName))
            {
                if (null != this._ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                        "名称为 '{0}' 的视图或模块已存在部件中.",
                        viewName));
                }
                itemMetadata.Name = viewName;
            }

            this._ItemMetadataCollection.Add(itemMetadata);

        }

        private ItemMetadata GetItemMetadataOrThrow(object view)
        {
            if (null == view)
            {
                throw new ArgumentNullException("view");
            }

            ItemMetadata itemMetadata = this._ItemMetadataCollection.FirstOrDefault(x => x.Item == view);
            if (null == itemMetadata)
            {
                throw new ArgumentException("该部件不包括指定视图或模块.", "view");
            }

            return itemMetadata;
        }

        public static int DefaultSortComparison(object x, object y)
        {
            if (null == x)
            {
                if (null == y)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (null == y)
                    return 1;
                else
                {
                    Type xType = x.GetType();
                    Type yType = y.GetType();

                    ViewSortHintAttribute xAttribute = xType.GetCustomAttributes(typeof(ViewSortHintAttribute), true).FirstOrDefault() as ViewSortHintAttribute;
                    ViewSortHintAttribute yAttribute = yType.GetCustomAttributes(typeof(ViewSortHintAttribute), true).FirstOrDefault() as ViewSortHintAttribute;

                    return ViewSortHintAttributeComparison(xAttribute, yAttribute);
                }
            }
        }
        private static int ViewSortHintAttributeComparison(ViewSortHintAttribute x, ViewSortHintAttribute y)
        {
            if (null == x)
            {
                if (null == y)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (null == y)
                    return 1;
                else
                    return string.Compare(x.Hint, y.Hint, StringComparison.Ordinal);
            }
        }
    }
}
