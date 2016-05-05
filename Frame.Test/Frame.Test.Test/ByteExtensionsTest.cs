using Frame.Core.Extensions;
using System;
using System.Data;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 ByteExtensionsTest 的测试类，旨在
    ///包含所有 ByteExtensionsTest 单元测试
    ///</summary>
    public class ByteExtensionsTest
    {
        /// <summary>
        ///DataSetDeserialize 的测试
        ///</summary>
        public void DataSetDeserializeTest()
        {
            byte[] data = null; // TODO: 初始化为适当的值
            DataSet expected = null; // TODO: 初始化为适当的值
            DataSet actual;
            
            actual = ByteExtensions.DeserializeToDataSet(data);
        }

        /// <summary>
        ///DataTableDeserialize 的测试
        ///</summary>
        public void DataTableDeserializeTest()
        {
            byte[] data = null; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = ByteExtensions.DeserializeToDataTable(data);
        }
    }
}
