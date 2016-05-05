namespace Frame.Core.Reflection
{
    /// <summary>
    /// 提供快速反射器创建对应访问器对象的工厂的方法接口。
    /// </summary>
    /// <typeparam name="TKey">对应访问器的类型对象。</typeparam>
    /// <typeparam name="TValue">访问器接口对象。</typeparam>
    internal interface IFastReflectionFactory<TKey, TValue>
    {
        /// <summary>
        /// 创建指定类型的访问器接口对象。
        /// </summary>
        /// <param name="key">访问器的类型对象。
        /// <remarks>
        /// TODO：例如TKey为FieldInfo对象，则创建对应的IFieldAccessor对象；
        /// 具体以工厂方法接口的TKey与TValue为主。
        /// </remarks>
        /// </param>
        /// <returns>创建的访问器接口对象。</returns>
        TValue Create(TKey key);
    }
}
