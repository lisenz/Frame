using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.DataStore
{
    /// <summary>
    /// 封装一个SQL语句声明对象所需的一系列属性与方法。
    /// </summary>
    public interface ISqlGeStatement
    {
        /// <summary>
        /// 标识一个值，该值表示该SQL语句是否为查询语句。
        /// </summary>
        bool IsQuery { get; }

        /// <summary>
        /// 获取SQL的文本语句。
        /// </summary>
        string Text { get; }

        /// <summary>
        /// 获取或设置数据连接。
        /// </summary>
        string Connection { get; set; }

        /// <summary>
        /// 创建一个SqlGeCommand操作对象，该对象继承实现ISqlGeCommand接口。
        /// </summary>
        /// <param name="parameters">SQL 语句或存储过程的参数。</param>
        /// <returns>一个实现ISqlGeCommand接口的SqlGeCommand操作对象。</returns>
        ISqlGeCommand CreateCommand(object parameters);

        /// <summary>
        /// 创建一个SqlGeCommand操作对象。
        /// </summary>
        /// <param name="provider">数据访问提供者对象。</param>
        /// <param name="parameters">SQL 语句或存储过程的参数。</param>
        /// <returns>SqlGeCommand操作对象。</returns>
        ISqlGeCommand CreateCommand(IDaoProvider provider, object parameters);
    }
}
