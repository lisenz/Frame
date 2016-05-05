using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Frame.Core.Reflection
{
    /// <summary>
    /// 加载处理应用程序集文件的缓存工作类。
    /// </summary>
    internal class AssemblyFastCache : FastReflectionCache<string, AppDomain>
    {
        private AppDomain _App = null;

        public AssemblyFastCache()
        {
        }

        /// <summary>
        /// 创建生成指定路径下的程序集文件对象。
        /// </summary>
        /// <param name="key">注意：这里将程序集文件的路径作为缓存列表中的键值。</param>
        /// <returns>返回一个程序集文件对象。</returns>
        private Assembly Build(string key)
        {
            Assemblyer builder = null;
            try
            {
                if (ContainsKey(key))
                {
                    object[] parms = new object[] { key };
                    BindingFlags bindings = BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance;
                    builder = (Assemblyer)this._App.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, "Frame.Core.Reflection.Assemblyer", true, bindings, null, parms, null, null);
                    return builder.Assemblyor;
                }

                this._App = Get(key);
                builder = (Assemblyer)this._App.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, "Frame.Core.Reflection.Assemblyer");
                return builder.Build(key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 从缓存中查找并获取指定键值(程序集文件路径)的程序集对象。
        /// </summary>
        /// <param name="key">获取与指定键(路径)相关联的程序集对象。</param>
        /// <param name="assembly">当此方法返回时，若找到指定键，则返回指定键相关联的程序集对象；否则，则返回assembly值的类型的默认值。</param>
        /// <returns>返回一个值，该值标识是否找到指定键。</returns>
        public bool TryGetValue(string key, out Assembly assembly)
        {
            bool isExists = false;
            assembly = null;
            try
            {
                assembly = Build(key);
                isExists = true;
                return isExists;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 卸载指定应用程序域，并从缓存中移除。
        /// </summary>
        /// <param name="key">移除与指定键(路径)相关联的程序集对象。</param>
        public void Unload(string key)
        {
            AppDomain app = Get(key);
            if (null != app)
                AppDomain.Unload(app);
            Remove(key);
        }

        protected override AppDomain Create(string key)
        {
            AppDomainSetup Setup = new AppDomainSetup();
            Setup.ShadowCopyFiles = "true";
            AppDomain app = AppDomain.CreateDomain(key, null, Setup);
            return app;
        }
    }
}
