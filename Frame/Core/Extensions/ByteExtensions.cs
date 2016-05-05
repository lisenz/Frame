using System;
using System.Data;

namespace Frame.Core.Extensions
{
    public static class ByteExtensions
    {
        public static DataTable DeserializeToDataTable(this byte[] data)
        {
            return DataSerialize.GetDataTableBytesByDeserialize(data);
        }

        public static DataSet DeserializeToDataSet(this byte[] data)
        {
            return DataSerialize.GetDataSetBytesByDeserialize(data);
        }
    }
}
