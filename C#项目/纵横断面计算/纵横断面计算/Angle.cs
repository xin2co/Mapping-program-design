using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 纵横断面计算
{
    public class Angle
    {
        public int Degrees { get; private set; }
        public int Minutes { get; private set; }
        public double Seconds { get; private set; }

        public Angle(double degrees)
        {

            Degrees = (int)degrees;
            Minutes = (int)((degrees - Degrees) * 60);
            Seconds = ((degrees - Degrees) * 60 - Minutes) * 60;
        }

        public Angle()
        {
        }

        public double ToRadians()
        {
            return Degrees * (Math.PI / 180.0) + Minutes * (Math.PI / 10800.0) + Seconds * (Math.PI / 648000.0);
        }

        public static Angle CalculateBearing(Point pointA, Point pointB)
        {

            double alpha_radians = Math.Atan2(pointB.y - pointA.y, pointB.x - pointA.x);

            double alpha_degrees = alpha_radians * (180.0 / Math.PI);

            return new Angle(alpha_degrees);
        }

    }
}
