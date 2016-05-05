using System;

namespace Frame.Service.Client.Attributes
{
    /// <summary>
    /// 该标注描述了Entity与结果集中的表的对应关系。
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DataEntityAttribute : Attribute
    {

    }
}
