using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Frame.DataStore;
using System.Data;

namespace Frame.Test.Web.DaoTest
{
    public partial class BaseDaoPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnProc1_Click(object sender, EventArgs e)
        {
            BaseDao dao = DaoFactory.GetDao();
            DataSet dst = dao.DoQueryDataSet("Proc_Bank_GetList");
            IList<IDictionary<string, object>> results = dao.DoQueryDictionaries("Proc_Bank_GetList");
            
        }

        protected void btnQueryDataSet_Click(object sender, EventArgs e)
        {
            BaseDao dao = DaoFactory.GetDao();
            DataSet dst = dao.QueryDataSet("QueryBank");
            dst = dao.QueryDataSet("SELECT top 2 * FROM dbo.Bank");
        }

        protected void btnQueryDataSet1_Click(object sender, EventArgs e)
        {
            BaseDao dao = DaoFactory.GetDao();

            IDictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"Name","农村信用合作社"}
            };
            DataSet dst = dao.QueryDataSet("QueryBankByName", parameters);

            DataSet dst1 = dao.QueryDataSet("QueryBankByName1");
        }

        protected void btnQueryDataSet2_Click(object sender, EventArgs e)
        {

        }

        protected void btnQueryDataSet3_Click(object sender, EventArgs e)
        {

        }

        protected void btnQueryDataSet4_Click(object sender, EventArgs e)
        {

        }
    }
}