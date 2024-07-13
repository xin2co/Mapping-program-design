using System;

namespace 导线网平差及精度评定程序
{
    public class AngleEquation
    {
        // 定义方程的系数
        public double CoefficientX1 { get; set; }
        public double CoefficientY1 { get; set; }
        public double CoefficientX2 { get; set; }
        public double CoefficientY2 { get; set; }
        public double CoefficientX3 { get; set; }
        public double CoefficientY3 { get; set; }
        public double CoefficientX4 { get; set; }
        public double CoefficientY4 { get; set; }

        public AngleEquation()
        {
            CoefficientX1 = 0;
            CoefficientY1 = 0;
            CoefficientX2 = 0;
            CoefficientY2 = 0;
            CoefficientX3 = 0;
            CoefficientY3 = 0;
            CoefficientX4 = 0;
            CoefficientY4 = 0;
        }

        public AngleEquation(double coefficientX1, double coefficientY1, double coefficientX2, double coefficientY2, double coefficientX3, double coefficientY3, double coefficientX4, double coefficientY4)
        {
            CoefficientX1 = coefficientX1;
            CoefficientY1 = coefficientY1;
            CoefficientX2 = coefficientX2;
            CoefficientY2 = coefficientY2;
            CoefficientX3 = coefficientX3;
            CoefficientY3 = coefficientY3;
            CoefficientX4 = coefficientX4;
            CoefficientY4 = coefficientY4;
        }
    }
}
