using System;
using System.IO;
using System.Text;
using System.Reflection;
using Frame.Core;

namespace Frame.Core.Reflection
{
    /// <summary>
    /// 动态加载程序集的对象类[支持远程处理]。
    /// </summary>
    public class Assemblyer : MarshalByRefObject
    {
        private Assembly _Assemblyer = null;

        public Assembly Assemblyor
        {
            get { return this._Assemblyer; }
        }

        public Assemblyer()
        {
            
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="path">程序集文件的完全路径。</param>
        public Assemblyer(string path)
        {
            if (null == this._Assemblyer)
            {
                Build(path);
            }
        }

        /// <summary>
        /// 加载指定路径的程序集文件。
        /// </summary>
        /// <param name="path">程序集文件完全路径。</param>
        /// <returns>程序集对象。</returns>
        public Assembly Build(string path)
        {
            Assembly assembly = null;
            try
            {
                string dllPath = Path.Combine(App.BaseDirectory, path);
                if (!File.Exists(dllPath))
                    throw new Exception(string.Format("您所请求的功能缺少相关文件的支持[文件完全路径:{0}]!", dllPath));
                this._Assemblyer = Assembly.LoadFile(dllPath);
                assembly = this._Assemblyer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return assembly;
        }
    
    }
}
