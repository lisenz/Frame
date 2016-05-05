namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 发现属性的特性并提供对属性元数据访问的接口。
    /// </summary>
    internal interface IPropertyAccessor
    {
        /// <summary>
        /// 在派生类中被重写时，获取给定对象支持的属性的值。
        /// </summary>
        /// <param name="instance">将获取其属性值的对象。</param>
        /// <returns>instance参数的属性值。</returns>
        object GetValue(object instance);

        /// <summary>
        /// 在派生类中被重写时，设置给定对象支持的该属性的值。
        /// </summary>
        /// <param name="instance">将设置其属性值的对象。</param>
        /// <param name="value">此属性的新值。</param>
        void SetValue(object instance, object value);
    }
}
