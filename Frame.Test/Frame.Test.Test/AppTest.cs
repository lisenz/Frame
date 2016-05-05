using Frame.Core;
using System;
using Frame.Core.Ioc;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 AppTest 的测试类，旨在
    ///包含所有 AppTest 单元测试
    ///</summary>
    public class AppTest
    {
        /// <summary>
        ///App Current属性的测试
        ///</summary>
        public void AppCurrentTest()
        {
            // 获取该属性的过程中，包括了创建App对象、AppConfig对象和UnityObjectContainer对象，
            // 其中AppConfig对象和UnityObjectContainer对象作为App对象的属性存在
            App app = App.Current;            
        }

        /// <summary>
        /// App GetConfiguration方法的测试
        /// </summary>
        public void AppGetAppConfigTest()
        {
            // AppConfig对象封装了应用程序配置文件的几个属性：
            // 1.AppSettings 配置节
            // 2.ConnectionStrings 配置节
            // 3.ConfigDirectory            
            // TODO:使用该类型的对象，必须引用 System.Configuration。
            AppConfig config1 = App.Current.GetConfiguration();
            App app = App.Current;
            AppConfig config2 = app.GetConfiguration();
            AppConfig config3 = App.Configuration; // 后两种方式等同于 App.Current.GetConfiguration()。 


            // TODO:注意，ConfigDirectory属性表示存放程序指定类型文件的文件夹，该来源分三种情况，依次进行查找返回：
            //      1.应用程序配置文件中<AppSettings>配置节键值为Application.Configuration.Directory所指定路径目录的文件夹
            //      2.应用程序根目录下名称为App_Config的文件夹
            //      3.应用程序根目录下名称为Config的文件夹
            System.IO.DirectoryInfo dir = config1.ConfigDirectory;
        }

        /// <summary>
        /// App 注入容器对象的测试
        /// </summary>
        public void AppGetObjectContainerTest()
        {
            IObjectContainer container1 = App.Current.GetObjectContainer();
            IObjectContainer container2 = App.ObjectContainer;
        }

        /// <summary>
        /// App 应用程序配置文件AppSettings配置节点的测试
        /// </summary>
        public void AppSettingsTest()
        {
            NameValueCollection settings = App.Settings;
            string result = settings["AppTest"];
        }

        /// <summary>
        /// App 应用程序配置文件ConnectionStrings配置节点的测试
        /// </summary>
        public void AppConnectionStringsTest()
        {
            ConnectionStringSettingsCollection connectionStrings = App.ConnectionStrings;
            string connectionString = connectionStrings["AppTest"].ConnectionString;
            string providerName = connectionStrings["AppTest"].ProviderName;
            string name = connectionStrings["AppTest"].Name;
        }
    }
}
