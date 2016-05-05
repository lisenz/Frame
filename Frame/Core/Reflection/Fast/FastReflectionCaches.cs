using System;
//---------
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 访问对应元数据对象的访问器的缓存。
    /// </summary>
    internal class FastReflectionCaches
    {
        #region 字段

        private static IFastReflectionCache<FieldInfo, IFieldAccessor> _FieldAccessorCache;
        private static IFastReflectionCache<MethodInfo, IMethodAccessor> _MethodAccessorCache;
        private static IFastReflectionCache<PropertyInfo, IPropertyAccessor> _PropertyAccessorCache;
        private static IFastReflectionCache<ConstructorInfo, IConstructorAccessor> _ConstructorAccessorCache;

        #endregion

        #region 构造函数

        static FastReflectionCaches()
        {
            _FieldAccessorCache = new FieldAccessorCache();
            _MethodAccessorCache = new MethodAccessorCache();
            _PropertyAccessorCache = new PropertyAccessorCache();
            _ConstructorAccessorCache = new ConstructorAccessorCache();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 访问构造函数访问器对象的缓存对象。
        /// </summary>
        public static IFastReflectionCache<ConstructorInfo, IConstructorAccessor> ConstructorAccessorCache
        {
            get { return FastReflectionCaches._ConstructorAccessorCache; }
            set { FastReflectionCaches._ConstructorAccessorCache = value; }
        }

        /// <summary>
        /// 访问属性访问器对象的缓存对象。
        /// </summary>
        public static IFastReflectionCache<PropertyInfo, IPropertyAccessor> PropertyAccessorCache
        {
            get { return FastReflectionCaches._PropertyAccessorCache; }
            set { FastReflectionCaches._PropertyAccessorCache = value; }
        }

        /// <summary>
        /// 访问方法访问器对象的缓存对象。
        /// </summary>
        public static IFastReflectionCache<MethodInfo, IMethodAccessor> MethodAccessorCache
        {
            get { return FastReflectionCaches._MethodAccessorCache; }
            set { FastReflectionCaches._MethodAccessorCache = value; }
        }

        /// <summary>
        /// 访问字段访问器对象的缓存对象。
        /// </summary>
        public static IFastReflectionCache<FieldInfo, IFieldAccessor> FieldAccessorCache
        {
            get { return FastReflectionCaches._FieldAccessorCache; }
            set { FastReflectionCaches._FieldAccessorCache = value; }
        }

        #endregion
    }
}
