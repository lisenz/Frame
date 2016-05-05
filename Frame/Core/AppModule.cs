using System;
using System.Web;
using System.Collections.Generic;

namespace Frame.Core
{
    /// <summary>
    /// 向实现类提供HttpModule模块初始化和处理
    /// </summary>
    public class AppModule : IHttpModule
    {
        private IEnumerable<IAppHttpModule> _Modules;
        
        public void Dispose()
        {
            if (null != this._Modules)
            {
                foreach (IAppHttpModule module in this._Modules)
                {
                    try
                    {
                        module.Dispose();
                    }
                    catch
                    {
                    }
                }
                App.Current.Dispose();
            }
        }

        public void Init(HttpApplication context)
        {
            this.InitConfigurationModules(context);
            this.InitThisModule(context);
        }

        protected virtual void InitConfigurationModules(HttpApplication context)
        {
            this._Modules = App.ObjectContainer.GetAllObjects<IAppHttpModule>();
            foreach (IAppHttpModule module in this._Modules)
            {
                module.Init(context);
            }
        }

        protected virtual void InitThisModule(HttpApplication context)
        {
        }
    }
}
