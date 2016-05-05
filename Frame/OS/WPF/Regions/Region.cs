using System;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;

namespace Frame.OS.WPF.Regions
{
    public class Region : IRegion
    {
        private ObservableCollection<ItemMetadata> _ItemMetadataCollection;
        private string _Name;
        private ViewsCollection _Views;
        private ViewsCollection _ActiveViews;
        private object _Context;
        private IRegionManager _RegionManager;
        private IRegionNavigationService _RegionNavigationService;

        private Comparison<object> sort;

        public Region()
        {
            this.Behaviors = new RegionBehaviorCollection(this);

            this.sort = Region.DefaultSortComparison;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IRegionBehaviorCollection Behaviors { get; private set; }

        public object Context
        {
            get
            {
                return this._Context;
            }

            set
            {
                if (this._Context != value)
                {
                    this._Context = value;
                    this.OnPropertyChanged("Context");
                }
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                if (null != this._Name && value != this._Name)
                {
                    throw new InvalidOperationException(string.Format("部件名称一旦被设置就不可更改. 当前部件名称为 '{0}'.",
                        this._Name));
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("部件名称不能为空.");
                }

                this._Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public virtual IViewsCollection Views
        {
            get
            {
                if (this._Views == null)
                {
                    this._Views = new ViewsCollection(this._ItemMetadataCollection, x => true);
                    this._Views.SortComparison = this.sort;
                }

                return this._Views;
            }
        }

        public virtual IViewsCollection ActiveViews
        {
            get
            {
                if (this._ActiveViews == null)
                {
                    this._ActiveViews = new ViewsCollection(this._ItemMetadataCollection, x => x.IsActive);
                    this._ActiveViews.SortComparison = this.sort;
                }

                return this._ActiveViews;
            }
        }

        public Comparison<object> SortComparison
        {
            get
            {
                return this.sort;
            }
            set
            {
                this.sort = value;

                if (this._ActiveViews != null)
                {
                    this._ActiveViews.SortComparison = this.sort;
                }

                if (this._Views != null)
                {
                    this._Views.SortComparison = this.sort;
                }
            }
        }

        public IRegionManager RegionManager
        {
            get
            {
                return this._RegionManager;
            }

            set
            {
                if (this._RegionManager != value)
                {
                    this._RegionManager = value;
                    this.OnPropertyChanged("RegionManager");
                }
            }
        }

        public IRegionNavigationService NavigationService
        {
            get
            {
                if (this._RegionNavigationService == null)
                {
                    this._RegionNavigationService = ServiceLocator.Current.GetInstance<IRegionNavigationService>();
                    this._RegionNavigationService.Region = this;
                }

                return this._RegionNavigationService;
            }

            set
            {
                this._RegionNavigationService = value;
            }
        }

        protected virtual ObservableCollection<ItemMetadata> ItemMetadataCollection
        {
            get
            {
                if (this._ItemMetadataCollection == null)
                {
                    this._ItemMetadataCollection = new ObservableCollection<ItemMetadata>();
                }

                return this._ItemMetadataCollection;
            }
        }

        public IRegionManager Add(object view)
        {
            return this.Add(view, null, false);
        }

        public IRegionManager Add(object view, string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format("提供的字符参数 {0} 不能为空.",
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

            this._ItemMetadataCollection.Remove(itemMetadata);

            DependencyObject dependencyObject = view as DependencyObject;
            if (dependencyObject != null && Regions.RegionManager.GetRegionManager(dependencyObject) == this.RegionManager)
            {
                dependencyObject.ClearValue(Regions.RegionManager.RegionManagerProperty);
            }
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
                throw new ArgumentException(string.Format("提供的字符参数{0}不能为空或为null.", "viewName"));
            }

            ItemMetadata metadata = this._ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName);

            if (metadata != null)
            {
                return metadata.Item;
            }

            return null;
        }

        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            this.NavigationService.RequestNavigate(target, navigationCallback);
        }

        private void InnerAdd(object view, string viewName, IRegionManager scopedRegionManager)
        {
            if (this._ItemMetadataCollection.FirstOrDefault(x => x.Item == view) != null)
            {
                throw new InvalidOperationException("该视图或模块已存在部件中.");
            }

            ItemMetadata itemMetadata = new ItemMetadata(view);
            if (!string.IsNullOrEmpty(viewName))
            {
                if (this._ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName) != null)
                {
                    throw new InvalidOperationException(String.Format("名称为 '{0}' 的视图或模块已存在部件中.",
                        viewName));
                }
                itemMetadata.Name = viewName;
            }

            DependencyObject dependencyObject = view as DependencyObject;

            if (dependencyObject != null)
            {
                Regions.RegionManager.SetRegionManager(dependencyObject, scopedRegionManager);
            }

            this._ItemMetadataCollection.Add(itemMetadata);
        }

        private ItemMetadata GetItemMetadataOrThrow(object view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            ItemMetadata itemMetadata = this._ItemMetadataCollection.FirstOrDefault(x => x.Item == view);
            if (itemMetadata == null)
            {
                throw new ArgumentException("该部件没有包含指定视图.", "view");
            }

            return itemMetadata;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static int DefaultSortComparison(object x, object y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
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
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    return string.Compare(x.Hint, y.Hint, StringComparison.Ordinal);
                }
            }
        }
    }
}
