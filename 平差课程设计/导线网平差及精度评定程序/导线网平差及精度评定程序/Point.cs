using System;

namespace 导线网平差及精度评定程序
{
    public class Point
    {

        public int PointNumber { get; set; }

        public double x { get; set; }

        public double y { get; set; }

        // 构造函数
        public Point(int pointNumber, double x, double y)
        {
            PointNumber = pointNumber;
            this.x = x;
            this.y = y;
        }

        public Point(double v1, double v2)
        {
            this.x = v1;
            this.y = v2;
        }
    }
}
