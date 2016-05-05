using System.Collections.Generic;
using System.Collections.Specialized;

namespace Frame.OS
{
    /// <summary>
    /// 视图集合提供的统一方法。
    /// </summary>
    public interface IViewsCollection : IEnumerable<object>, INotifyCollectionChanged
    {
        /// <summary>
        /// 返回一个值，该值标识列表集合中是否包含value对象。
        /// </summary>
        /// <param name="value">要在列表集合中定位查找的对象。</param>
        /// <returns>如果列表集合中包含指定对象，则为 true；否则为 false。</returns>
        bool Contains(object value);
    }
}
