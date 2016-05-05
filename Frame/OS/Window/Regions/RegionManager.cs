using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace Frame.OS.Window.Regions
{
    public class RegionManager : IRegionManager
    {
        private readonly RegionCollection _RegionCollection;

        public RegionManager()
        {
            this._RegionCollection = new RegionCollection(this);
        }

        #region 实现接口IRegionManager

        public IRegionCollection Regions
        {
            get { return this._RegionCollection; }
        }

        public IRegionManager CreateRegionManager()
        {
            return new RegionManager();
        }

        #endregion

    }
}
