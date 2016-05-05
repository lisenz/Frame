using Frame.Core.Extensions;
using System;
using Frame.Test.Lib;
using Frame.Test.Lib.Attributes;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 TypeExtenstionsTest 的测试类，旨在
    ///包含所有 TypeExtenstionsTest 单元测试
    ///</summary>
    public class TypeExtenstionsTest
    {
        /// <summary>
        ///CreateInstance 的测试
        ///</summary>
        public void CreateInstanceTestHelper()
        {
            Type type = Type.GetType("Frame.Test.Lib.ContainerOneTest,Frame.Test.Lib");
            IContanerTest actual = type.CreateInstance<IContanerTest>();
        }

        /// <summary>
        ///CreateInstance 的测试
        ///</summary>
        public void CreateInstanceTest1Helper()
        {
            string typeName = "Frame.Test.Lib.ContainerOneTest,Frame.Test.Lib"; // TODO: 初始化为适当的值
            IContanerTest actual = TypeExtenstions.CreateInstance<IContanerTest>(typeName);
        }

        /// <summary>
        ///GetCustomAttribute 的测试
        ///</summary>
        public void GetCustomAttributeTestHelper()
        {
            Type type = Type.GetType("Frame.Test.Lib.Table,Frame.Test.Lib");
            bool inherit = false; // TODO: 初始化为适当的值
            NamedAttribute actual = type.GetCustomAttribute<NamedAttribute>(inherit);
        }

        /// <summary>
        ///GetType 的测试
        ///</summary>
        public void GetTypeTest()
        {
            string typeName = string.Empty; // TODO: 初始化为适当的值
            Type expected = null; // TODO: 初始化为适当的值
            Type actual;
            actual = TypeExtenstions.GetType(typeName);
        }
    }
}
