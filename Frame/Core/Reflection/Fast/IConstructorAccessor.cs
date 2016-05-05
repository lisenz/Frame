namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 发现类构造函数的属性并提供对构造函数元数据访问的接口。
    /// </summary>
    internal interface IConstructorAccessor
    {
        /// <summary>
        /// 调用具有指定参数的实例所反映的构造函数。
        /// </summary>
        /// <param name="parameters">与此构造函数的参数的个数、顺序和类型（受默认联编程序的约束）相匹配的值数组。
        /// 如果此构造函数没有参数，则 parameters应为 null。</param>
        /// <returns>与构造函数关联的类的实例。</returns>
        object Invoke(params object[] parameters);
    }
}
