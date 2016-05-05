using System;
using System.Globalization;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Frame.OS.Window.Regions;
using Frame.OS.Modularity;
using Frame.OS.Unity.Unity;
using Frame.OS.Window.Modularity;

namespace Frame.OS.Window.Unity
{
    public abstract class UnityBootstrapper : Bootstrapper
    {
        private bool _UseDefaultConfiguration = true;
        public IUnityContainer Container { get; protected set; }
        public IRegionManager RegionManagor { get; protected set; }

        #region 实现抽象类Bootstrapper

        public override void Run(bool runWithDefaultConfiguration)
        {
            this._UseDefaultConfiguration = runWithDefaultConfiguration;

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

            if (this.Container.IsRegistered<IRegionManager>())
            {
                this.InitializeRegions();
            }

            if (this.Container.IsRegistered<IModuleManager>())
            {
                this.InitializeModules();
            }

            this.Shell = this.CreateShell();
            if (this.Shell != null)
            {
                this.InitializeShell();
            }
        }

        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => this.Container.Resolve<IServiceLocator>());
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

        #endregion

        protected virtual void ConfigureContainer()
        {
            this.Container.AddNewExtension<UnityBootstrapperExtension>();

            this.Container.RegisterInstance(this.ModuleCatalog);

            if (_UseDefaultConfiguration)
            {
                RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
                RegisterTypeIfMissing(typeof(IModuleInitializer), typeof(ModuleInitializer), true);
                RegisterTypeIfMissing(typeof(IModuleManager), typeof(ModuleManager), true);
                RegisterTypeIfMissing(typeof(IRegionManager), typeof(RegionManager), true);
                RegisterTypeIfMissing(typeof(IRegionViewRegistry), typeof(RegionViewRegistry), true);
            }
        }
        
        protected virtual void InitializeRegions()
        {
            IRegionManager manager;
            try
            {
                manager = this.Container.Resolve<IRegionManager>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            this.RegionManagor = manager;
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

            // 调用UnityContainerHelper的拓展方法
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
