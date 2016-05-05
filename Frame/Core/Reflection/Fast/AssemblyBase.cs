using System;
using System.Collections.Generic;
using System.Text;
//-------------
using System.Reflection;
using System.Collections;
using System.Web;
using System.IO;

namespace Frame.Core.Reflection.Fast
{
    /// <summary>
    /// 加载处理应用程序集文件的工作类。
    /// </summary>
    public class AssemblyBase : FastReflectionCache<string, AppDomain>
    {
        #region 字段

        /// <summary>
        /// 表示一个应用程序域。
        /// </summary>
        private AppDomain _AppDomainor = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 加载处理应用程序集文件的工作类。
        /// </summary>
        public AssemblyBase()
        {
        }

        #endregion

        #region 方法

        /// <summary>
        /// 解析加载指定路径的程序集，若已存在该程序集的应用程序域，则使用缓存的程序域进行加载。
        /// </summary>
        /// <param name="fStrDllName">程序集完全路径。</param>
        /// <returns>程序集对象。</returns>
        public Assembly Resolve(string fStrDllName)
        {
            Assembly tmpAssembly = null;
            try
            {
                tmpAssembly = Load(FindFile(fStrDllName));
                //tmpAssembly = Load(fStrDllName);
            }
            catch (Exception)
            {
                throw new AssemblyException(fStrDllName);
            }
            return tmpAssembly;
        }

        /// <summary>
        /// 当缓存区不存在该程序集时，使用该方法进行加载。
        /// </summary>
        /// <param name="fStrDllName">程序集完全路径。</param>
        /// <returns>程序集对象。</returns>
        private Assembly Load(string fStrDllName)
        {
            try
            {
                this._AppDomainor = Get(fStrDllName);
                AssemblyAccessor baseLoader = (AssemblyAccessor)this._AppDomainor.CreateInstanceFromAndUnwrap("xFrame.Core.dll", "xFrame.Core.FastReflection.AssemblyAccessor");
                return baseLoader.GetAssembly(fStrDllName); ;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 卸载加载了指定程序集的应用程序域。
        /// </summary>
        /// <param name="fStrDllName">程序集的完全路径。</param>
        public void UnloadAssembly(string fStrDllName)
        {
            AppDomain app = Get(fStrDllName);
            AppDomain.Unload(app);
            Remove(fStrDllName);
        }

        /// <summary>
        /// 使用指定KEY创建一个应用程序域。
        /// </summary>
        /// <param name="key">域的KEY名称。此KEY可在用户界面中显示以标识域。</param>
        /// <returns>新创建的应用程序域。</returns>
        protected override AppDomain Create(string key)
        {
            AppDomainSetup Setup = new AppDomainSetup();
            Setup.ShadowCopyFiles = "true";
            key = key.Substring(key.LastIndexOf("\\") + 1);
            AppDomain app = AppDomain.CreateDomain(key, null, Setup);
            return app;
        }

        /// <summary>
        /// 查找获取指定文件的文件路径。
        /// </summary>
        /// <param name="file">动态链接库文件的文件名称(带后缀)。</param>
        /// <returns>若存在此动态链接库文件，则返回文件路径，否则返回空。</returns>
        public string FindFile(string file)
        {
            string path = null;
            if (null != HttpContext.Current)
            {
                path = HttpContext.Current.Server.MapPath("~") + @"\" + file;
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = System.IO.Path.Combine(baseDirectory, file);
                if (!File.Exists(path) && ((baseDirectory.ToLower().LastIndexOf(@"bin\debug") > 0) || (baseDirectory.ToLower().LastIndexOf(@"bin\release") > 0)))
                {
                    //TODO：这个文件夹路径的文件夹是建立在与bin文件夹同一级的位置
                    path = baseDirectory + @"\..\..\" + file;
                }
            }

            return path;

        }

        #endregion

    }
}
