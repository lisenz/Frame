using Frame.Core.Reflection;
using System;
using Frame.Core;
using System.Reflection;
using System.IO;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 AssemblyBuilderTest 的测试类，旨在
    ///包含所有 AssemblyBuilderTest 单元测试
    ///</summary>
    public class AssemblyBuilderTest
    {
        /// <summary>
        ///Build 的测试
        ///</summary>
        public void BuildTest()
        {
            // 为“Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly”创建专用访问器失败
        }

        public void FastGetValueTest()
        {
            //string prefix = App.BaseDirectory;
            //string path = "WindowOS.Modularity.lib.dll";
            //object obj = AssemblyBuilder.Build().FastGetValue(path, "WindowOS.Modularity.lib.ModularityModule");

            Assembly assembly = null;
            try
            {
                //AppDomainSetup setup = new AppDomainSetup();
                //setup.ShadowCopyFiles = "true";
                AppDomain app = AppDomain.CreateDomain("Domain");
                string dllPath = @"E:\Project\Actual\Frame\Frame.Console\Ref\WindowOS.Modularity.lib.dll";
                if (!File.Exists(dllPath))
                    throw new Exception(string.Format("您所请求的功能缺少相关文件的支持[文件完全路径:{0}]!", dllPath));
                Assemblyer builder = (Assemblyer)app.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, "Frame.Core.Reflection.Assemblyer");
                assembly = builder.Build(dllPath);
                dllPath = @"E:\Project\Actual\Frame\Frame.Console\Ref\WindowOS.Modularity.lib.dll";
                assembly = builder.Build(dllPath);
                object obj = assembly.CreateInstance("WindowOS.Modularity.lib.ModularityModule");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
