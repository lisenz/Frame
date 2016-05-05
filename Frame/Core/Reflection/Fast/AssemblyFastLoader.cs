using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Web;

namespace Frame.Core.Reflection.Fast
{
    public class AssemblyFastLoader : FastReflectionCache<string, AppDomain>
    {
        private RemoteLoader _Loader;
        public static string ApplicationName = "Ref";

        public object Resolve(string assemblyFile, string typeName, string methodName, params object[] arguments)
        {
            string path = FindFile(Path.Combine(ApplicationName, assemblyFile));
            if (!File.Exists(path))
            {
                throw new Exception(string.Format("您所请求的功能缺少相关文件的支持[文件完全路径:{0}]!", path));
            }

            AppDomain app = Get(assemblyFile);
            this._Loader = (RemoteLoader)app.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().FullName, typeof(RemoteLoader).FullName);
            return this._Loader.Get(path, typeName, methodName, arguments);
        }

        public T Resolve<T>(string assemblyFile, string typeName, string methodName, params object[] arguments)
        {
            object result = Resolve(assemblyFile, typeName, methodName, arguments);
            if (result == null)
                return default(T);
            return (T)result;
        }

        public void UnLoad(string assemblyFile)
        {
            AppDomain app = Get(assemblyFile);
            AppDomain.Unload(app);
            Remove(assemblyFile);
        }

        protected override AppDomain Create(string key)
        {
            AppDomainSetup Setup = new AppDomainSetup();
            Setup.ApplicationName = ApplicationName;
            Setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            Setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private");
            Setup.CachePath = Setup.ApplicationBase;
            Setup.ShadowCopyFiles = "true";
            Setup.ShadowCopyDirectories = Setup.ApplicationBase;//影像副本目录
            AppDomain app = AppDomain.CreateDomain(key, null, Setup);

            return app;
        }

        /// <summary>
        /// 查找获取指定文件的文件路径。
        /// </summary>
        /// <param name="file">动态链接库文件的文件名称(带后缀)。</param>
        /// <returns>若存在此动态链接库文件，则返回文件路径，否则返回空。</returns>
        private string FindFile(string file)
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
    }
}
