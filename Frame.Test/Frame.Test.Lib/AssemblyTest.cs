using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Frame.Test.Lib
{
    public class AssemblyTest : MarshalByRefObject
    {
        public Assembly Build(string path)
        {
            Assembly assembly = null;
            try
            {
                if (!File.Exists(path))
                    throw new Exception(string.Format("您所请求的功能缺少相关文件的支持[文件完全路径:{0}]!", path));
                assembly = Assembly.LoadFile(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return assembly;
        }
    }
}
