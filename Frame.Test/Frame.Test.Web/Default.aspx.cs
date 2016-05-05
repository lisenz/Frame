using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Frame.Core;
using System.Collections;
using System.Data;
using Frame.Core.Extensions;

namespace Frame.Test.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AppSession.Set("AppSession", "张立鑫");

        }

        private static void LinqTest()
        {
            DataTable dt = new DataTable("table");
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("name", typeof(string));

            DataRow dr = dt.NewRow();
            dr["id"] = 1;
            dr["name"] = "A";
            dt.Rows.Add(dr.ItemArray);

            dr = dt.NewRow();
            dr["id"] = 2;
            dr["name"] = "B";
            dt.Rows.Add(dr.ItemArray);

            dr = dt.NewRow();
            dr["id"] = 3;
            dr["name"] = "C";
            dt.Rows.Add(dr.ItemArray);

            dr = dt.NewRow();
            dr["id"] = 4;
            dr["name"] = "D";
            dt.Rows.Add(dr.ItemArray);


            var results = from s in dt.AsEnumerable()
                          where Convert.ToInt32(s["id"]) > 2
                          select new { key = s["id"], item = s["name"] };
            Type type = results.GetType();

            object oo = results;

            foreach (var r in results)
            {
                object o = r;
                object rr = o.GetType().GetProperty("item").FastGetValue(o);
            }
        }
    }
}