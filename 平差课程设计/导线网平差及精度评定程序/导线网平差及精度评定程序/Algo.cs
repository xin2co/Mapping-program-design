using System;

namespace 导线网平差及精度评定程序
{
    public class Algo
    {

        //坐标正算
        public static Point CoordinateForwardCalculation(Point pointA, double distance, double azimuthRadians)
        {
            double x2 = pointA.x + distance * Math.Cos(azimuthRadians);
            double y2 = pointA.y + distance * Math.Sin(azimuthRadians);
            return new Point(0, x2, y2); // 这里假设点B的序号为0，可以根据实际情况调整
        }

        //坐标反算计算初始方位角
        public Angle coordinateBackwardCalculation(Point p1, Point p2)
        {
            // 计算两点之间的差值
            double deltaX = p2.x - p1.x;
            double deltaY = p2.y - p1.y;

            // 计算弧度
            double degree = Math.Atan2(Math.Abs(deltaY), Math.Abs(deltaX));
            Angle angle = new Angle();
            // 根据象限调整角度
            if (deltaX > 0 && deltaY > 0)
            {
            }
            else if (deltaX < 0 && deltaY >= 0)
            {
                // 第二象限，角度加上180°
                degree = Math.PI - degree;
            }
            else if (deltaX < 0 && deltaY <= 0)
            {
                // 第三象限，角度加上180°
                degree += Math.PI;
            }
            else if (deltaX > 0 && deltaY <= 0)
            {
                // 第四象限，角度加上360°
                degree = 2 * Math.PI - degree;
            }
            else if (deltaX == 0 && deltaY >= 0)
            {
                degree = Math.PI / 2;
            }
            else
            {
                degree = 3 * Math.PI;
            }

            // 将度数存储到Angle实例中
            angle.Radians = degree;


            // 返回Angle实例
            return angle;
        }

        //计算其他方位角
        public static Angle CalculateAzimuth(Angle angleRadians, Angle angleDifferenceRadians)
        {
            Angle azimuthRadians = new Angle();
            azimuthRadians.Radians = angleRadians.Radians - Math.PI + angleDifferenceRadians.Radians;
            while (azimuthRadians.Radians < 0)
            {
                azimuthRadians.Radians += 2 * Math.PI;
            }
            while (azimuthRadians.Radians > 2 * Math.PI)
            {
                azimuthRadians.Radians -= 2 * Math.PI;
            }
            return azimuthRadians;
        }

        //计算角度l
        public double CalculateLi(Angle alpha, Angle beta, Angle gamma)
        {
            double li = alpha.Radians -( beta.Radians - gamma.Radians);
            return li;
        }

        //计算距离
        public  double CalculateDistance(Point point1, Point point2)
        {
            double deltaX = point2.x - point1.x;
            double deltaY = point2.y - point1.y;
            double distanceSquared = deltaX * deltaX + deltaY * deltaY;
            return Math.Sqrt(distanceSquared);
        }
    }
}
