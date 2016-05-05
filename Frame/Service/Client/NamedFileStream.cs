using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Frame.Service.Client
{
    /// <summary>
    /// 一个简单的数据结构，用以保存一个文件的名字,和流
    /// </summary>
    public sealed class NamedFileStream
    {
        public string Name;
        public string Filename;
        public string ContentType;
        public Stream Stream;

        public NamedFileStream() { }

        public NamedFileStream(string name, string filename, string contentType, Stream stream)
        {
            this.Name = name;
            this.Filename = filename;
            this.ContentType = contentType;
            this.Stream = stream;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
