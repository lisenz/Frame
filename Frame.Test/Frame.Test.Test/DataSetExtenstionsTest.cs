using Frame.Core.Extensions;
using System;
using System.Data;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 DataSetExtenstionsTest 的测试类，旨在
    ///包含所有 DataSetExtenstionsTest 单元测试
    ///</summary>
    public class DataSetExtenstionsTest
    {
        /// <summary>
        ///DataSetSerialize 的测试
        ///</summary>
        public void DataSetSerializeTest()
        {
            DataSet ds = null; // TODO: 初始化为适当的值
            byte[] expected = null; // TODO: 初始化为适当的值
            byte[] actual;
            actual = DataSetExtenstions.SerializeFromDataSet(ds);
        }

        /// <summary>
        ///DataTableSerialize 的测试
        ///</summary>
        public void DataTableSerializeTest()
        {
            DataTable table = null; // TODO: 初始化为适当的值
            byte[] expected = null; // TODO: 初始化为适当的值
            byte[] actual;
            actual = DataSetExtenstions.SerializeFromDataTable(table);
        }
    }
}
