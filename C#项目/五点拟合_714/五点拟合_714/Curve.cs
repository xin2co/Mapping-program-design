using System;
using System.Collections.Generic;

namespace 五点拟合_714
{

    public class Curve
    {

        public double E0, E1, E2, E3, F0, F1, F2, F3;
        public double R;
        public double A1, B1, A2, B2, A3, B3, A4, B4, W2, W3, A0, B0, CosTheta, SinTheta;
        public Point pi_2, pi_1, p, pi1, pi2;

        public Curve(int i, List<Point> points)
        {
             pi_2 = new Point();
            if (i == 1)
            {
                pi_2 = points[(points.Count - 1)];
            }
            else
            {
                pi_2 = points[(i - 2) < 0 ? (points.Count - 2) : (i - 2)];
            }

            pi_1 = points[(i - 1) < 0 ? (points.Count - 1) : (i - 1)];
            p = points[i];
            pi1 = points[(i + 1) > points.Count - 1 ? 0 : (i + 1)];
            pi2 = new Point();
            if (i == points.Count - 2)
            {
                pi2 = points[0];
            }
            else
            {
                pi2 = points[(i + 2) > points.Count - 1 ? 1 : (i + 2)];
            }
            double r = Math.Sqrt(Math.Pow(pi1.x - p.x, 2) + Math.Pow(pi1.y - p.y, 2));
            // 使用差分近似梯度
            double a1 = pi_1.x - pi_2.x;
            double b1 = pi_1.y - pi_2.y;
            double a2 = p.x - pi_1.x;
            double b2 = p.y - pi_1.y;
            double a3 = pi1.x - p.x;
            double b3 = pi1.y - p.y;
            double a4 = pi2.x - pi1.x;
            double b4 = pi2.y - pi1.y;

            double w2 = Math.Abs(a3 * b4 - a4 * b3);
            double w3 = Math.Abs(a1 * b2 - a2 * b1);
            double a0 = w2 * a2 + w3 * a3;
            double b0 = w2 * b2 + w3 * b3;

            double cosTheta = a0 / Math.Sqrt(Math.Pow(a0, 2) + Math.Pow(b0, 2));
            double sinTheta = b0 / Math.Sqrt(Math.Pow(a0, 2) + Math.Pow(b0, 2));
            E0 = p.x;
            E1 = R * CosTheta;
            F0 = p.y;
            F1 = R * SinTheta;
        }
        public List<Point> GetCurvePoints(double interval)
        {
            List<Point> curvePoints = new List<Point>();
            for (double z = 0; z <= 1; z += interval)
            {
                double x = E0 + E1 * z + E2 * z * z + E3 * z * z * z;
                double y = F0 + F1 * z + F2 * z * z + F3 * z * z * z;
                curvePoints.Add(new Point { x = x, y = y });
            }
            return curvePoints;
        }
    }
}
