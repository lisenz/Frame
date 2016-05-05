namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 发现字段特性并提供对字段元数据访问的接口。
    /// </summary>
    internal interface IFieldAccessor
    {
        /// <summary>
        /// 在派生类中被重写时，获取给定对象支持的字段的值。
        /// </summary>
        /// <param name="instance">其字段值所属的对象。</param>
        /// <returns>instance参数的字段值。</returns>
        object GetValue(object instance);
    }
}
