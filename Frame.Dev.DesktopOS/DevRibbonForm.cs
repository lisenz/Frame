using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
//-----------------
using Frame.DataStore;
using Frame.Service.Client;
using Frame.Dev.DesktopOS.Common;
using DevExpress.XtraBars.Ribbon;

namespace Frame.Dev.DesktopOS
{
    /// <summary>
    /// C/S框架的Ribbon窗体基类，定义了用于Ribbon窗体操作的常用方法。
    /// </summary>
    public class DevRibbonForm : RibbonForm
    {
        #region 组件设计器代码

        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.ShowInTaskbar = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.MaximizeBox = false;
            this.Load += new System.EventHandler(this.BaseRibbonForm_Load);
            this.ResumeLayout(false);
        }

        #endregion

        #region 字段

        private BaseDao _Dao = null;
        private readonly string _name = "DefaultDB";

        #endregion

        #region 构造函数

        public DevRibbonForm()
        {
            InitializeComponent();
        }

        public DevRibbonForm(string name)
        {
            InitializeComponent();
            this._name = name;
        }

        #endregion

        #region 内置方法

        private void InitStyle()
        {
            InitializeButtonStyle();
            InitialGridRowStyle();
        }

        /// <summary>
        /// 设置数据库访问对象。
        /// </summary>
        protected void SetDao()
        {
            this._Dao = BaseDao.Get(this._name);
        }

        /// <summary>
        /// 设置按钮控件的样式。
        /// </summary>
        protected virtual void InitializeButtonStyle()
        {
            CommonStyler.Instance.InitializeButtonStyle(this);
        }

        /// <summary>
        /// 设置GridView行的样式。
        /// </summary>
        protected virtual void InitialGridRowStyle()
        {
            CommonStyler.Instance.InitializeGridRowStyle(this);
        }

        #region 虚方法，对应主界面操作按钮

        /// <summary>
        /// 新增信息。
        /// </summary>
        public virtual void AddInfo()
        {
        }

        /// <summary>
        /// 查询信息。
        /// </summary>
        public virtual void QueryInfo()
        {
        }

        /// <summary>
        /// 更新信息。
        /// </summary>
        public virtual void UpdateInfo()
        {
        }

        /// <summary>
        /// 删除信息。
        /// </summary>
        public virtual void DeleteInfo()
        {
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public virtual void RefreshInfo()
        {
        }

        #endregion

        #region 基本实现方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回受影响的行数。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>受影响的行数。</returns>
        protected int ExecuteNonQuery(string sql, IDictionary<string, object> parameters = null)
        {
            return this._Dao.ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回一个结果只读器。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果只读器。</returns>
        protected IDataReader QueryReader(string sql, IDictionary<string, object> parameters = null)
        {
            return this._Dao.QueryReader(sql, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回结果集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个结果集合。</returns>
        protected DataSet QueryDataSet(string sql, IDictionary<string, object> parameters = null)
        {
            return this._Dao.QueryDataSet(sql, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句，并返回指定泛型类型对象。
        /// </summary>
        /// <typeparam name="T">返回的结果的类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns></returns>
        protected T QueryScalar<T>(string sql, IDictionary<string, object> parameters = null)
        {
            return this._Dao.QueryScalar<T>(sql, parameters);
        }

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
        protected DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, IDictionary<string, object> parameters = null)
        {
            return this._Dao.PageQueryDataSet(sql, startRowIndex, maximumRows, sortExpression, parameters);
        }

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
        protected DataSet PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, IDictionary<string, object> parameters = null)
        {
            return this._Dao.PageQueryDataSet(sql, startRowIndex, maximumRows, sortExpression, out totalRowCount, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的IDictionary<string, object>对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果对象集合。</returns>
        protected IList<IDictionary<string, object>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, IDictionary<string, object> parameters = null)
        {
            return this._Dao.PageQueryDictionaries(sql, startRowIndex, maximumRows, sortExpression, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的IDictionary<string, object>对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="totalRowCount">实际结果集的行数。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>分页后的结果对象集合。</returns>
        protected IList<IDictionary<string, object>> PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, IDictionary<string, object> parameters = null)
        {
            return this._Dao.PageQueryDictionaries(sql, startRowIndex, maximumRows, sortExpression, out totalRowCount, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">返回的对象集合的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>结果对象集合。</returns>
        protected IList<T> PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, IDictionary<string, object> parameters = null) where T : class,new()
        {
            return this._Dao.PageQueryEntities<T>(sql, startRowIndex, maximumRows, sortExpression, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回分页后的指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">返回的对象集合的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="startRowIndex">读取的数据集的起始行。</param>
        /// <param name="maximumRows">要读取的数据集的行数。</param>
        /// <param name="sortExpression">数据集的排序规则。</param>
        /// <param name="totalRowCount">实际结果集的行数。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>结果对象集合。</returns>
        protected IList<T> PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, IDictionary<string, object> parameters = null) where T : class,new()
        {
            return this._Dao.PageQueryEntities<T>(sql, startRowIndex, maximumRows, sortExpression, out totalRowCount, parameters);
        }

        #endregion

        #region 查询多行数据方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回IDictionary<string, object>类型的对象集合。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>IDictionary<string, object>类型的对象结果集合。</returns>
        protected IList<IDictionary<string, object>> QueryDictionaries(string sql, IDictionary<string, object> parameters = null)
        {
            return this._Dao.QueryDictionaries(sql, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的数据结果集合。
        /// </summary>
        /// <typeparam name="T">指定返回的数据结果类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的数据结果集合。</returns>
        protected IList<T> QueryScalarList<T>(string sql, IDictionary<string, object> parameters = null)
        {
            return this._Dao.QueryScalarList<T>(sql, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回指定类型的对象集合。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>指定类型的对象集合。</returns>
        protected IList<T> QueryEntities<T>(string sql, IDictionary<string, object> parameters = null) where T : class,new()
        {
            return this._Dao.QueryEntities<T>(sql, parameters);
        }

        #endregion

        #region 查询单行单列数据方法接口的方法

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行Dictionary<string, object>类型的对象结果。
        /// </summary>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>Dictionary<string, object>类型的对象</returns>
        protected IDictionary<string, object> QueryDictionary(string sql, IDictionary<string, object> parameters = null)
        {
            return this._Dao.QueryDictionary(sql, parameters);
        }

        /// <summary>
        /// 执行SQL语句或SQL配置文件中指定KEY的SQL语句,并返回单行指定类型的对象结果。
        /// </summary>
        /// <typeparam name="T">指定返回的对象类型。</typeparam>
        /// <param name="sql">SQL语句或SQL配置文件中指定KEY名称。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>单行指定类型的对象结果。</returns>
        protected T QueryEntity<T>(string sql, IDictionary<string, object> parameters = null) where T : class,new()
        {
            return this._Dao.QueryEntity<T>(sql, parameters);
        }

        #endregion

        #endregion

        #region 事件

        private void BaseRibbonForm_Load(object sender, EventArgs e)
        {
            InitStyle();
            //SetDao();
        }

        #endregion
        
        #region 远程轻量级服务异步调用

        protected void AsynQuery(string command, IDictionary<string, object> parameters, Action<ReturnResult<DataValue>> callBack)
        {
            DataService.AsynQuery(command, parameters, callBack);
        }

        protected void AsynExecute<T>(string command, IDictionary<string, object> parameters, Action<ReturnResult<T>> callBack)
        {
            DataService.AsynExcute<T>(command, parameters, callBack);
        }

        #endregion

        #region 远程轻量级服务同步调用

        protected ReturnResult<DataValue> SynQuery(string command, IDictionary<string, object> parameters)
        {
            return DataService.SynQuery(command, parameters);
        }

        protected ReturnResult<T> SynExecute<T>(string command, IDictionary<string, object> parameters)
        {
            return DataService.SynExcute<T>(command, parameters);
        }

        #endregion
    }
}
