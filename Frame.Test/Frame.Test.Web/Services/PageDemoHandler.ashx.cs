using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frame.Service.Server.Attributes;
using System.Data;

namespace Frame.Test.Web.Services
{
    [Page("PageDemo", "Template/VMTest.htm")]
    public class PageDemoHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context) {}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        [Action]
        public object GetResult()
        {
            List<object> users = new List<object>()
            {
            new {Name = "张三", age=20},
            new {Name = "李四", age=24},
            new {Name = "王五", age=30}
            };

            Dictionary<string, string> userDict = new Dictionary<string, string>();
            userDict.Add("name1", "20");
            userDict.Add("name2", "24");
            userDict.Add("name3", "30");

            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("age", typeof(string));

            DataRow dr = dt.NewRow();
            dr["Name"] = "sdf";
            dr["age"] = "20";
            dt.Rows.Add(dr.ItemArray);

            return "hhh";
        }
    }
}