using System;
using System.Windows;
using Frame.OS.WPF.Regions;

namespace Frame.OS.WPF.Regions.Behaviors
{
    public class ClearChildViewsRegionBehavior : RegionBehavior
    {
        public const string BehaviorKey = "ClearChildViews";

        public static readonly DependencyProperty ClearChildViewsProperty =
            DependencyProperty.RegisterAttached("ClearChildViews", typeof(bool), typeof(ClearChildViewsRegionBehavior), new PropertyMetadata(false));

        public static bool GetClearChildViews(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            return (bool)target.GetValue(ClearChildViewsRegionBehavior.ClearChildViewsProperty);
        }

        public static void SetClearChildViews(DependencyObject target, bool value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            target.SetValue(ClearChildViewsRegionBehavior.ClearChildViewsProperty, value);
        }

        protected override void OnAttach()
        {
            this.Region.PropertyChanged += Region_PropertyChanged;
        }

        private static void ClearChildViews(IRegion region)
        {
            foreach (var view in region.Views)
            {
                DependencyObject dependencyObject = view as DependencyObject;
                if (dependencyObject != null)
                {
                    if (GetClearChildViews(dependencyObject))
                    {
                        dependencyObject.ClearValue(RegionManager.RegionManagerProperty);
                    }
                }
            }
        }

        private void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RegionManager")
            {
                if (this.Region.RegionManager == null)
                {
                    ClearChildViews(this.Region);
                }
            }
        }
    }
}
