using System;

namespace 导线网平差及精度评定程序
{
    public class Angle
    {
        public int AngleNumber { get; set; }
        public int Degrees { get; set; }
        public int Minutes { get; set; }
        public double Seconds { get; set; }
        public double Radians { get; set; }

        // 构造函数，接受度分秒格式
        public Angle(int angleNumber, int degrees, int minutes, double seconds)
        {
            AngleNumber = angleNumber;
            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
            Radians = ToRadians();
        }

        // 构造函数，传入弧度
        public Angle(int angleNumber, double radians)
        {
            AngleNumber = angleNumber;
            Radians = radians;
            RadiansToDegrees(radians);
        }

        public Angle()
        {
        }

        // 将以弧度为单位的浮点数转换为度分秒
        private void RadiansToDegrees(double radians)
        {
            double degrees = radians * (180.0 / Math.PI);
            FromDecimalDegrees(degrees);
        }

        // 将以度为单位的浮点数转换为度分秒
        private void FromDecimalDegrees(double decimalDegrees)
        {
            Degrees = (int)decimalDegrees;
            double minutesPart = (decimalDegrees - Degrees) * 60;
            Minutes = (int)minutesPart;
            Seconds = (minutesPart - Minutes) * 60;
        }

        public static Angle Parse(string angleString, int angleNumber)
        {
            double decimalDegrees = double.Parse(angleString);
            int degrees = (int)decimalDegrees;
            double minutesPart = (decimalDegrees - degrees) * 100;
            int minutes = (int)minutesPart;
            double seconds = (minutesPart - minutes) * 100;

            return new Angle(angleNumber, degrees, minutes, seconds);
        }

        // 实现弧度加法
        public static Angle operator +(Angle a1, Angle a2)
        {
            return new Angle(0, a1.Radians + a2.Radians);
        }

        // 实现弧度减法
        public static Angle operator -(Angle a1, Angle a2)
        {
            return new Angle(0, a1.Radians - a2.Radians);
        }

        // 将度分秒转换为弧度
        private double ToRadians()
        {
            return (Degrees + Minutes / 60.0 + Seconds / 3600.0) * Math.PI / 180.0;
        }

        // 秒转弧度的方法
        public static double SecondsToRadians(double seconds)
        {
            return seconds * (Math.PI / (180.0 * 3600));
        }
    }
}
