using System;
using System.Collections.Generic;

namespace Frame.OS.WPF.Regions
{
    public class RegionAdapterMappings
    {
        private readonly Dictionary<Type, IRegionAdapter> _Mappings = new Dictionary<Type, IRegionAdapter>();

        public void RegisterMapping(Type controlType, IRegionAdapter adapter)
        {
            if (controlType == null)
            {
                throw new ArgumentNullException("controlType");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            if (this._Mappings.ContainsKey(controlType))
            {
                throw new InvalidOperationException(String.Format("Mapping with the given type is already registered: {0}.",
                                                                  controlType.Name));
            }

            this._Mappings.Add(controlType, adapter);
        }

        public IRegionAdapter GetMapping(Type controlType)
        {
            Type currentType = controlType;

            while (currentType != null)
            {
                if (this._Mappings.ContainsKey(currentType))
                {
                    return this._Mappings[currentType];
                }
                currentType = currentType.BaseType;
            }
            throw new KeyNotFoundException(String.Format("The IRegionAdapter for the type {0} is not registered in the region adapter mappings. You can register an IRegionAdapter for this control by overriding the ConfigureRegionAdapterMappings method in the bootstrapper.",
                controlType));
        }

    }
}
