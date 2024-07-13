using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdmWrite
{
    class FileHelper
    {
        public static DataEntity Read(string filename)
        {
            DataEntity data = new DataEntity();
            try
            {
                var reader = new StreamReader(filename);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Length > 0)
                    {
                        Point pt = new Point();
                        pt.Parse(line);
                        data.Add(pt);
                    }
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return data;
        }

        public static void Write(String text,string filename)
        {
            try
            {
                var writer = new StreamWriter(filename);
                writer.Write(text);
                writer.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    } 
}
