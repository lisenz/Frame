using Frame.Core;
using System;
using System.IO;
using System.Configuration;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 AppUtilityTest 的测试类，旨在
    ///包含所有 AppUtilityTest 单元测试
    ///</summary>
    public class AppUtilityTest
    {
        /// <summary>
        /// 此示例示范获取应用程序配置文件中配置节Key为Application.Configuration.Directory指定的目录下的Unity文件夹，
        /// 当不存在该配置节时，继而查找获取应用程序根目录下的App_Config文件夹下的Unity文件夹，
        /// 若根目录下不存在App_Config文件夹，则查找获取应用程序根目录下Config文件夹下的Unity文件夹，
        /// 若以上3种路径的文件夹都不存在，则放弃查找。
        ///</summary>
        public void FindConfigDirectoryTest()
        {
            DirectoryInfo dirInfo = null; 

            bool actual = AppUtility.FindConfigDirectory("Unity", out dirInfo);
        }

        /// <summary>
        /// 此示例示范获取应用程序配置文件中配置节Key为Application.Configuration.Directory指定的文件夹中的Config文件，
        /// 当不存在该配置节时，继而查找获取应用程序根目录下的App_Config文件夹下的Config文件，
        /// 若根目录下不存在App_Config文件夹，则查找获取应用程序根目录下Config文件夹下的Config文件，
        /// 若以上3种路径的文件夹下都不存在Config文件，则放弃查找。
        ///</summary>
        public void FindConfigFileTest()
        {
            FileInfo fileInfo = null; 
            bool actual = AppUtility.FindConfigFile("ConfigTest.config", out fileInfo);
        }

        /// <summary>
        /// 此示例演示获取应用程序根目录下指定名称的文件夹。
        ///</summary>
        public void FindDirectoryTest()
        {
            DirectoryInfo dirInfo = null; 
            bool actual = AppUtility.FindDirectory("App_Config", out dirInfo);
        }

        /// <summary>
        /// 此示例演示获取配置节appSettings中键值为Application.Test.Directory所指定的文件夹，当Application.Test.Directory键
        /// 所指定的文件夹不存在时，继而获取应用程序根目录下的Config文件夹。
        ///</summary>
        public void FindDirectoryOrConfigTest()
        {
            DirectoryInfo dirInfo = null;

            // Application.Test.Directory1方式为相对路径
            bool actual = AppUtility.FindDirectoryOrConfig("Application.Test.Directory1", "Config", out dirInfo);

            // Application.Test.Directory2方式为绝对路径
            actual = AppUtility.FindDirectoryOrConfig("Application.Test.Directory2", "Config", out dirInfo);
        }

        /// <summary>
        /// 此示例演示获取指定目录下的用于依赖注入的config配置文件的unity配置节对象，
        /// 即Microsoft.Practices.Unity.Configuration.dll的UnityConfigurationSection对象，
        /// 这里需引用Microsoft.Practices.Unity.Configuration.dll。
        ///</summary>
        public void GetConfigSectionTestHelper<T>()
            where T : ConfigurationSection
        {
            T actual = AppUtility.GetConfigSection<T>("unity", "ConfigTest.config");
        }

    }
}
