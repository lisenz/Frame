using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Frame.Core.Security
{
    /// <summary>
    /// 提供访问数据加密标准SHA算法的程式方法。
    /// </summary>
    public sealed class SHAEncrpytProvider
    {
        /// <summary>
        /// 被私有化的构造函数。
        /// </summary>
        private SHAEncrpytProvider()
        {
        }

        /// <summary>
        /// SHA加密方法。
        /// </summary>
        /// <param name="data">要加密的二进制数据。</param>
        /// <returns>返回加密后的字符串。</returns>
        public static string Encrypt(byte[] data)
        {
            try
            {
                HashAlgorithm sha = new SHA1CryptoServiceProvider();
                byte[] result = sha.ComputeHash(data);
                string hash = BitConverter.ToString(result).Replace("-", "");
                return hash;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
