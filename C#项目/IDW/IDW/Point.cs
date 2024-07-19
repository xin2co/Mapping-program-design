using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDW
{
    class Point
    {
        // 公共成员，表示点的ID
        public string Id;
        // 公共成员，表示点的X坐标
        public double X;
        // 公共成员，表示点的Y坐标
        public double Y;
        // 公共成员，表示点的高程
        public double H;
        // 公共成员，表示点到另一个点的距离
        public double Dist;

        // 无参构造函数，初始化所有属性为0
        public Point()
        {
            X = Y = H = Dist = 0;
        }

        // 有参构造函数，初始化ID、X坐标和Y坐标
        public Point(string id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        // Parse方法，用于从字符串解析点的属性
        public void Parse(string line)
        {
            var buf = line.Split(','); // 以逗号分隔字符串
            Id = buf[0]; // 设置ID
            X = Convert.ToDouble(buf[1]); // 设置X坐标
            Y = Convert.ToDouble(buf[2]); // 设置Y坐标
            H = Convert.ToDouble(buf[3]); // 设置高程
        }

        // 重写ToString方法，返回点的字符串表示
        public override string ToString()
        {
            return $"{Id}   {X:F3}   {Y:F3}   {H:F3}"; // 格式化输出点的属性
        }
    }
}
