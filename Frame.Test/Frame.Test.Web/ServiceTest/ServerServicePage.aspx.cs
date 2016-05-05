using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Frame.Service.ServiceHttp;

//------------
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.ServiceModel;
using System.CodeDom.Compiler;

namespace Frame.Test.Web.ServiceTest
{
    public partial class ServerServicePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0"), ServiceContract(ConfigurationName = "Takewin.Data.Contract.IGenercContract")]
    public interface IGenericContract
    {
        #region 基本实现方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回受影响的行数。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>受影响的行数。</returns>
        [OperationContract(Action = "http://tempuri.org/ExecuteNonQuery", ReplyAction = "*"), XmlSerializerFormat]
        int ExecuteNonQuery(string sql, DictionaryEntry[] parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果集合。</returns>
        [OperationContract(Action = "http://tempuri.org/QueryDataSet", ReplyAction = "*"), XmlSerializerFormat]
        DataSet QueryDataSet(string sql, DictionaryEntry[] parameters = null);

        #endregion

        #region 分页查询方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果集合。</returns>
        [OperationContract(Action = "http://tempuri.org/PageQueryDataSet", ReplyAction = "*"), XmlSerializerFormat]
        DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, DictionaryEntry[] parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="totalRowCount">实际结果集的行数。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果集合。</returns>
        [OperationContract(Name = "PageQueryDataSetWithOut", Action = "http://tempuri.org/PageQueryDataSetWithOut", ReplyAction = "*"), XmlSerializerFormat]
        DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, DictionaryEntry[] parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的IDictionary对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果对象集合。</returns>
        [OperationContract(Action = "http://tempuri.org/PageQueryDictionaries", ReplyAction = "*"), XmlSerializerFormat]
        List<List<DictionaryEntry>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, DictionaryEntry[] parameters = null);

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的IDictionary对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="totalRowCount">实际结果集的行数。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果对象集合。</returns>
        [OperationContract(Name = "PageQueryDictionariesWithOut", Action = "http://tempuri.org/PageQueryDictionariesWithOut", ReplyAction = "*"), XmlSerializerFormat]
        List<List<DictionaryEntry>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, DictionaryEntry[] parameters = null);

        #endregion

        #region 查询多行数据方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回IDictionary类型的对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>IDictionary类型的对象结果集合。</returns>
        [OperationContract(Action = "http://tempuri.org/QueryDictionaries", ReplyAction = "*"), XmlSerializerFormat]
        List<List<DictionaryEntry>> QueryDictionaries(string sql, DictionaryEntry[] parameters = null);

        #endregion

        #region 查询单行单列数据方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行Dictionary类型的对象结果。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>Dictionary类型的对象</returns>
        [OperationContract(Action = "http://tempuri.org/QueryDictionary", ReplyAction = "*"), XmlSerializerFormat]
        List<DictionaryEntry> QueryDictionary(string sql, DictionaryEntry[] parameters = null);

        #endregion

    }
}