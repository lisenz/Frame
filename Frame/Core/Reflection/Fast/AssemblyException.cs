using System;
using System.Text;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 动态加载DLL链接库异常事件类。
    /// </summary>
    internal class AssemblyException : Exception
    {
        #region 属性
        #endregion

        #region 字段

        /// <summary>
        /// 异常消息
        /// </summary>
        private string _StrError = string.Empty;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数。动态加载DLL链接库异常事件类。
        /// </summary>
        public AssemblyException()
        {
            this._StrError = string.Empty;
        }

        /// <summary>
        /// 构造函数。动态加载DLL链接库异常事件类。
        /// </summary>
        /// <param name="fStrDllName">加载的DLL链接库名称。</param>
        public AssemblyException(string fStrDllName)
            : this()
        {
            this._StrError = fStrDllName;
        }

        #endregion

        #region 方法

        public override string Message
        {
            get
            {
                string StrMessage = string.Format("试图加载系统模块文件(源于:{0})出错，请联系开发商请求技术支持!", this._StrError);
                return base.Message;
            }
        }

        #endregion
    }
}
