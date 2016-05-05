using System;

namespace Frame.OS.WPF.Regions
{
    public abstract class RegionBehavior : IRegionBehavior
    {
        private IRegion _Region;
        public bool IsAttached { get; private set; }
        protected abstract void OnAttach();

        #region 实现接口IRegionBehavior

        public IRegion Region
        {
            get
            {
                return this._Region;
            }
            set
            {
                if (this.IsAttached)
                {
                    throw new InvalidOperationException("Attach方法已经被调用后该属性不可进行设置.");
                }

                this._Region = value;
            }
        }

        public void Attach()
        {
            if (this._Region == null)
            {
                throw new InvalidOperationException("当Region属性为空时,Attach方法不可调用.");
            }

            IsAttached = true;
            OnAttach();
        }

        #endregion
    }
}
