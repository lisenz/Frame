using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Frame.Service.Client;
using System.IO;

namespace Frame.Test.Web.ServiceTest
{
    public partial class AjaxServiceBackground : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"CommandName","sqlid:QueryBankByName"},
                {"Params",new {Name="工商银行"}}
            };
            Ajax.AsynRequest("http://localhost:29258/services/DataService/Execute", data: parameters, contentType: Ajax.CONTENT_TYPE_APPLICATION_JSON,
            onSuccess: (request, response) =>
            {
            },
            onError: (request, response, ex) =>
            {
            });
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"CommandName","sqlid:QueryBankByName"},
                {"Params",new {Name="工商银行"}}
            };
            Ajax.AsynPost("http://localhost:29258/services/DataService/Execute", data: parameters, contentType: Ajax.CONTENT_TYPE_APPLICATION_JSON,
            onSuccess: (request, response) =>
            {
                if(response.OK)
                {
                    string text = response.Text;
                }
            },
            onError: (request, response, ex) =>
            {
            });
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"CommandName","sqlid:QueryBankByName"},
                {"Params",new {Name="工商银行"}}
            };
            AjaxResponse response = Ajax.SynRequest("http://localhost:29258/services/DataService/Execute",
            data: parameters, contentType: Ajax.CONTENT_TYPE_APPLICATION_JSON);

            if (response.OK)
            {
                string text = response.Text;
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {            
            IDictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"CommandName","sqlid:QueryBankByName"},
                {"Params",new {Name="工商银行"}}
            };
            Ajax.AsynGet("http://localhost:29258/services/DataService/Execute", data: parameters, contentType: Ajax.CONTENT_TYPE_APPLICATION_JSON,
            onSuccess: (request, response) =>
            {
                if (response.OK)
                {
                    string text = response.Text;
                }
            },
            onError: (request, response, ex) =>
            {
            });
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"CommandName","sqlid:QueryBankByName"},
                {"Params",new {Name="工商银行"}}
            };
            AjaxResponse response = Ajax.SynGet("http://localhost:29258/services/DataService/Execute",
            data: parameters, contentType: Ajax.CONTENT_TYPE_APPLICATION_JSON);

            if (response.OK)
            {
                string text = response.Text;
            }
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            IDictionary<object, object> parameters = new Dictionary<object, object>()
            {
                {new { name = "file",text="value" },new[] {new NamedFileStream("file", "arrow.jpg", "png", new FileStream(@"E:\Project\Actual\Frame\Frame.Test\Frame.Test.Web\Files\arrow.png",FileMode.Open))}}
            };
            //HttpClient.AsynFileCall("http://localhost:29258/services/DemoHandlerService/FileUpload", parameters, result =>
            //{
            //    string res = result.Value;
            //});

            //ReturnResult<string> r = HttpClient.SynFileCall("http://localhost:29258/services/DemoHandlerService/FileUpload", parameters);

            ReturnResult<DataValue> r = HttpClient.SynFileCall<DataValue>("http://localhost:29258/services/DemoHandlerService/FileUpload", parameters);

            //DataValue v = r.Value;
            string s = r.ValueFor<string>();
        }
    }
}