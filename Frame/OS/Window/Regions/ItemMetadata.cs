using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.OS.Window.Regions
{
    /// <summary>
    /// 装载的项元数据对象。
    /// </summary>
    public class ItemMetadata
    {
        private string _Name;
        private bool _IsActive;
        public event EventHandler MetadataChanged;

        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }

        public bool IsActive
        {
            get { return this._IsActive; }
            set { this._IsActive = value; }
        }

        public object Item { get; private set; }
        
        public ItemMetadata(object item)
        {
            this.Item = item;
        }

        public void InvokeMetadataChanged()
        {
            EventHandler metadataChangedHandler = this.MetadataChanged;
            if (null != metadataChangedHandler)
                metadataChangedHandler(this, EventArgs.Empty);
        }
    }
}
