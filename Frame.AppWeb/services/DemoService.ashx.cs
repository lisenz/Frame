using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Frame.Service.Server.Attributes;
using Frame.DataStore;
using Frame.Service.Server.SqlGe;
using System.Collections;

namespace Frame.AppWeb.services
{
    [Service("demo")]
    public class DemoService : IHttpHandler
    {
        public void ProcessRequest(HttpContext context) { }

        public bool IsReusable { get { return false; } }

        [ServiceMethod]
        public object query(int page,int pagesize)
        {
            BaseDao dao = DaoFactory.GetDao("RemoteDB1");
            int total;
            int start = page == 0 ? 1 : ((page - 1) * pagesize) + 1;
            DataSet ds = dao.PageQueryDataSet("SELECT * FROM dbo.ACAuditPoint", start, pagesize, "PId", out total);

            Hashtable data = new Hashtable();
            data["Rows"] = ds.Tables[0];
            data["Total"] = total;
            return data;
        }

        [ServiceMethod]
        public object binding()
        {
            BaseDao dao = DaoFactory.GetDao("RemoteDB1");
            DataSet ds = dao.QueryDataSet(string.Format("SELECT * FROM dbo.ACAuditPoint"));
            return ds.Tables[0];
        }
    }
}