using System;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Frame.Core.Extensions;

namespace Frame.OS
{
    public class RegionViewRegistry : IRegionViewRegistry
    {
        private readonly IServiceLocator _Locator;
        private readonly ListDictionary<string, Func<object>> _RegisteredContent = new ListDictionary<string, Func<object>>();
        private readonly WeakDelegatesManager _ContentRegisteredListeners = new WeakDelegatesManager();

        public RegionViewRegistry(IServiceLocator locator)
        {
            this._Locator = locator;
        }

        #region 实现接口IRegionViewRegistry

        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered
        {
            add { this._ContentRegisteredListeners.AddListener(value); }
            remove { this._ContentRegisteredListeners.RemoveListener(value); }
        }

        public IEnumerable<object> GetContents(string regionName)
        {
            List<object> items = new List<object>();
            foreach (Func<object> getContentDelegate in this._RegisteredContent[regionName])
            {
                items.Add(getContentDelegate());
            }

            return items;
        }

        public void RegisterViewWithRegion(string regionName, Type viewType)
        {
            this.RegisterViewWithRegion(regionName, () => this.CreateInstance(viewType));
        }

        public void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            this._RegisteredContent.Add(regionName, getContentDelegate);
            this.OnContentRegistered(new ViewRegisteredEventArgs(regionName, getContentDelegate));
        }

        #endregion

        protected virtual object CreateInstance(Type type)
        {
            return this._Locator.GetInstance(type);
        }

        private void OnContentRegistered(ViewRegisteredEventArgs e)
        {
            try
            {
                this._ContentRegisteredListeners.Raise(this, e);
            }
            catch (TargetInvocationException ex)
            {
                Exception rootException;
                if (ex.InnerException != null)
                {
                    rootException = ex.InnerException.GetRootException();
                }
                else
                {
                    rootException = ex.GetRootException();
                }

                throw new ViewRegistrationException(string.Format(CultureInfo.CurrentCulture,
                    "当试图添加一个视图或模块'{0}'到部件中时发生异常 .- 导致该异常的原因可能为: '{1}'.但也检查内部异常中的更多信息或者调用GetRootException()进行调试查看. ",
                    e.RegionName, rootException), ex.InnerException);
            }
        }
    }
}
