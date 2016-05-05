using System;
using System.Text;
//-------------
using System.IO;
using System.Reflection;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 动态加载程序集的对象类[支持远程处理]。
    /// </summary>
    internal class AssemblyAccessor : MarshalByRefObject
    {

        #region 属性

        /// <summary>
        /// 程序集对象。
        /// </summary>
        public Assembly Assemblyer
        {
            get { return this._Assemblyer; }
        }

        #endregion

        #region 字段

        /// <summary>
        /// 程序集对象。
        /// </summary>
        private Assembly _Assemblyer = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数。
        /// </summary>
        public AssemblyAccessor()
        {
            this._Assemblyer = null;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="fStrDllName">程序集文件的完全路径。</param>
        public AssemblyAccessor(string fStrDllName)
        {
            if (this._Assemblyer == null)
            {
                string fPath = System.IO.Path.Combine(App.BaseDirectory, fStrDllName);
                if (!System.IO.File.Exists(fPath))
                {
                    throw new Exception(string.Format("您所请求的功能缺少相关文件的支持[文件完全路径:{0}]!", fPath));
                }
                this._Assemblyer = Assembly.LoadFile(fPath);
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 加载指定路径的程序集文件。
        /// </summary>
        /// <param name="fStrDllName">程序集文件完全路径。</param>
        /// <returns>程序集对象。</returns>
        public Assembly GetAssembly(string fStrDllName)
        {
            Assembly tmpAssembly = null;
            try
            {
                string fPath = System.IO.Path.Combine(App.BaseDirectory, fStrDllName);
                if (!System.IO.File.Exists(fPath))
                {
                    throw new Exception(string.Format("您所请求的功能缺少相关文件的支持[文件完全路径:{0}]!", fPath));
                }
                this._Assemblyer = Assembly.LoadFile(fPath);
                tmpAssembly = this._Assemblyer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return tmpAssembly;
        }

        #endregion
    }
}
