using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ObjectBuilder2;

namespace Frame.OS.Unity.Unity
{
    public class UnityBootstrapperExtension : UnityContainerExtension
    {
        public static bool IsTypeRegistered(IUnityContainer container, Type type)
        {
            UnityBootstrapperExtension extension = container.Configure<UnityBootstrapperExtension>();
            if (extension == null)
            {
                return false;
            }
            IBuildKeyMappingPolicy policy = extension.Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type));
            return policy != null;
        }

        protected override void Initialize()
        {
        }

    }
}
