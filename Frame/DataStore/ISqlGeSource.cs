using System;

namespace Frame.DataStore
{
    /// <summary>
    /// 提供操作SQL文本或存储过程源的方法接口。
    /// </summary>
    public interface ISqlGeSource
    {
        /// <summary>
        /// 判断Sql语句的唯一Key是否有效。
        /// </summary>
        /// <param name="key">数据语句源中的键。默认规则：key不能包含空格、逗号、括号</param>
        /// <returns>返回一个值，该值标识是否有效。</returns>
        bool IsValidKey(string key);

        /// <summary>
        /// 添加一个SQL语句对象。
        /// </summary>
        /// <param name="key">在SQL语句映射字典中的键。</param>
        /// <param name="statement">要添加的SQL语句对象。</param>
        /// <param name="overwrite">表示是否覆盖原有对象写入字典中。</param>
        /// <returns>返回一个值，该值标识是否添加成功。</returns>
        bool Add(string key, ISqlGeStatement statement, bool overwrite = true);

        /// <summary>
        /// 根据唯一Key查找返回一个经过Parse的SQL语句
        /// </summary>
        /// <param name="key">通过IsValidKey检查的唯一标识一条SQL语句的字符串</param>
        /// <returns>没有对应的语句返回null</returns>
        ISqlGeStatement Find(string key);
    }
}
