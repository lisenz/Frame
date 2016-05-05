using System;
using System.Data;
using Frame.Core;

namespace Frame.Core.Extensions
{
    public static class DataSetExtenstions
    {
        public static byte[] SerializeFromDataTable(this DataTable table)
        {
            return DataSerialize.GetDataTableBytesBySerialize(table);
        }

        public static byte[] SerializeFromDataSet(this DataSet ds)
        {
            return DataSerialize.GetDataSetBytesBySerialize(ds);
        }
    }
}
