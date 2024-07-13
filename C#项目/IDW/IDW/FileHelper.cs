using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// 命名空间
namespace IDW
{
    // FileHelper类，提供文件读写的辅助方法
    class FileHelper
    {
        // Read方法，用于从指定文件读取数据到DataEntity对象
        public static DataEntity Read(string filename)
        {
            DataEntity data = new DataEntity(); // 创建一个新的DataEntity实例

            try
            {
                var reader = new StreamReader(filename); // 创建一个StreamReader实例用于读取文件
                while (!reader.EndOfStream) // 当未到达文件末尾时继续读取
                {
                    string line = reader.ReadLine(); // 读取一行文本
                    if (line.Length > 0) // 如果行不为空
                    {
                        Point pt = new Point(); // 创建一个新的Point实例
                        pt.Parse(line); // 解析行文本到Point实例

                        data.Add(pt); // 将解析后的Point添加到DataEntity实例
                    }
                }

                reader.Close(); // 关闭StreamReader

            }
            catch (Exception ex) // 捕获并处理可能发生的异常
            {
                throw ex; // 抛出异常
            }

            return data; // 返回填充了数据的DataEntity实例
        }

        // Write方法，用于将文本写入到指定文件
        public static void Write(string text, string filename)
        {
            try
            {
                var writer = new StreamWriter(filename); // 创建一个StreamWriter实例用于写入文件
                writer.Write(text); // 写入文本
                writer.Close(); // 关闭StreamWriter

            }
            catch (Exception ex) // 捕获并处理可能发生的异常
            {
                throw ex; // 抛出异常
            }
        }
    }
}
