using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Frame.Core.Security
{
    /// <summary>
    /// 提供访问数据加密标准HMACSHA1算法的程式方法。
    /// </summary>
    public sealed class HMACSHA1EncrpytProvider
    {
        /// <summary>
        /// 被私有化的构造函数。
        /// </summary>
        private HMACSHA1EncrpytProvider()
        {
        }

        ///<summary>生成签名</summary>
        ///<param name="signStr">被加密串</param>
        ///<param name="secret">加密密钥</param>
        ///<returns>签名</returns>
        public static string Signature(string signStr, string secret)
        {
            using (HMACSHA1 mac = new HMACSHA1(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(signStr));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
