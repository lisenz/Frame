using System;
//----------------
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Frame.Core.Security
{
    /// <summary>
    /// 提供访问数据加密标准DES算法的程式方法。
    /// </summary>
    public sealed class DESEncrpytProvider
    {
        /// <summary>
        /// 被私有化的构造函数。
        /// </summary>
        private DESEncrpytProvider()
        {
        }

        /// <summary>
        /// 默认密钥向量
        /// </summary>
        private static byte[] _DesIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 设置DES加密密钥
        /// </summary>
        /// <param name="keyName"></param>
        private static string SetDesKey(string keyName)
        {
            byte[] desKey = Encoding.Default.GetBytes(keyName);
            return desKey.ToString().Substring(0, 8);
        }

        /// <summary>
        /// 设置新的密钥向量。
        /// </summary>
        /// <param name="IV">密钥向量字符串。</param>
        public static void SetDesIV(string IV)
        {
            byte[] desIV = Encoding.Default.GetBytes(IV);
            _DesIV = desIV;
        }


        /// <summary>
        /// DES加密方法。
        /// </summary>
        /// <param name="fEncryptString">待加密的字符串。</param>
        /// <param name="fEncryptKey">加密密钥,要求为8位。</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串。</returns>
        public static string Encrypt(string fEncryptString, string fEncryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(SetDesKey(fEncryptKey));
                byte[] rgbIV = _DesIV;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(fEncryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return fEncryptString;
            }
        }

        /// <summary>
        /// DES解密方法。
        /// </summary>
        /// <param name="fDecryptString">待解密的字符串。</param>
        /// <param name="fDecryptKey">解密密钥,要求为8位,和加密密钥相同。</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串。</returns>
        public static string Decrypt(string fDecryptString, string fDecryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(SetDesKey(fDecryptKey));
                byte[] rgbIV = _DesIV;
                byte[] inputByteArray = Convert.FromBase64String(fDecryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return fDecryptString;
            }
        }

        /// <summary>
        /// 验证方法。
        /// </summary>
        /// <param name="fSourceString">要验证的字符串。</param>
        /// <param name="fEncryptString">解密过的字符串。</param>
        /// <returns>返回一个布尔值，标识是否为同一字符串。</returns>
        public static bool Verify(string fSourceString, string fEncryptString)
        {
            try
            {
                if (fSourceString.Equals(fEncryptString))
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
