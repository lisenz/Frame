using System;
using System.Globalization;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Frame.OS.Modularity;
using Frame.OS.WPF.Regions;
using Frame.OS.Unity.Unity;
using Frame.Core.Extensions;
using Frame.OS.WPF.Modularity;
using Frame.OS.WPF.Events;

namespace Frame.OS.WPF.Unity
{
    public abstract class UnityBootstrapper : Bootstrapper
    {
        private bool useDefaultConfiguration = true;

        public IUnityContainer Container { get; protected set; }

        public override void Run(bool runWithDefaultConfiguration)
        {
            this.useDefaultConfiguration = runWithDefaultConfiguration;

            this.ModuleCatalog = this.CreateModuleCatalog();
            if (this.ModuleCatalog == null)
            {
                throw new InvalidOperationException("模块目录对象不能为空或null.");
            }

            this.ConfigureModuleCatalog();
            this.Container = this.CreateContainer();
            if (this.Container == null)
            {
                throw new InvalidOperationException("DI容器对象不能为空或null.");
            }

            this.ConfigureContainer();

            this.ConfigureServiceLocator();

            this.ConfigureRegionAdapterMappings();

            this.ConfigureDefaultRegionBehaviors();

            this.RegisterFrameworkExceptionTypes();

            this.Shell = this.CreateShell();
            if (this.Shell != null)
            {
                RegionManager.SetRegionManager(this.Shell, this.Container.Resolve<IRegionManager>());

                RegionManager.UpdateRegions();

                this.InitializeShell();
            }

            if (this.Container.IsRegistered<IModuleManager>())
            {
                this.InitializeModules();
            }

        }

        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => this.Container.Resolve<IServiceLocator>());
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(Microsoft.Practices.Unity.ResolutionFailedException));
        }

        protected virtual void ConfigureContainer()
        {
            this.Container.AddNewExtension<UnityBootstrapperExtension>();
            
            this.Container.RegisterInstance(this.ModuleCatalog);

            if (useDefaultConfiguration)
            {
                RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
                RegisterTypeIfMissing(typeof(IModuleInitializer), typeof(ModuleInitializer), true);
                RegisterTypeIfMissing(typeof(IModuleManager), typeof(ModuleManager), true);
                RegisterTypeIfMissing(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings), true);
                RegisterTypeIfMissing(typeof(IRegionManager), typeof(RegionManager), true);
                RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
                RegisterTypeIfMissing(typeof(IRegionViewRegistry), typeof(RegionViewRegistry), true);
                RegisterTypeIfMissing(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory), true);
                RegisterTypeIfMissing(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry), false);
                RegisterTypeIfMissing(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal), false);
                RegisterTypeIfMissing(typeof(IRegionNavigationService), typeof(RegionNavigationService), false);
                RegisterTypeIfMissing(typeof(IRegionNavigationContentLoader), typeof(RegionNavigationContentLoader), true);
            }
        }

        protected override void InitializeModules()
        {
            IModuleManager manager;

            try
            {
                manager = this.Container.Resolve<IModuleManager>();
            }
            catch (ResolutionFailedException ex)
            {
                if (ex.Message.Contains("IModuleCatalog"))
                {
                    throw new InvalidOperationException("模块目录对象不能为空或null.");
                }

                throw;
            }

            manager.Run();
        }

        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected void RegisterTypeIfMissing(Type fromType, Type toType, bool registerAsSingleton)
        {
            if (fromType == null)
            {
                throw new ArgumentNullException("fromType");
            }
            if (toType == null)
            {
                throw new ArgumentNullException("toType");
            }
            if (Container.IsTypeRegistered(fromType))
            {
            }
            else
            {
                if (registerAsSingleton)
                {
                    Container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
                }
                else
                {
                    Container.RegisterType(fromType, toType);
                }
            }
        }
    }
}
