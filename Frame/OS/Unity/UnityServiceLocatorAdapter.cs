using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;

namespace Frame.OS.Unity.Unity
{
    public class UnityServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly IUnityContainer _unityContainer;

        public UnityServiceLocatorAdapter(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _unityContainer.ResolveAll(serviceType);
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return _unityContainer.Resolve(serviceType, key);
        }
    }
}
