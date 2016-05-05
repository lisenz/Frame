using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 发现属性的特性并提供对属性元数据访问的方法。
    /// </summary>
    internal class PropertyAccessor : IPropertyAccessor
    {
        #region 字段

        /// <summary>
        /// 给定对象支持的属性。
        /// </summary>
        private PropertyInfo _PropertyInfo;

        /// <summary>
        /// 给定对象支持的属性对象的一个具有两个参数并返回object参数的类型值的委托。
        /// </summary>
        private Func<object, object> _Gettor;

        /// <summary>
        /// 给定对象支持的属性对象的set访问器。
        /// </summary>
        private MethodAccessor _Settor;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fPropertyInfo">给定对象支持的属性。</param>
        public PropertyAccessor(PropertyInfo fPropertyInfo)
        {
            this._PropertyInfo = fPropertyInfo;
            InitializeGet(this._PropertyInfo);
            InitializeSet(this._PropertyInfo);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化设置给定对象支持的属性对象的一个具有两个参数并返回object参数的类型值的委托。
        /// 用于设置给定对象的属性的get访问器。
        /// </summary>
        /// <param name="fPropertyInfo">给定对象支持的属性。</param>
        private void InitializeGet(PropertyInfo fPropertyInfo)
        {
            if (fPropertyInfo.CanRead)
            {
                ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
                UnaryExpression parameter;
                if (fPropertyInfo.GetGetMethod(true).IsStatic)
                    parameter = null;
                else
                    parameter = Expression.Convert(instance, fPropertyInfo.ReflectedType);
                this._Gettor = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(parameter, fPropertyInfo), typeof(object)), new ParameterExpression[] { instance }).Compile();
            }
        }

        /// <summary>
        /// 初始化设置给定对象支持的属性对象的set访问器。
        /// </summary>
        /// <param name="fPropertyInfo">给定对象支持的属性。</param>
        private void InitializeSet(PropertyInfo fPropertyInfo)
        {
            if (fPropertyInfo.CanWrite)
            {
                this._Settor = new MethodAccessor(fPropertyInfo.GetSetMethod(true));
            }
        }

        /// <summary>
        /// 获取给定对象支持的属性的值。
        /// </summary>
        /// <param name="instance">将获取其属性值的对象。</param>
        /// <returns>instance参数的属性值。</returns>
        public object GetValue(object instance)
        {
            return this._Gettor.Invoke(instance);
        }

        /// <summary>
        /// 设置给定对象支持的该属性的值。
        /// </summary>
        /// <param name="instance">将设置其属性值的对象。</param>
        /// <param name="value">此属性的新值。</param>
        public void SetValue(object instance, object value)
        {
            this._Settor.Invoke(instance, new object[] { value });
        }

        /// <summary>
        /// IPropertyAccessor接口的显式方法，获取给定对象支持的属性的值。
        /// </summary>
        /// <param name="instance">将获取其属性值的对象。</param>
        /// <returns>instance参数的属性值。</returns>
        object IPropertyAccessor.GetValue(object instance)
        {
            return GetValue(instance);
        }

        /// <summary>
        /// IPropertyAccessor接口的显式方法，设置给定对象支持的该属性的值。
        /// </summary>
        /// <param name="instance">将设置其属性值的对象。</param>
        /// <param name="value">此属性的新值。</param>
        void IPropertyAccessor.SetValue(object instance, object value)
        {
            SetValue(instance, value);
        }

        #endregion
    }
}
