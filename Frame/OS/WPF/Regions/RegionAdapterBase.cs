using System;
using System.Windows;
using Frame.OS.WPF.Regions.Behaviors;

namespace Frame.OS.WPF.Regions
{
    public abstract class RegionAdapterBase<T> : IRegionAdapter where T : class
    {
        protected IRegionBehaviorFactory RegionBehaviorFactory { get; set; }

        protected RegionAdapterBase(IRegionBehaviorFactory regionBehaviorFactory)
        {
            this.RegionBehaviorFactory = regionBehaviorFactory;
        }

        #region 实现接口IRegionAdapter

        public IRegion Initialize(T regionTarget, string regionName)
        {
            if (regionName == null)
            {
                throw new ArgumentNullException("regionName");
            }

            IRegion region = this.CreateRegion();
            region.Name = regionName;

            SetObservableRegionOnHostingControl(region, regionTarget);

            this.Adapt(region, regionTarget);
            this.AttachBehaviors(region, regionTarget);
            this.AttachDefaultBehaviors(region, regionTarget);
            return region;
        }

        IRegion IRegionAdapter.Initialize(object regionTarget, string regionName)
        {
            return this.Initialize(GetCastedObject(regionTarget), regionName);
        }

        #endregion

        protected virtual void AttachDefaultBehaviors(IRegion region, T regionTarget)
        {
            if (region == null) 
                throw new ArgumentNullException("region");
            if (regionTarget == null) 
                throw new ArgumentNullException("regionTarget");

            IRegionBehaviorFactory behaviorFactory = this.RegionBehaviorFactory;
            if (behaviorFactory != null)
            {
                DependencyObject dependencyObjectRegionTarget = regionTarget as DependencyObject;

                foreach (string behaviorKey in behaviorFactory)
                {
                    if (!region.Behaviors.ContainsKey(behaviorKey))
                    {
                        IRegionBehavior behavior = behaviorFactory.CreateFromKey(behaviorKey);

                        if (dependencyObjectRegionTarget != null)
                        {
                            IHostAwareRegionBehavior hostAwareRegionBehavior = behavior as IHostAwareRegionBehavior;
                            if (hostAwareRegionBehavior != null)
                            {
                                hostAwareRegionBehavior.HostControl = dependencyObjectRegionTarget;
                            }
                        }

                        region.Behaviors.Add(behaviorKey, behavior);
                    }
                }
            }
        }

        protected virtual void AttachBehaviors(IRegion region, T regionTarget)
        {
        }

        protected abstract void Adapt(IRegion region, T regionTarget);

        protected abstract IRegion CreateRegion();

        private static T GetCastedObject(object regionTarget)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException("regionTarget");
            }

            T castedObject = regionTarget as T;
            if (castedObject == null)
            {
                throw new InvalidOperationException(string.Format("为使用当前部件适配器，对象类型必须为'{0}'.",
                    typeof(T).Name));
            }

            return castedObject;
        }

        private static void SetObservableRegionOnHostingControl(IRegion region, T regionTarget)
        {
            DependencyObject targetElement = regionTarget as DependencyObject;

            if (targetElement != null)
            {
                RegionManager.GetObservableRegion(targetElement).Value = region;
            }
        }
    }
}
