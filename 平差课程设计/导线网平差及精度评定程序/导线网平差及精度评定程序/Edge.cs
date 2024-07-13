// Edge.cs
using System;

namespace 导线网平差及精度评定程序
{
    public class Edge
    {
        // 边的端点
        public int Start { get; set; }
        public int End { get; set; }

        // 边长
        public double Length { get; set; }

        public Edge()
        {
        }

        // 构造函数
        public Edge(int start, int end, double length)
        {
            Start = start;
            End = end;
            Length = length;
        }



        public Edge(Point p1, Point p2)
        {
            Start = p1.PointNumber;
            End = p2.PointNumber;
            Length = CalculateLength(p1, p2);
        }

        // 计算边长
        private double CalculateLength(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        }
        public static Edge CosineLaw(double a, double b, double gammaRadians)
        {
            Edge edge = new Edge();
            edge.Length = Math.Sqrt(a * a + b * b - 2 * a * b * Math.Cos(gammaRadians));
            return edge;
        }
        public static Edge SineLaw(double a, double b, double angleRadians)
        {
            Edge edge = new Edge();
            double c = a * Math.Sin(angleRadians) / Math.Sin(b);
            edge.Length = c;
            return edge;
        }
    }
}
