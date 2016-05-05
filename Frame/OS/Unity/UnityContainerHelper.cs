using System;
using Microsoft.Practices.Unity;

namespace Frame.OS.Unity.Unity
{
    public static class UnityContainerHelper
    {
        public static bool IsTypeRegistered(this IUnityContainer container, Type type)
        {
            return UnityBootstrapperExtension.IsTypeRegistered(container, type);
        }

        public static T TryResolve<T>(this IUnityContainer container)
        {
            object result = TryResolve(container, typeof(T));
            if (result != null)
            {
                return (T)result;
            }
            return default(T);
        }

        public static object TryResolve(this IUnityContainer container, Type typeToResolve)
        {
            try
            {
                return container.Resolve(typeToResolve);
            }
            catch
            {
                return null;
            }
        }
    }
}
