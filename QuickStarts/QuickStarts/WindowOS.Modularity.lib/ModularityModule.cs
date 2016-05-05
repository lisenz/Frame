using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frame.OS.Modularity;

namespace WindowOS.Modularity.lib
{
    public class ModularityModule:IModule
    {
        private IModuleManager _Manager;

        public ModularityModule(IModuleManager manager)
        {
            this._Manager = manager;
        }

        public void Initialize()
        {
            string v = string.Empty;
        }
    }
}
