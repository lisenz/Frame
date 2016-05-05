
namespace Frame.Core.Reflection
{
    /// <summary>
    /// 提供快速反射器的缓存控制的方法接口。
    /// </summary>
    /// <typeparam name="TKey">参数类型。</typeparam>
    /// <typeparam name="TValue">返回值类型。</typeparam>
    public interface IFastReflectionCache<TKey, TValue>
    {
        /// <summary>
        /// 获取以TKey类型对象为键的对应的缓存对象。
        /// </summary>
        /// <param name="key">在缓存中的键值。</param>
        /// <returns>对应的缓存对象。</returns>
        TValue Get(TKey key);
    }
}
