using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.ServiceLocation;
using Frame.OS.Modularity;
using Frame.OS.WPF.Regions;
using Frame.Core.Extensions;
using Frame.OS.WPF.Modularity;
using Frame.OS.WPF.Regions.Behaviors;

namespace Frame.OS.WPF
{
    public abstract class Bootstrapper
    {
        #region 字段

        /// <summary>
        /// 
        /// </summary>
        protected IModuleCatalog ModuleCatalog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected DependencyObject Shell { get; set; }

        #endregion

        #region 公有方法

        public void Run()
        {
            this.Run(true);
        }

        #endregion
        
        protected virtual IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog();
        }

        protected virtual void ConfigureModuleCatalog()
        {
        }

        protected virtual void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(Microsoft.Practices.ServiceLocation.ActivationException));
        }

        protected virtual void InitializeModules()
        {
            IModuleManager manager = ServiceLocator.Current.GetInstance<IModuleManager>();
            manager.Run();
        }

        protected virtual RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings regionAdapterMappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
            if (regionAdapterMappings != null)
            {
                regionAdapterMappings.RegisterMapping(typeof(Selector), ServiceLocator.Current.GetInstance<SelectorRegionAdapter>());
                regionAdapterMappings.RegisterMapping(typeof(ItemsControl), ServiceLocator.Current.GetInstance<ItemsControlRegionAdapter>());
                regionAdapterMappings.RegisterMapping(typeof(ContentControl), ServiceLocator.Current.GetInstance<ContentControlRegionAdapter>());
            }

            return regionAdapterMappings;
        }

        protected virtual IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var defaultRegionBehaviorTypesDictionary = ServiceLocator.Current.GetInstance<IRegionBehaviorFactory>();

            if (defaultRegionBehaviorTypesDictionary != null)
            {
                defaultRegionBehaviorTypesDictionary.AddIfMissing(AutoPopulateRegionBehavior.BehaviorKey,
                                                                  typeof(AutoPopulateRegionBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(BindRegionContextToDependencyObjectBehavior.BehaviorKey,
                                                                  typeof(BindRegionContextToDependencyObjectBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionActiveAwareBehavior.BehaviorKey,
                                                                  typeof(RegionActiveAwareBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(SyncRegionContextWithHostBehavior.BehaviorKey,
                                                                  typeof(SyncRegionContextWithHostBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionManagerRegistrationBehavior.BehaviorKey,
                                                                  typeof(RegionManagerRegistrationBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionMemberLifetimeBehavior.BehaviorKey,
                                                  typeof(RegionMemberLifetimeBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(ClearChildViewsRegionBehavior.BehaviorKey,
                                                  typeof(ClearChildViewsRegionBehavior));

            }

            return defaultRegionBehaviorTypesDictionary;
        }

        protected virtual void InitializeShell()
        {
        }

        public abstract void Run(bool runWithDefaultConfiguration);

        protected abstract DependencyObject CreateShell();

        protected abstract void ConfigureServiceLocator();
    }
}
