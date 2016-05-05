using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frame.Service.Server.Attributes;
using Frame.Core;

namespace Frame.Test.Web.Services
{
    /// <summary>
    /// SessionHandler 的摘要说明
    /// </summary>
    [Service("session")]
    public class SessionHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        [ServiceMethod]
        public string GetSession()
        {
            string str = AppSession.Get<string>("AppSession");
            return str;
        }
    }
}