using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;
using Frame.Core.Reflection;
using System.Reflection;
using Frame.Test.Test;

namespace Frame.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //AppTest apptest = new AppTest();
            //apptest.AppCurrentTest();

            //UnityObjectContainerTest unityObjectContainerTest = new UnityObjectContainerTest();
            //unityObjectContainerTest.RegisterByCodeTest();
            //unityObjectContainerTest.RegisterByConfigTest();

            //AppUtilityTest utilityTest = new AppUtilityTest();
            //utilityTest.FindConfigDirectoryTest();
            //utilityTest.FindConfigFileTest();
            //utilityTest.FindDirectoryTest();
            //utilityTest.FindDirectoryOrConfigTest();
            //utilityTest.GetConfigSectionTestHelper<UnityConfigurationSection>();

            //TypeExtenstionsTest typeExtenstionsTest = new TypeExtenstionsTest();
            //typeExtenstionsTest.CreateInstanceTestHelper();
            //typeExtenstionsTest.CreateInstanceTest1Helper();
            //typeExtenstionsTest.GetCustomAttributeTestHelper();

            //ThreadContextTest threadContextTest = new ThreadContextTest();
            //threadContextTest.ThreadContextConstructorTestHelper();

            //FastReflectionExtensionsTest fastReflectionExtensionsTest = new FastReflectionExtensionsTest();
            //fastReflectionExtensionsTest.FastInvokeTest();

            AssemblyBuilderTest assemblyBuilderTest = new AssemblyBuilderTest();
            assemblyBuilderTest.FastGetValueTest();




            System.Console.ReadLine();
        }
    }
}
