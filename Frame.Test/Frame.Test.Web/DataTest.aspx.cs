using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Frame.Core;
using Frame.Data;
using Frame.Data.SqlGe;
using System.Configuration;
using System.Data;

namespace Frame.Test.Web
{
    public partial class DataTest : System.Web.UI.Page
    {
        DataBase db;

        protected void Page_Load(object sender, EventArgs e)
        {
            string connectString = App.ConnectionStrings["DefaultDB"].ConnectionString;
            db = new SqlGeClient(connectString);
            
        }

        protected void btnServiceLocation_Click(object sender, EventArgs e)
        {

            object[] parameters = new object[] { 1 };
            DataSet dst = db.ExecuteDataSet("Proc_Bank_GetModel", parameters);
        }
    }
}