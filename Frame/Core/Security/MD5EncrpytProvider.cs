using System;
//----------
using System.Text;
using System.Security.Cryptography;

namespace Frame.Core.Security
{
    /// <summary>
    /// 提供访问数据加密标准MD5算法的程式方法。
    /// </summary>
    public sealed class MD5EncrpytProvider
    {
        /// <summary>
        /// 被私有化的构造函数。
        /// </summary>
        private MD5EncrpytProvider()
        {
        }


        /// <summary>
        /// MD5加密方法。
        /// </summary>
        /// <param name="fEncryptString">要加密的字符串。</param>
        /// <returns>返回加密后的字符串。</returns>
        public static string Encrypt(string fEncryptString)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] data = Encoding.UTF8.GetBytes(fEncryptString);
                byte[] result = md5.ComputeHash(data);
                string strTmp = string.Empty;
                foreach (byte b in result)
                {
                    strTmp += b.ToString("x").PadLeft(2, '0');
                }
                return strTmp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 验证方法。
        /// </summary>
        /// <param name="fSourceString">要验证的字符串。</param>
        /// <param name="fEncryptString">加密过的字符串。</param>
        /// <returns>返回一个布尔值，标识是否为同一字符串。</returns>
        public static bool Verify(string fSourceString, string fEncryptString)
        {
            try
            {
                string tmpStrVerify = Encrypt(fSourceString);
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                if (comparer.Compare(tmpStrVerify, fEncryptString) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
