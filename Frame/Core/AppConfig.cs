using System;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;

namespace Frame.Core
{
    /// <summary>
    /// 表示应用程序的Config类，提供对指定Config配置文件的访问。
    /// </summary>
    public sealed class AppConfig
    {
        /// <summary>
        /// 所属的应用程序对象。
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// Config文件的AppSettings配置节。
        /// </summary>
        private readonly NameValueCollection _appSettings;

        /// <summary>
        /// Config文件的ConnectionStrings配置节。
        /// </summary>
        private readonly ConnectionStringSettingsCollection _connectionStrings;

        /// <summary>
        /// 存放config文件的文件夹对象。
        /// </summary>
        private readonly DirectoryInfo _configDirectory;

        /// <summary>
        /// 默认Config文件的AppSettings配置节的默认key名称。
        /// </summary>
        public const string CONFIG_DIRECTORY_KEY = "Application.Configuration.Directory";

        /// <summary>
        /// 默认的存放Config文件的文件夹名称。
        /// </summary>
        public const string CONFIG_DIRECTORY_NAME1 = "App_Config";

        /// <summary>
        /// 默认的存放Config文件的文件夹名称。
        /// </summary>
        public const string CONFIG_DIRECTORY_NAME2 = "Config";

        /// <summary>
        /// 构造应用程序Config配置对象。
        /// </summary>
        /// <param name="app">该Config对象所属的应用程序对象。</param>
        private AppConfig(App app)
        {
            this._app = app;
            this._configDirectory = FindConfigDirectory();
            this._appSettings = ConfigurationManager.AppSettings;
            this._connectionStrings = ConfigurationManager.ConnectionStrings;
        }

        /// <summary>
        /// 释放资源对象。
        /// </summary>
        internal void Dispose()
        {
        }

        /// <summary>
        /// 为指定应用程序加载指定的Config对象以及相关属性。
        /// </summary>
        /// <param name="app">要加载Config信息的应用程序对象。</param>
        /// <returns>已构造好相关Config属性的要加载的Config对象。</returns>
        internal static AppConfig Load(App app)
        {
            return new AppConfig(app);
        }

        /// <summary>
        /// 查找并获取存放Config文件的文件夹。
        /// </summary>
        /// <returns>存放Config文件的文件夹对象。</returns>
        private static DirectoryInfo FindConfigDirectory()
        {
            DirectoryInfo info;
            if (!(AppUtility.FindDirectoryOrConfig(CONFIG_DIRECTORY_KEY, CONFIG_DIRECTORY_NAME1, out info)
                || AppUtility.FindDirectory(CONFIG_DIRECTORY_NAME2, out info)))
            {
                return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            }
            return info;
        }

        // <summary>
        /// 提供一个值，该值表示Config文件的configuration元素中的appSettings配置节。
        /// </summary>
        public NameValueCollection AppSettings
        {
            get
            {
                return this._appSettings;
            }
        }

        /// <summary>
        /// 获取表示存放Config文件的文件夹对象。
        /// </summary>
        public DirectoryInfo ConfigDirectory
        {
            get
            {
                return this._configDirectory;
            }
        }

        /// <summary>
        /// 提供一个值，该值表示Config文件的configuration元素中的connectionStrings配置节。
        /// </summary>
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return this._connectionStrings;
            }
        }
    }
}
