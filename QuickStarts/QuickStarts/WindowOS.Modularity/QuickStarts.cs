using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Frame.Core;
using Frame.Core.Reflection;
using Frame.OS.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Frame.OS.Unity;
using Frame.OS.Window.Modularity;
using Frame.OS.Unity.Unity;


namespace WindowOS.Modularity
{
    public partial class QuickStarts : Form
    {
        public QuickStarts()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            IModuleCatalog moduleCatalog = new ModuleCatalog();
            IUnityContainer container = new UnityContainer();

            

            string path = "WindowOS.Modularity.lib.dll";
            Type moduleAType = AssemblyBuilder.Build().TryGetType(path, "WindowOS.Modularity.lib.ModularityModule");
            ModuleInfo moduleInfo = new ModuleInfo("ModularityModule", moduleAType.AssemblyQualifiedName);
            moduleInfo.Ref =string.Format("file://{0}", Path.Combine(App.BaseDirectory, path));
            moduleCatalog.AddModule(moduleInfo);

            container.RegisterInstance(moduleCatalog);

            container.RegisterType(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IModuleInitializer), typeof(ModuleInitializer), new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IModuleManager), typeof(ModuleManager), new ContainerControlledLifetimeManager());
            
            //ServiceLocator.SetLocatorProvider(() => container.Resolve<IServiceLocator>());

            IModuleManager manager = container.Resolve<IModuleManager>();
            //manager.Run();

            manager.LoadModule("ModularityModule");
        }

        private void button2_Click(object sender, EventArgs e)
        {


            string path = "WindowOS.Modularity.lib.dll";
            Type moduleAType = AssemblyBuilder.Build().TryGetType(path, "WindowOS.Modularity.lib.ModularityModule");
            ModuleInfo moduleInfo = new ModuleInfo("ModularityModule", moduleAType.AssemblyQualifiedName);

            IUnityContainer container = new UnityContainer();
            IModuleCatalog moduleCatalog = new ModuleCatalog();
            moduleCatalog.AddModule(moduleInfo);

            container.RegisterInstance(moduleCatalog);

            container.RegisterType(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IModuleInitializer), typeof(ModuleInitializer), new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IModuleManager), typeof(ModuleManager), new ContainerControlledLifetimeManager());
            //ServiceLocator.SetLocatorProvider(() => container.Resolve<IServiceLocator>());


            //IServiceLocator s = new UnityServiceLocatorAdapter(container);
            IModuleInitializer initer = container.Resolve<IModuleInitializer>();
            initer.Initialize(moduleInfo);
        }
    }
}
