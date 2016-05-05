using System.Collections.Generic;

namespace Frame.OS.Window.Regions
{
    /// <summary>
    /// 提供操作部件对象集合的属性和方法。
    /// </summary>
    public interface IRegionCollection : IEnumerable<IRegion>
    {
        /// <summary>
        /// 获取指定名称的部件。
        /// </summary>
        /// <param name="regionName">部分的名称。</param>
        /// <returns>返回指定名称的部件对象。</returns>
        IRegion this[string regionName] { get; }

        /// <summary>
        /// 添加部件到集合中。
        /// </summary>
        /// <param name="region">部件对象。</param>
        void Add(IRegion region);

        /// <summary>
        /// 移除集合中指定名称的部件对象。
        /// </summary>
        /// <param name="regionName">要移除的部件名称。</param>
        /// <returns>返回一个值，该值标识是否移除成功。</returns>
        bool Remove(string regionName);

        /// <summary>
        /// 判断集合中是否存在指定名称的部件。
        /// </summary>
        /// <param name="regionName">部件名称。</param>
        /// <returns>返回一个值，该值标识是否存在指定名称的部件。若存在，则返回true；否则返回false。</returns>
        bool ContainsRegionWithName(string regionName);
    }
}
