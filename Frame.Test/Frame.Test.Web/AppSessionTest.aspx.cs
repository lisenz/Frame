using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Frame.Core;

namespace Frame.Test.Web
{
    public partial class AppSessionTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSession_Click(object sender, EventArgs e)
        {
            string session = AppSession.Get<string>("AppSession");
            
        }
    }
}