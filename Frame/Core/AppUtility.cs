using System;
using System.IO;
using System.Web;
using System.Configuration;
using System.Runtime.InteropServices;

namespace Frame.Core
{
    public static class AppUtility
    {
        /// <summary>
        /// 查找指定名称的文件夹，并返回查找结果。
        /// </summary>
        /// <param name="dirName">要查找的文件夹的名称。</param>
        /// <param name="dirInfo">查找到的文件夹对象。</param>
        /// <returns>提供一个值，该值指示是否存在指定名称的文件夹。</returns>
        public static bool FindDirectory(string dirName, out DirectoryInfo dirInfo)
        {
            string path = null;
            if (null != HttpContext.Current)
            {
                // TODO:表示当前应用级程序的目录下指定名称的文件夹。
                // path = HttpContext.Current.Server.MapPath("~") + @"\" + dirName;
                // 尽量不要用MapPath，HttpRuntime.AppDomainAppPath才是更安全的选择.
                path = HttpRuntime.AppDomainAppPath + @"\" + dirName;
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = Path.Combine(baseDirectory, dirName);
                if (!Directory.Exists(path) && ((baseDirectory.ToLower().LastIndexOf(@"bin\debug") > 0) 
                    || (baseDirectory.ToLower().LastIndexOf(@"bin\release") > 0)))
                {
                    // TODO:当在debug或者release文件夹中不存在指定文件夹(dirName)时，
                    //      将查找目录重新指定为与bin文件夹在同一级目录的位置。
                    path = baseDirectory + @"..\..\" + dirName;
                }
            }

            if (!Directory.Exists(path))
            {
                dirInfo = null;
                return false;
            }

            dirInfo = new DirectoryInfo(path);
            return true;
        }

        /// <summary>
        /// 查找Config文件appSettings配置节中指定key的值所标识的名称的文件夹
        /// 或者基目录下指定名称的文件夹，并返回查找结果。
        /// </summary>
        /// <param name="configKey">Config文件中AppSettings节的key名称。</param>
        /// <param name="dirName">查找的文件夹的名称。</param>
        /// <param name="dirInfo">查找到的文件夹对象。</param>
        /// <returns>返回一个值，该值指示是否存在指定名称的文件夹。</returns>
        public static bool FindDirectoryOrConfig(string configKey, string dirName, out DirectoryInfo dirInfo)
        {
            string str = ConfigurationManager.AppSettings[configKey];
            if (!string.IsNullOrEmpty(str))
            {
                if (str.StartsWith("~") && (null != HttpContext.Current))
                {
                    // TODO:当运行环境为WEB程序,这里str的文本内容格式为~/...,表示当前应用级程序的目录下的文件夹。
                    // str = HttpContext.Current.Server.MapPath(str);
                    // 尽量不要用MapPath，HttpRuntime.AppDomainAppPath才是更安全的选择.
                    str = (HttpRuntime.AppDomainAppPath + @"\" + str);
                }
                if (str.StartsWith(".") && (null == HttpContext.Current))
                {
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string dir = str.Substring(str.IndexOf("\\") + 1);
                    str = Path.Combine(baseDirectory, dir);
                    if (!Directory.Exists(str) && ((baseDirectory.ToLower().LastIndexOf(@"bin\debug") > 0)
                        || (baseDirectory.ToLower().LastIndexOf(@"bin\release") > 0)))
                    {
                        // TODO:当在debug或者release文件夹中不存在指定文件夹(dirName)时，
                        //      将查找目录重新指定为与bin文件夹在同一级目录的位置。
                        str = baseDirectory + @"..\..\" + dir;
                    }
                }

                if (Directory.Exists(str))
                {
                    dirInfo = new DirectoryInfo(str);
                    return true;
                }
                dirInfo = null;
                return false;
            }
            return FindDirectory(dirName, out dirInfo);
        }

        /// <summary>
        /// 查找存放Config文件的文件夹，并返回查找结果。
        /// TODO：这里的文件夹指定3个固定路径的文件夹中的文件夹  
        ///     1.应用程序配置文件中[AppSettings]配置节键值为Application.Configuration.Directory所指定路径目录的文件夹；
        ///     2.应用程序根目录下名称为App_Config的文件夹；
        ///     3.应用程序根目录下名称为Config的文件夹。
        /// </summary>
        /// <param name="dirName">要查找的文件夹的名称。</param>
        /// <param name="dirInfo">若存在指定名称的文件夹，输出该文件夹对象。</param>
        /// <returns>提供一个值，该值指示是否存在指定名称的文件夹。</returns>
        public static bool FindConfigDirectory(string dirName, out DirectoryInfo dirInfo)
        {
            string path = App.ConfigDirectory.FullName + @"\" + dirName;
            if (!Directory.Exists(path))
            {
                dirInfo = null;
                return false;
            }
            dirInfo = new DirectoryInfo(path);
            return true;
        }

        /// <summary>
        /// 查找指定名称的Config文件，并返回查找结果。
        /// TODO：这里的文件指定以下3个固定路径的文件夹中的Config文件  
        ///      1.应用程序配置文件中[AppSettings]配置节键值为Application.Configuration.Directory所指定路径目录的文件夹；
        ///      2.应用程序根目录下名称为App_Config的文件夹；
        ///      3.应用程序根目录下名称为Config的文件夹。
        /// </summary>
        /// <param name="fileName">要查找的Config文件的名称，带拓展名称。</param>
        /// <param name="fileInfo">查找到的Config文件对象。</param>
        /// <returns>提供一个值，该值指示是否查找到指定文件。</returns>
        public static bool FindConfigFile(string fileName, out FileInfo fileInfo)
        {
            string path = App.ConfigDirectory.FullName + @"\" + fileName;
            if (File.Exists(path))
            {
                fileInfo = new FileInfo(path);
                return true;
            }
            fileInfo = null;
            return false;
        }

        /// <summary>
        /// 获取指定Config文件中的指定名称的配置节对象。
        /// </summary>
        /// <typeparam name="T">继承ConfigurationSection的配置节对象类型。</typeparam>
        /// <param name="sectionName">配置节的名称。</param>
        /// <param name="configFileName">Config文件的名称。</param>
        /// <returns>配置节对象。</returns>
        public static T GetConfigSection<T>(string sectionName, string configFileName) where T : ConfigurationSection
        {
            FileInfo info;
            if (FindConfigFile(configFileName, out info))
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = info.FullName
                };
                return (ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None).GetSection(sectionName) as T);
            }
            return (ConfigurationManager.GetSection(sectionName) as T);
        }
    }
}
