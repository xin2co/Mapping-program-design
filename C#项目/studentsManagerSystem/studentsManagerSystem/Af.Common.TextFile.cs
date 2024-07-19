using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Af.Common
{
    class AfTextFile
    {
        public static string Read(string filePath, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(filePath, encoding))
            {
                return sr.ReadToEnd();
            }
        }

        public static void Write(string filePath, string content, Encoding encoding)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, encoding))
            {
                sw.Write(content);
            }
        }

        // 两个静态属性
        public static Encoding UTF8
        {
            get
            {
                return UTF8Encoding.UTF8;
            }
        }
        public static Encoding GBK
        {
            get
            {
                return Encoding.GetEncoding("GBK");
            }
        }
    }
}
