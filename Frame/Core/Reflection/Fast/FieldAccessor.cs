using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Frame.Core.Reflection.Fast
{
    internal class FieldAccessor : IFieldAccessor
    {
        #region 字段

        /// <summary>
        /// 将获取给定对象支持的字段值的字段对象。
        /// </summary>
        private FieldInfo _FieldInfo;

        /// <summary>
        /// IFieldAccessor接口提供对字段元数据访问的委托对象。
        /// </summary>
        private Func<object, object> _Gettor;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fFieldInfo">将获取给定对象支持的字段值的字段对象。</param>
        public FieldAccessor(FieldInfo fFieldInfo)
        {
            this._FieldInfo = fFieldInfo;
            this._Gettor = GetDelegate(fFieldInfo);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 创建返回给定对象支持的字段对象的一个具有一个参数并返回object参数的类型值的委托。
        /// </summary>
        /// <param name="fFieldInfo">给定对象的字段对象。</param>
        /// <returns>一个具有一个参数并返回object参数的类型值的委托。</returns>
        private Func<object, object> GetDelegate(FieldInfo fFieldInfo)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            UnaryExpression parameter;
            if (fFieldInfo.IsStatic)
            {
                parameter = null;
            }
            else
            {
                parameter = Expression.Convert(instance, fFieldInfo.ReflectedType);
            }

            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Field(parameter, fFieldInfo), typeof(object)), new ParameterExpression[] { instance }).Compile();
        }

        /// <summary>
        /// 获取给定对象支持的字段的值。
        /// </summary>
        /// <param name="instance">其字段值所属的对象。</param>
        /// <returns>instance参数的字段值。</returns>
        public object GetValue(object instance)
        {
            return this._Gettor.Invoke(instance);
        }

        /// <summary>
        /// IFieldAccessor接口显式方法，获取给定对象支持的字段的值。
        /// </summary>
        /// <param name="instance">其字段值所属的对象。</param>
        /// <returns>instance参数的字段值。</returns>
        object IFieldAccessor.GetValue(object instance)
        {
            return GetValue(instance);
        }

        #endregion
    }
}
