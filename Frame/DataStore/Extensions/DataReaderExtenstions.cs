using System;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
//----------------
using Frame.Core;
using Frame.Core.Extensions;

namespace Frame.DataStore.Extensions
{
    public static class DataReaderExtenstions
    {
        #region 单个对象映射

        public static T Read<T>(this IDataReader reader, Action<T, string, object> unknowMapping = null) where T : class,new()
        {
            return Read<T>(reader, typeof(T), unknowMapping);
        }

        public static T Read<T>(this IDataReader reader, Type instanceType, Action<T, string, object> unknowMapping = null)
        {
            if (reader.Read())
            {
                object instance = Activator.CreateInstance(instanceType);

                //获取对象中公共的属性
                PropertyInfo[] props =
                    instanceType.GetProperties(BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    object columnValue = reader[i];

                    PropertyInfo prop =
                        props.SingleOrDefault(p => MatchColumnName(p.ColumnName(), columnName));

                    if (prop != null)
                    {
                        prop.FastSetValue(instance, Convertor.Convert(prop.PropertyType, columnValue));
                    }
                    else if (null != unknowMapping)
                    {
                        unknowMapping((T)instance, columnName, columnValue);
                    }
                }
                return (T)instance;
            }
            return default(T);
        }

        #endregion

        #region 集合映射

        public static IList<T> ReadList<T>(this IDataReader reader, Action<T, string, object> unknowMapping = null) where T : class,new()
        {
            return ReadList<T>(reader, typeof(T), unknowMapping);
        }

        public static IList<T> ReadList<T, T1>(this IDataReader reader, Action<T, string, object> unknowMapping = null)
            where T : class
            where T1 : T, new()
        {
            return ReadList<T>(reader, typeof(T1), unknowMapping);
        }

        public static IList<T> ReadList<T>(this IDataReader reader, Type instanceType, Action<T, string, object> unknowMapping = null) where T : class
        {
            IList<T> list = new List<T>();

            if (reader.Read())
            {
                IList<KeyValuePair<PropertyInfo, int>> mappings = GetSetterMappings(instanceType, reader);

                do
                {
                    list.Add(Read<T>(reader, instanceType, mappings, unknowMapping));
                }
                while (reader.Read());
            }
            return list;
        }

        #endregion

        #region 内部实现

        internal static T Read<T>(IDataReader reader, Type type, IList<KeyValuePair<PropertyInfo, int>> mappings, Action<T, string, object> unknowMapping = null)
        {
            object instance = Activator.CreateInstance(type);

            foreach (KeyValuePair<PropertyInfo, int> mapping in mappings)
            {
                int index = mapping.Value;
                PropertyInfo prop = mapping.Key;
                if (index >= 0)
                {
                    prop.FastSetValue(instance, Convertor.Convert(prop.PropertyType, reader[index]));
                }
                else if (unknowMapping != null)
                {
                    unknowMapping((T)instance, reader.GetName(index), reader[index]);
                }
            }
            return (T)instance;
        }

        internal static IList<KeyValuePair<PropertyInfo, int>> GetSetterMappings(Type type, IDataReader reader)
        {
            PropertyInfo[] props = type.GetProperties(BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public);
            IList<KeyValuePair<PropertyInfo, int>> mappings = new List<KeyValuePair<PropertyInfo, int>>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);

                PropertyInfo prop =
                    props.SingleOrDefault(p => MatchColumnName(p.ColumnName(), columnName));

                if (prop != null)
                {
                    mappings.Add(new KeyValuePair<PropertyInfo, int>(prop, i));
                }
                else
                {
                    mappings.Add(new KeyValuePair<PropertyInfo, int>(prop, -1));
                }
            }

            return mappings;
        }

        internal static bool MatchColumnName(string name, string columnName)
        {
            return columnName.EqualsIgnoreCase(name) ||
                    columnName.Replace(" ", "").EqualsIgnoreCase(name) ||
                    columnName.Replace("_", "").EqualsIgnoreCase(name);
        }

        #endregion
    }
}
