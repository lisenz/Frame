using System;
using System.Windows.Forms;
using Frame.OS.Modularity;
using Frame.OS.Window.Modularity;
using Microsoft.Practices.ServiceLocation;

namespace Frame.OS.Window
{
    public abstract class Bootstrapper
    {
        #region 字段

        protected Form Shell { get; set; }

        protected IModuleCatalog ModuleCatalog { get; set; }
        
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

        protected virtual void InitializeModules()
        {
            IModuleManager manager = ServiceLocator.Current.GetInstance<IModuleManager>();
            manager.Run();
        }

        protected virtual void InitializeShell()
        {
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(this.Shell);
        }

        public abstract void Run(bool runWithDefaultConfiguration);

        protected abstract Form CreateShell();

        protected abstract void ConfigureServiceLocator();
    }
}
