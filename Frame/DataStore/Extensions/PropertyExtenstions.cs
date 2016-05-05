using System;
using System.Reflection;
//----------
using Frame.DataStore.Map.Attributes;

namespace Frame.DataStore.Extensions
{
    /// <summary>
    /// Property的拓展类，提供对Property拓展的方法。
    /// </summary>
    public static class PropertyExtenstions
    {
        /// <summary>
        /// 获取指定属性元数据中标记ColumnAttribute特性的成员名称。
        /// </summary>
        /// <param name="prop">指定属性元数据。</param>
        /// <returns>标记了ColumnAttribute特性的成员名称，若不存在此特性，则返回属性元数据的名称。</returns>
        public static string ColumnName(this PropertyInfo prop)
        {
            object[] attributes = prop.GetCustomAttributes(typeof(ColumnAttribute), false);
            if (attributes.Length > 0)
            {
                return ((ColumnAttribute)attributes[0]).Name;
            }

            return prop.Name;
        }

        /// <summary>
        /// 获取指定属性元数据中标记TableAttribute特性的成员名称。
        /// </summary>
        /// <param name="prop">指定属性元数据。</param>
        /// <returns>标记了TableAttribute特性的成员名称，若不存在此特性，则返回属性元数据的名称。</returns>
        public static string TableName(this PropertyInfo prop)
        {
            object[] attributes = prop.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes.Length > 0)
            {
                return ((TableAttribute)attributes[0]).Name;
            }

            return prop.Name;
        }

        /// <summary>
        /// 返回一个值，该值标识指定属性元数据中是否标记PrimaryKeyAttribute特性。
        /// </summary>
        /// <param name="prop">指定属性元数据。</param>
        /// <returns>指定属性元数据中是否标记PrimaryKeyAttribute特性。</returns>
        public static bool IsPrimaryKey(this PropertyInfo prop)
        {
            object[] attributes = prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
            if (attributes.Length > 0)
            {
                return true;
            }
            return false;
        }
    }
}
