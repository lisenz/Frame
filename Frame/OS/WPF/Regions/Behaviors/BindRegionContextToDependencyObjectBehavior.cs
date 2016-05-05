﻿using System.Windows;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class BindRegionContextToDependencyObjectBehavior : IRegionBehavior
    {
        public const string BehaviorKey = "ContextToDependencyObject";

        public IRegion Region { get; set; }

        public void Attach()
        {
            this.Region.Views.CollectionChanged += this.Views_CollectionChanged;
            this.Region.PropertyChanged += this.Region_PropertyChanged;
            SetContextToViews(this.Region.Views, this.Region.Context);
            this.AttachNotifyChangeEvent(this.Region.Views);
        }

        private static void SetContextToViews(IEnumerable views, object context)
        {
            foreach (var view in views)
            {
                DependencyObject dependencyObjectView = view as DependencyObject;
                if (dependencyObjectView != null)
                {
                    ObservableObject<object> contextWrapper = RegionContext.GetObservableContext(dependencyObjectView);
                    contextWrapper.Value = context;
                }
            }
        }

        private void AttachNotifyChangeEvent(IEnumerable views)
        {
            foreach (var view in views)
            {
                var dependencyObject = view as DependencyObject;
                if (dependencyObject != null)
                {
                    ObservableObject<object> viewRegionContext = RegionContext.GetObservableContext(dependencyObject);
                    viewRegionContext.PropertyChanged += this.ViewRegionContext_OnPropertyChangedEvent;
                }
            }
        }

        private void DetachNotifyChangeEvent(IEnumerable views)
        {
            foreach (var view in views)
            {
                var dependencyObject = view as DependencyObject;
                if (dependencyObject != null)
                {
                    ObservableObject<object> viewRegionContext = RegionContext.GetObservableContext(dependencyObject);
                    viewRegionContext.PropertyChanged -= this.ViewRegionContext_OnPropertyChangedEvent;
                }
            }
        }

        private void ViewRegionContext_OnPropertyChangedEvent(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Value")
            {
                var context = (ObservableObject<object>)sender;
                this.Region.Context = context.Value;
            }
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SetContextToViews(e.NewItems, this.Region.Context);
                this.AttachNotifyChangeEvent(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && this.Region.Context != null)
            {
                this.DetachNotifyChangeEvent(e.OldItems);
                SetContextToViews(e.OldItems, null);

            }
        }

        private void Region_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Context")
            {
                SetContextToViews(this.Region.Views, this.Region.Context);
            }
        }
    }
}
