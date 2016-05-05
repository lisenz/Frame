using System;
using System.IO;
using System.Web;
using System.Configuration;
using System.Collections.Specialized;
//-----------------
using Frame.Core.Ioc;

namespace Frame.Core
{
    /// <summary>
    /// 表示一个应用程序，提供应用程序的相关属性。
    /// </summary>
    public sealed class App
    {
        /// <summary>
        /// 表示该应用程序的Config对象。
        /// </summary>
        private AppConfig _configuration = null;

        /// <summary>
        /// 表示一个依赖注入容器对象。
        /// </summary>
        private UnityObjectContainer _objectContainer = null;

        /// <summary>
        /// 表示当前应用程序域的预定义应用程序域属性的名称，或已定义的应用程序域属性的名称。
        /// </summary>
        private static readonly string CURRENT_APP_DATA_KEY = (typeof(App).FullName + "$Current");

        /// <summary>
        /// 表示lock机制中的锁标识对象。
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        private App()
        {
        }

        /// <summary>
        /// 创建一个已配置相关信息的应用程序。
        /// </summary>
        /// <returns>已配置相关信息的应用程序对象。</returns>
        private static App CreateInstance()
        {
            App app = new App();
            app._configuration = AppConfig.Load(app);
            app._objectContainer = UnityObjectContainer.Create(app);
            return app;
        }

        /// <summary>
        /// 注销。
        /// </summary>
        internal void Dispose()
        {
            try
            {
                this._objectContainer.Dispose();
            }
            catch
            {
            }
            try
            {
                this._configuration.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 获取该应用程序的Config对象。
        /// </summary>
        /// <returns>已配置信息的Config对象。</returns>
        public AppConfig GetConfiguration()
        {
            return this._configuration;
        }

        /// <summary>
        /// 获取当前可用的应用程序对象。
        /// </summary>
        /// <returns>当前可用的应用程序对象。</returns>
        private static App GetCurrent()
        {
            App data;
            object obj2;
            if (null != HttpContext.Current)
            {
                HttpContext current = HttpContext.Current;
                data = current.Application.Get(CURRENT_APP_DATA_KEY) as App;
                if (null == data)
                {
                    lock ((obj2 = syncRoot))
                    {
                        if (null == (data = current.Application.Get(CURRENT_APP_DATA_KEY) as App))
                        {
                            data = CreateInstance();
                            current.Application.Set(CURRENT_APP_DATA_KEY, data);
                        }
                    }
                }
                return data;
            }
            AppDomain currentDomain = AppDomain.CurrentDomain;
            data = currentDomain.GetData(CURRENT_APP_DATA_KEY) as App;
            if (null == data)
            {
                lock ((obj2 = syncRoot))
                {
                    if (null == (data = currentDomain.GetData(CURRENT_APP_DATA_KEY) as App))
                    {
                        data = CreateInstance();
                        currentDomain.SetData(CURRENT_APP_DATA_KEY, data);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 获取对象容器。
        /// </summary>
        /// <returns>返回一个可用的对象容器。</returns>
        public IObjectContainer GetObjectContainer()
        {
            return this._objectContainer;
        }

        /// <summary>
        /// 获取当前应用程序的基目录的路径。
        /// </summary>
        public static string BaseDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 获取当前应用程序的Config文件的存放文件夹。
        /// </summary>
        public static DirectoryInfo ConfigDirectory
        {
            get
            {
                return Configuration.ConfigDirectory;
            }
        }

        /// <summary>
        /// 获取当前应用程序对象的Config文件对象。
        /// </summary>
        public static AppConfig Configuration
        {
            get
            {
                return Current.GetConfiguration();
            }
        }

        /// <summary>
        /// 获取当前应用程序的Config配置文件的ConnectionStrings配置节。
        /// </summary>
        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return Configuration.ConnectionStrings;
            }
        }

        /// <summary>
        /// 获取当前应用程序对象。
        /// </summary>
        public static App Current
        {
            get
            {
                return GetCurrent();
            }
        }

        /// <summary>
        /// 获取当前对象容器。
        /// </summary>
        public static IObjectContainer ObjectContainer
        {
            get
            {
                return Current.GetObjectContainer();
            }
        }

        /// <summary>
        /// 获取当前应用程序的Config文件的AppSettings配置节。
        /// </summary>
        public static NameValueCollection Settings
        {
            get
            {
                return Configuration.AppSettings;
            }
        }
    }
}
