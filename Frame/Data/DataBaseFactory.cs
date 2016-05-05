using System;

namespace Frame.Data
{
    /// <summary>
    /// 创建数据库的工厂类。
    /// </summary>
    public static class DataBaseFactory
    {
        /// <summary>
        /// 数据库在配置文件中的默认键值。
        /// </summary>
        private const string _DbName = "DefaultDB";


        /// <summary>
        /// 创建默认数据库对象。
        /// </summary>
        /// <returns>对新建的数据库对象的引用。</returns>
        public static DataBase CreateDatabase()
        {
            return InnerCreateDatabase(_DbName);
        }

        /// <summary>
        /// 创建配置文件中指定键值名称对应的数据库。
        /// </summary>
        /// <param name="name">数据库在配置文件中键的名称。</param>
        /// <returns>创建的数据库。</returns>
        public static DataBase CreateDatabase(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("该值不能为null或空值", name);

            return InnerCreateDatabase(name);
        }

        /// <summary>
        /// 获取创建的数据库。
        /// </summary>
        /// <param name="name">数据库的名称。</param>
        /// <returns>数据库对象。</returns>
        private static DataBase InnerCreateDatabase(string name)
        {
            DataBase instance;
            try
            {
                instance = DataBaseLibraryContainer.Current.GetDataBase(name);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            return instance;
        }
    }
}
