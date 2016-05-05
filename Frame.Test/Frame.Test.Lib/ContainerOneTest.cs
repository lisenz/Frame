using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;

namespace Frame.Test.Lib
{
    public class ContainerOneTest : IContanerTest
    {
        public ContainerOneTest()
        {
            
        }
        
        public string GetValue()
        {
            return "ContainerOneTest";
        }

        public void Test()
        {


            DateTime dt = HttpContext.Current.Timestamp;
            string id = HttpContext.Current.Session.SessionID;
            string s = dt.ToString("yyyyMMddhhmmss");
        }
    }
}
