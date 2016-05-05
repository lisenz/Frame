using Frame.Core.Extensions;
using System;
using System.Reflection;
using System.Linq;

using Frame.Test.Lib;

namespace Frame.Test.Test
{   
    /// <summary>
    ///这是 FastReflectionExtensionsTest 的测试类，旨在
    ///包含所有 FastReflectionExtensionsTest 单元测试
    ///</summary>
    public class FastReflectionExtensionsTest
    {
        /// <summary>
        ///FastGetValue 的测试
        ///</summary>
        public void FastGetValueTest()
        {
            FastReflectionObject fastObj = new FastReflectionObject();

            PropertyInfo[] props = fastObj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            int count = props.Length;
            Console.WriteLine(string.Format("属性个数:{0}", count));

            foreach (PropertyInfo prop in props)
            {
                Console.WriteLine(string.Format("属性{0}:{1}", prop.Name, prop.FastGetValue(fastObj)));
            }

            fastObj.GetType().GetProperty("Name").FastSetValue(fastObj, "张立鑫");
            fastObj.GetType().GetProperty("Age").FastSetValue(fastObj, 26);
            fastObj.GetType().GetProperty("IsAuto").FastSetValue(fastObj, true);
            fastObj.GetType().GetProperty("Weight").FastSetValue(fastObj, 111.6M);

            Console.WriteLine("赋值后:");
            foreach (PropertyInfo prop in props)
            {
                Console.WriteLine(string.Format("属性{0}:{1}", prop.Name, prop.FastGetValue(fastObj)));
            }

        }

        /// <summary>
        ///FastGetValue 的测试
        ///</summary>
        public void FastGetValueTest1()
        {
            FastReflectionObject fastObj = new FastReflectionObject();
            FieldInfo[] fields = fastObj.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            int count = fields.Length;
            Console.WriteLine(string.Format("字段个数:{0}", count));

            foreach (FieldInfo field in fields)
            {
                Console.WriteLine(string.Format("属性{0}:{1}", field.Name, field.FastGetValue(fastObj)));
            }
        }

        /// <summary>
        ///FastInvoke 的测试
        ///</summary>
        public void FastInvokeTest()
        {
            FastReflectionObject fastObj = new FastReflectionObject();

            MethodInfo[] methods = fastObj.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance|BindingFlags.DeclaredOnly);

            int count = methods.Length;
            Console.WriteLine(string.Format("方法个数:{0}", count));

            Console.WriteLine(string.Format("方法{0}:{1}", "TestMethod", methods.First(m => m.Name.Equals("TestMethod")).FastInvoke(fastObj, null)));
            Console.WriteLine(string.Format("方法{0}:{1}", "TestMethod1", methods.First(m => m.Name.Equals("TestMethod1")).FastInvoke(fastObj, "hehe")));
            
            
        }
    }
}
