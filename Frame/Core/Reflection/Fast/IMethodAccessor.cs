namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 发现方法的属性并提供对方法元数据访问的接口。
    /// </summary>
    internal interface IMethodAccessor
    {
        /// <summary>
        /// 使用指定的参数调用当前实例所表示的方法。
        /// </summary>
        /// <param name="instance">对其调用方法的对象。</param>
        /// <param name="parameters">调用的方法的参数列表。这是一个对象数组，
        /// 这些对象与要调用的方法的参数具有相同的数量、顺序和类型。如果没有任何参数，则 parameters应为 null。</param>
        /// <returns>一个对象，包含被调用方法的返回值。</returns>
        object Invoke(object instance, params object[] parameters);
    }
}
