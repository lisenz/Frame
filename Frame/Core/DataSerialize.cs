using System;
using System.IO;
using System.Data;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Frame.Core
{
    /// <summary>
    /// 提供数据压缩序列化与解压反序列化的方法。
    /// </summary>
    public sealed class DataSerialize
    {
        #region DataSet数据的压缩序列化和解压反序列化

        /// <summary>
        /// 将DataSet数据进行压缩序列化转换为二进制数组。
        /// </summary>
        /// <param name="fDstObj">DataSet对象</param>
        /// <returns>压缩序列化DataSet后的二进制数组</returns>
        public static byte[] GetDataSetBytesBySerialize(DataSet fDstObj)
        {
            try
            {
                byte[] buffer = null;
                byte[] zipBuffer = null;
                fDstObj.RemotingFormat = SerializationFormat.Binary;
                MemoryStream tmpMs = new MemoryStream();
                IFormatter tmpFormatter = new BinaryFormatter();
                tmpFormatter.Serialize(tmpMs, fDstObj);
                buffer = tmpMs.ToArray();
                zipBuffer = Compress(buffer);
                tmpMs.Close();
                return buffer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 将压缩序列化DataSet的二进制数组解压反序列化。
        /// </summary>
        /// <param name="fByteData">压缩序列化DataSet的二进制数组</param>
        /// <returns>反序列化后返回的DataSet对象</returns>
        public static DataSet GetDataSetBytesByDeserialize(byte[] fByteData)
        {
            try
            {
                MemoryStream tmpMs = new MemoryStream(fByteData);
                IFormatter tmpFormat = new BinaryFormatter();
                object tmpObj = tmpFormat.Deserialize(tmpMs);
                DataSet tmpDstData = (DataSet)tmpObj;
                tmpMs.Close();
                return tmpDstData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region DataTable数据的压缩序列化和解压反序列化

        /// <summary>
        /// 将DataTable数据进行压缩序列化转换为二进制数组。
        /// </summary>
        /// <param name="fDtObj">DataTable对象</param>
        /// <returns>压缩序列化DataTable后的二进制数组</returns>
        public static byte[] GetDataTableBytesBySerialize(DataTable fDtObj)
        {
            try
            {
                byte[] buffer = null;
                byte[] zipBuffer = null;
                if (null != fDtObj.DataSet)
                    fDtObj.DataSet.RemotingFormat = SerializationFormat.Binary;
                fDtObj.RemotingFormat = SerializationFormat.Binary;
                MemoryStream tmpMs = new MemoryStream();
                IFormatter tmpFormatter = new BinaryFormatter();
                tmpFormatter.Serialize(tmpMs, fDtObj);
                buffer = tmpMs.ToArray();
                zipBuffer = Compress(buffer);
                tmpMs.Close();
                return buffer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 将压缩序列化DataTable的二进制数组解压反序列化。
        /// </summary>
        /// <param name="fByteData">压缩序列化DataTable的二进制数组</param>
        /// <returns>反序列化后返回的DataTable对象</returns>
        public static DataTable GetDataTableBytesByDeserialize(byte[] fByteData)
        {
            try
            {
                MemoryStream tmpMs = new MemoryStream(fByteData);
                IFormatter tmpFormat = new BinaryFormatter();
                object tmpObj = tmpFormat.Deserialize(tmpMs);
                DataTable tmpDtData = (DataTable)tmpObj;
                tmpMs.Close();
                return tmpDtData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 泛型对象数据的压缩序列化和解压反序列化

        /// <summary>
        /// 将指定类型的数据进行压缩序列化转换为二进制数组。
        /// </summary>
        /// <param name="fDtObj">泛型对象</param>
        /// <returns>压缩序列化泛型对象后的二进制数组</returns>
        public static byte[] GetGenericBytesBySerialize<T>(T fDtObj)
        {
            try
            {
                byte[] buffer = null;
                byte[] zipBuffer = null;
                MemoryStream tmpMs = new MemoryStream();
                IFormatter tmpFormatter = new BinaryFormatter();
                tmpFormatter.Serialize(tmpMs, fDtObj);
                buffer = tmpMs.ToArray();
                zipBuffer = Compress(buffer);
                tmpMs.Close();
                return buffer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 将压缩序列化的二进制数组解压反序列化。
        /// </summary>
        /// <param name="fByteData">压缩序列化的二进制数组</param>
        /// <returns>反序列化后返回的指定类型的对象</returns>
        public static T GetGenericBytesByDeserialize<T>(byte[] fByteData)
        {
            try
            {
                MemoryStream tmpMs = new MemoryStream(fByteData);
                IFormatter tmpFormat = new BinaryFormatter();
                object tmpObj = tmpFormat.Deserialize(tmpMs);
                T tmpDtData = (T)tmpObj;
                tmpMs.Close();
                return tmpDtData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 压缩序列化私有方法

        /// <summary>
        /// 压缩序列化后的二进制数组。
        /// </summary>
        /// <param name="fByteData">已进行序列化的二进制数组数据。</param>
        /// <returns>压缩后的二进制数组数据。</returns>
        private static byte[] Compress(byte[] fByteData)
        {
            try
            {
                MemoryStream tmpMs = new MemoryStream();
                Stream tmpStream = new GZipStream(tmpMs, CompressionMode.Compress, true);
                tmpStream.Write(fByteData, 0, fByteData.Length);
                tmpStream.Close();
                tmpMs.Position = 0;
                byte[] tmpByteData = new byte[tmpMs.Length];
                tmpMs.Read(tmpByteData, 0, int.Parse(tmpMs.Length.ToString()));
                return tmpByteData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 解压压缩后的二进制数组。
        /// </summary>
        /// <param name="fByteData">进行压缩序列化的二进制数组数据。</param>
        /// <returns>解压缩后的二进制数组数据。</returns>
        private static byte[] Decompress(byte[] fByteData)
        {
            try
            {
                MemoryStream tmpMs = new MemoryStream(fByteData);
                Stream tmpStream = new GZipStream(tmpMs, CompressionMode.Decompress);
                byte[] tmpByteData = EtractBytesFormStream(tmpStream, fByteData.Length);
                return tmpByteData;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 从进行解压的文件流中提取压缩的二进制数组。
        /// </summary>
        /// <param name="fStream"></param>
        /// <param name="fIntByteDataLength"></param>
        /// <returns></returns>
        private static byte[] EtractBytesFormStream(Stream fStream, int fIntByteDataLength)
        {
            try
            {
                byte[] tmpByteData = null;
                int tmpIntBytesRead = 0;
                while (true)
                {
                    Array.Resize(ref tmpByteData, tmpIntBytesRead + fIntByteDataLength + 1);
                    int bytesRead = fStream.Read(tmpByteData, tmpIntBytesRead, fIntByteDataLength);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    tmpIntBytesRead += bytesRead;
                }
                Array.Resize(ref tmpByteData, tmpIntBytesRead);
                return tmpByteData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion
    }
}
