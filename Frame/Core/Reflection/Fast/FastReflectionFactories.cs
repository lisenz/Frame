using System;
//---------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 创建访问对应元数据对象的访问器的工厂。
    /// </summary>
    internal static class FastReflectionFactories
    {
        #region 字段

        private static IFastReflectionFactory<FieldInfo, IFieldAccessor> _FieldAccessorFactory;
        private static IFastReflectionFactory<MethodInfo, IMethodAccessor> _MethodAccessorFactory;
        private static IFastReflectionFactory<PropertyInfo, IPropertyAccessor> _PropertyAccessorFactory;
        private static IFastReflectionFactory<ConstructorInfo, IConstructorAccessor> _ConstructorAccessorFactory;

        #endregion

        #region 静态构造函数

        static FastReflectionFactories()
        {
            _FieldAccessorFactory = new FieldAccessorFactory();
            _MethodAccessorFactory = new MethodAccessorFactory();
            _PropertyAccessorFactory = new PropertyAccessorFactory();
            _ConstructorAccessorFactory = new ConstructorAccessorFactory();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 对构造函数元数据访问的访问器的工厂对象。
        /// </summary>
        public static IFastReflectionFactory<ConstructorInfo, IConstructorAccessor> ConstructorAccessorFactory
        {
            get { return FastReflectionFactories._ConstructorAccessorFactory; }
            set { FastReflectionFactories._ConstructorAccessorFactory = value; }
        }

        /// <summary>
        /// 对属性元数据访问的访问器的工厂对象。
        /// </summary>
        public static IFastReflectionFactory<PropertyInfo, IPropertyAccessor> PropertyAccessorFactory
        {
            get { return FastReflectionFactories._PropertyAccessorFactory; }
            set { FastReflectionFactories._PropertyAccessorFactory = value; }
        }

        /// <summary>
        /// 对方法元数据访问的访问器的工厂对象。
        /// </summary>
        public static IFastReflectionFactory<MethodInfo, IMethodAccessor> MethodAccessorFactory
        {
            get { return FastReflectionFactories._MethodAccessorFactory; }
            set { FastReflectionFactories._MethodAccessorFactory = value; }
        }

        /// <summary>
        /// 对字段元数据访问的访问器的工厂对象。
        /// </summary>
        public static IFastReflectionFactory<FieldInfo, IFieldAccessor> FieldAccessorFactory
        {
            get { return FastReflectionFactories._FieldAccessorFactory; }
            set { FastReflectionFactories._FieldAccessorFactory = value; }
        }

        #endregion
    }
}
