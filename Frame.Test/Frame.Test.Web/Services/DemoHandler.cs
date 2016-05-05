using System;
using System.Web;
using Frame.Service.Server.Attributes;
using Frame.Core;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Security.Principal;
using Frame.Test.Lib;
using Frame.Test.Test;

namespace Frame.Test.Web.Services
{
    [Service("DemoHandlerService")]
    public class DemoHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //在此写入您的处理程序实现。
        }

        #endregion

        [ServiceMethod]
        public string GetDemoString(string p1, string p2)
        {
            DateTime dt = HttpContext.Current.Timestamp;
            string id = HttpContext.Current.Session.SessionID;

            string s = dt.ToString("yyyyMMddhhmmss");
            ContainerOneTest test = new ContainerOneTest();
            test.Test();


            //string session = AppSession.Get<string>("AppSession");
            return string.Format("参数p1:{0},p2:{1}", p1, p2);
        }

        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public string GetWebClientIp()
        {
            string userIP = "未获取用户IP";

            try
            {
                if (System.Web.HttpContext.Current == null
            || System.Web.HttpContext.Current.Request == null
            || System.Web.HttpContext.Current.Request.ServerVariables == null)

                    return "";
                string CustomerIP = "";

                //CDN加速后取到的IP simone 090805 
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!String.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)

                    return System.Web.HttpContext.Current.Request.UserHostAddress;

                return CustomerIP;

            }

            catch { }
            return userIP;

        }

        [ServiceMethod]
        public string GetDemoStringForJsonParam(string json)
        {
            return "JsonParamDemo";
        }

        [ServiceMethod]
        public string FileUpload(string name,string text, HttpPostedFile file)
        {
            return "OK";
        }

        [ServiceMethod]
        public IDictionary<string, string> get_public_xml()
        {
            IDictionary<string, string> publicKey = new Dictionary<string, string>()
            {
                {"Exponent","010001"},
                {"Modulus","A5705F5740720E0356D546108C41537F92654AD79047AD3A8CF4CD880CE79868D108D831DE8B8D3B0E1A554661C24C5DB583ACC46AD9525E5D39E49727EB0461030F2F2BA93A153949F56B1499AC67ABD53254179EF4BD9DC962FBE5C34C119D5D4EE2E0CB54764F928C93D3915A62A4163E8A9E7FA77C7FB11ABCA188F3FFA7"}
            };
            return publicKey;
            //return "<RSAKeyValue><Modulus>yYMKx73kAaDEnCfeE5pYbfSLu6tI+bm3Haqn9DJiCiJXhiPGvb6RRRICj77p8il8nZyWpM7dhzkRoAWRkjLhymh/o4NB4K+NWwWsqHd6fWbyFMBDSUmNQsCt7pCo8Rlyh0qxkwoBqrzDbIrpdrQFZrxfTQEtOjGm/E85EO5FJi8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        }

        [ServiceMethod]
        public string decrypt(string str)
        {
            string privateXml = "<RSAKeyValue><Modulus>pXBfV0ByDgNW1UYQjEFTf5JlSteQR606jPTNiAznmGjRCNgx3ouNOw4aVUZhwkxdtYOsxGrZUl5dOeSXJ+sEYQMPLyupOhU5SfVrFJmsZ6vVMlQXnvS9ncli++XDTBGdXU7i4MtUdk+SjJPTkVpipBY+ip5/p3x/sRq8oYjz/6c=</Modulus><Exponent>AQAB</Exponent><P>1sMD0GaBILWSvcLi1+akXIevQuLPO/3ZNMtEAEPxLSlnWTZ/vvAKTii5JiKokpd+bRSohwC0tGe32iRsbN8BKQ==</P><Q>xTTPKjg1uSfakhCZVRfeUqmPeJbmk4Z9GBx1/uhW3mdN9WujIIhzsd5T1rH+BtT+HpoVROxzbsErbZDGYO4ETw==</Q><DP>CmyKydm/2MOXbMiB1DLotWkMk7WIk4PdwBdBpLWnhialUoo3px/lkCef3P7/qaXayBahm3PoUX1bSiZMcPheCQ==</DP><DQ>IsRWqZjTT9tI22t1vNzCY0xlcNsZt3SEZVXPL6uCdR89TUE2tyuXSgpqOXWT1VyDmJ2NlmMhTqtbnqthbgFIXQ==</DQ><InverseQ>Zbm1yxjsH3J3FFfqK/0MeliXj/89zkVJonPlgPc+KuhJImH9jW5pPofyVWdh/i/i9Ddfd5d5P+tqFDzKGmjOwQ==</InverseQ><D>IMYVLRzIO3xv3EpIBvD+EJy40k3H+FsZ6Uip2tTroGbLWlwx7OtqbBOMJe6OeUZVnhraxAKC0O1+vHRLeY32TMwIdawFcB/3P8vSL1EAVgeylxRZ3BESutWvOUvsCNEF+tYp/TXXWuavNPR9xZW6bfTmLeZdPTs+lgHDTC1pOFE=</D></RSAKeyValue>";

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string strPwdToDecrypt = str;
            rsa.FromXmlString(privateXml);
            byte[] result = rsa.Decrypt(HexStringToBytes(strPwdToDecrypt), false);
            string strPwdMD5 = Encoding.Default.GetString(result);

            return strPwdMD5;
        }

        public byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }

            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }

            byte[] result = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return result;
        }
    }
}
