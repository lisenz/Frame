using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Frame.Core;

namespace Frame.Core
{
    public class SimpleListener : TraceListener
    {
        public override void Write(string message)
        {
            string path = Path.Combine(App.BaseDirectory, string.Format("logs\\{0}.log", DateTime.Now.ToString("yyyyMMdd")));
            File.AppendAllText(path, message);
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }
    }
}
