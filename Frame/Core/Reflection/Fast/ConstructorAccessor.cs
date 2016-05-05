using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
//----------
using System.Reflection;
using System.Linq.Expressions;

namespace Frame.Core.Reflection.Fast
{
    internal class ConstructorAccessor : IConstructorAccessor
    {
        #region 字段

        /// <summary>
        /// 给定对象的构造函数属性对象。
        /// </summary>
        private ConstructorInfo _ConstructorInfo;

        /// <summary>
        /// IConstructorAccessor接口提供对方法元数据访问的委托对象。
        /// </summary>
        private Func<object[], object> _Invoker;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fConstructorInfo">调用给定对象支持的构造函数元数据对象。</param>
        public ConstructorAccessor(ConstructorInfo fConstructorInfo)
        {
            this._ConstructorInfo = fConstructorInfo;
            this._Invoker = this.InitializeInvoker(fConstructorInfo);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 创建返回给定对象支持的方法元数据对象的一个具有一个参数并返回object参数的类型值的委托。
        /// </summary>
        /// <param name="fConstructorInfo">给定对象支持的构造函数元数据对象。</param>
        /// <returns>一个具有一个参数并返回object参数的类型值的委托。</returns>
        private Func<object[], object> InitializeInvoker(ConstructorInfo fConstructorInfo)
        {
            ParameterExpression parameters = Expression.Parameter(typeof(object[]), "parameters");
            List<Expression> arguments = new List<Expression>();
            ParameterInfo[] paramInfos = fConstructorInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                UnaryExpression item = Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i)), paramInfos[i].ParameterType);
                arguments.Add(item);
            }

            return Expression.Lambda<Func<object[], object>>(Expression.Convert(Expression.New(fConstructorInfo, arguments), typeof(object)), new ParameterExpression[] { parameters }).Compile();
        }

        /// <summary>
        /// 调用具有指定参数的实例所反映的构造函数。
        /// </summary>
        /// <param name="parameters">与此构造函数的参数的个数、顺序和类型（受默认联编程序的约束）相匹配的值数组。
        /// 如果此构造函数没有参数，则像 Object[] parameters new Object[0] 中那样，使用包含零元素或 null 的数组。</param>
        /// <returns>与构造函数关联的类的实例。</returns>
        public object Invoke(params object[] parameters)
        {
            return this._Invoker.Invoke(parameters);
        }

        /// <summary>
        /// IConstructorAccessor接口显式方法，调用具有指定参数的实例所反映的构造函数。
        /// </summary>
        /// <param name="parameters">与此构造函数的参数的个数、顺序和类型（受默认联编程序的约束）相匹配的值数组。
        /// 如果此构造函数没有参数，则像 Object[] parameters new Object[0] 中那样，使用包含零元素或 null 的数组。</param>
        /// <returns>与构造函数关联的类的实例。</returns>
        object IConstructorAccessor.Invoke(params object[] parameters)
        {
            return this.Invoke(parameters);
        }

        #endregion
    }
}
