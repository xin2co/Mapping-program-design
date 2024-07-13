using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiData
{
    class FileHelper
    {
        // 静态类，不需要实例化。

        // Read方法用于读取文件，并将标识为Id的记录列表返回。
        public static List<Epoch> Read(string Id, string pathname)
        {
            // 创建一个空列表data，用于存储读取的Epoch对象。
            var data = new List<Epoch>();

            // 尝试块用于捕获并处理可能发生的异常。
            try
            {
                // 创建一个StreamReader对象，用于读取指定路径的文件。
                var reader = new StreamReader(pathname);

                // 读取并忽略文件的第一行，通常是标题或元数据。
                reader.ReadLine();

                // 循环读取文件中的每一行，直到到达文件末尾。
                while (!reader.EndOfStream)
                {
                    // 读取文件中的下一行。
                    string line = reader.ReadLine();

                    // 检查读取的行是否为空。
                    if (line.Length > 0)
                    {
                        // 创建一个新的Epoch对象。
                        var ep = new Epoch();

                        // 调用Epoch对象的Parse方法解析行。
                        ep.Parse(line);

                        // 检查Epoch对象的Id是否与指定的Id相匹配。
                        if (Id.Equals(ep.Id))
                        {
                            // 如果匹配，将Epoch对象添加到data列表中。
                            data.Add(ep);
                        }
                    }
                }

                // 关闭StreamReader。
                reader.Close();

            }
            catch (Exception ex)
            {
                // 如果发生异常，抛出异常。
                throw ex;
            }

            // 返回读取的Epoch对象列表。
            return data;
        }

        // Write方法用于将SessionList对象的数据写入文件。
        public static void Write(SessionList data, string filename)
        {
            // 尝试块用于捕获并处理可能发生的异常。
            try
            {
                // 创建一个StreamWriter对象，用于写入指定路径的文件。
                var writer = new StreamWriter(filename);

                // 调用SessionList对象的ToString方法获取字符串表示，并写入文件。
                writer.Write(data.ToString());

                // 关闭StreamWriter。
                writer.Close();

            }
            catch (Exception ex)
            {
                // 如果发生异常，抛出异常。
                throw ex;
            }
        }
    }

}
