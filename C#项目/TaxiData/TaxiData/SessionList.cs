using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiData
{
    class SessionList
    {
        // Data属性是一个List<Session>类型的集合，用于存储Session对象。
        public List<Session> Data = new List<Session>();

        // TotalLength属性用于存储计算出的总长度。
        public double TotalLength;

        // DirctLength属性用于存储计算出的直线距离。
        public double DirctLength;

        // 构造函数，接受一个List<Epoch>类型的参数epoches。
        public SessionList(List<Epoch> epoches)
        {
            // 遍历epoches集合，创建Session对象，并添加到Data集合中。
            for (int i = 0; i < epoches.Count - 1; i++)
            {
                Session s = new Session(epoches[i], epoches[i + 1]);
                s.Sn = i; // 设置Session对象的序列号。
                Data.Add(s);
            }

            // 调用GetTotalLength方法计算总长度。
            GetTotalLength();

            // 调用GetDirctLength方法计算直线距离。
            GetDirctLength(epoches);
        }

        // 私有方法，用于计算直线距离。
        private void GetDirctLength(List<Epoch> epoches)
        {
            // 获取epoches集合的元素数量。
            int n = epoches.Count;
            // 创建一个新的Session对象，使用epoches的第一个和最后一个元素。
            Session s = new Session(epoches[0], epoches[n - 1]);
            // 将计算出的距离赋值给DirctLength属性。
            DirctLength = s.Length;
        }

        // 私有方法，用于计算总长度。
        private void GetTotalLength()
        {
            // 初始化TotalLength为0。
            TotalLength = 0;
            // 遍历Data集合中的每个Session对象，累加它们的长度。
            foreach (var d in Data)
            {
                TotalLength += d.Length;
            }
        }

        // 重写ToString方法，返回SessionList对象的字符串表示。
        public override string ToString()
        {
            // 创建一个字符串变量line，用于存储输出信息。
            string line = "------------速度和方位角计算结果----------\r\n";
            // 遍历Data集合中的每个Session对象，将它们的字符串表示添加到line中。
            foreach (var d in Data)
            {
                line += d.ToString() + "\r\n";
            }
            // 添加距离计算结果的标题。
            line += "------------距离计算结果-----------------\r\n";
            // 添加总长度和直线距离的值到line中。
            line += $"累积距离：{TotalLength:f3} (km)\r\n";
            line += $"首尾直线距离： {DirctLength:f3} (km)";

            // 返回line字符串。
            return line;
        }
    }

}
