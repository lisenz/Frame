using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 发现方法的属性并提供对方法元数据访问的方法。
    /// </summary>
    internal class MethodAccessor : IMethodAccessor
    {
        #region 字段

        /// <summary>
        /// 调用给定对象支持的方法元数据对象。
        /// </summary>
        private MethodInfo _MethodInfo;

        /// <summary>
        /// IMethodAccessor接口提供对方法元数据访问的委托对象。
        /// </summary>
        private Func<object, object[], object> _Invoker;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fMethodInfo">调用给定对象支持的方法元数据对象。</param>
        public MethodAccessor(MethodInfo fMethodInfo)
        {
            this._MethodInfo = fMethodInfo;
            this._Invoker = GetDelegate(fMethodInfo);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 创建返回给定对象支持的方法元数据对象的一个具有两个参数并返回object参数的类型值的委托。
        /// </summary>
        /// <param name="fMethodInfo">给定对象支持的方法元数据对象。</param>
        /// <returns>一个具有两个参数并返回object参数的类型值的委托。</returns>
        private Func<object, object[], object> GetDelegate(MethodInfo fMethodInfo)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression parameters = Expression.Parameter(typeof(object[]), "parameters");
            List<Expression> parameterExpressions = new List<Expression>();
            ParameterInfo[] paramInfos = fMethodInfo.GetParameters();
            int length = paramInfos.Length;
            for (int i = 0; i < length; i++)
            {
                BinaryExpression valueObj = Expression.ArrayIndex(parameters, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);
                parameterExpressions.Add(valueCast);
            }

            Expression instanceCast = fMethodInfo.IsStatic ? null : Expression.Convert(instance, fMethodInfo.ReflectedType);
            MethodCallExpression methodCall = Expression.Call(instanceCast, fMethodInfo, parameterExpressions);
            if (methodCall.Type == typeof(void))
            {
                Expression<Action<object, object[]>> lambda = Expression.Lambda<Action<object, object[]>>(methodCall, instance, parameters);
                Action<object, object[]> invoke = lambda.Compile();
                return (fInstance, fParameters) =>
                {
                    invoke(fInstance, fParameters);
                    return null;
                };
            }
            else
            {
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<Func<object, object[], object>> lambda = Expression.Lambda<Func<object, object[], object>>(castMethodCall, instance, parameters);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// 使用指定的参数调用当前实例所表示的方法。
        /// </summary>
        /// <param name="instance">对其调用方法的对象。</param>
        /// <param name="parameters">调用的方法的参数列表。这是一个对象数组，
        /// 这些对象与要调用的方法的参数具有相同的数量、顺序和类型。如果没有任何参数，则 parameters应为 null。</param>
        /// <returns>被调用方法的返回值。</returns>
        public object Invoke(object instance, params object[] parameters)
        {
            return this._Invoker.Invoke(instance, parameters);
        }

        /// <summary>
        /// IMethodAccessor接口显式方法，使用指定的参数调用当前实例所表示的方法。
        /// </summary>
        /// <param name="instance">对其调用方法的对象。</param>
        /// <param name="parameters">调用的方法的参数列表。这是一个对象数组，
        /// 这些对象与要调用的方法的参数具有相同的数量、顺序和类型。如果没有任何参数，则 parameters应为 null。</param>
        /// <returns>被调用方法的返回值。</returns>
        object IMethodAccessor.Invoke(object instance, params object[] parameters)
        {
            return Invoke(instance, parameters);
        }

        #endregion

    }
}
