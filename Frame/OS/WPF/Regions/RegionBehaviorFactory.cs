using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Frame.OS.WPF.Regions
{
    public class RegionBehaviorFactory : IRegionBehaviorFactory
    {
        private readonly IServiceLocator _ServiceLocator;
        private readonly Dictionary<string, Type> _RegisteredBehaviors = new Dictionary<string, Type>();

        public RegionBehaviorFactory(IServiceLocator serviceLocator)
        {
            this._ServiceLocator = serviceLocator;
        }

        #region 实现接口IRegionBehaviorFactory

        public void AddIfMissing(string behaviorKey, Type behaviorType)
        {
            if (behaviorKey == null)
                throw new ArgumentNullException("behaviorKey");

            if (behaviorType == null)
                throw new ArgumentNullException("behaviorType");

            // 检测对象类型是否为IRegionBehavior接口
            if (!typeof(IRegionBehavior).IsAssignableFrom(behaviorType))
            {
                throw new ArgumentException(
                    string.Format(Thread.CurrentThread.CurrentCulture,
                    "类型 '{0}' 不是从接口IRegionBehavior继承.",
                    behaviorType.Name), "behaviorType");
            }

            // 注册的行为列表中已存在键值. 
            if (this._RegisteredBehaviors.ContainsKey(behaviorKey))
                return;

            this._RegisteredBehaviors.Add(behaviorKey, behaviorType);
        }

        public bool ContainsKey(string behaviorKey)
        {
            return this._RegisteredBehaviors.ContainsKey(behaviorKey);
        }

        public IRegionBehavior CreateFromKey(string key)
        {
            if (!this.ContainsKey(key))
            {
                throw new ArgumentException(
                    string.Format(Thread.CurrentThread.CurrentCulture,
                    "键值'{0}'的行为已注册.",
                    key), "key");
            }

            return (IRegionBehavior)this._ServiceLocator.GetInstance(this._RegisteredBehaviors[key]);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this._RegisteredBehaviors.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }
}
