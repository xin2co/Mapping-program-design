using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 大地线长_5_5
{
    class Calculation
    {
        public struct Input
        {
            /// <summary>
            /// 离心率
            /// </summary>
            public double e2;
            public double e12;
            /// <summary>
            /// 第一偏心率
            /// </summary>
            public double a;
            public double b;
            public double L1;
            public double B1;
            /// <summary>
            /// 大地方位角
            /// </summary>
            /// <summary>
            /// 大地线长
            /// </summary>
            //反算
            public double L2;
            public double B2;
        }



        public double Bessal_P(Input input)
        {
            #region 变量
            double u1;
            double u2;
            double L;
            double a1;
            double a2;
            double b1;
            double b2;
            double delta0 = 0;
            double p;
            double q;
            double lamda;
            double sigema;
            double A1;
            double Sin_sigema;
            double Cos_sigema;
            double sin_A0;
            double sigma1;
            double alpha = 0;
            double beta;
            double gamma;
            double epsilon;
            double k2;
            double A = 0;
            double B = 0;
            double C = 0;
            double xs;
            double S;

            #endregion

            //辅助计算
            u1 = Math.Atan(Math.Sqrt(1 - (input.e2) * (input.e2)) * Math.Tan(input.B1));
            u2 = Math.Atan(Math.Sqrt(1 - (input.e2) * (input.e2)) * Math.Tan(input.B2));
            L = input.L2 - input.L1;
            a1 = Math.Sin(u1) * Math.Sin(u2);
            a2 = Math.Cos(u1) * Math.Cos(u2);
            b1 = Math.Cos(u1) * Math.Sin(u2);
            b2 = Math.Sin(u1) * Math.Cos(u2);

            //逐次趋近法
            lamda = L + delta0;
            while (true)
            {
                p = Math.Cos(u2) * Math.Sin(lamda);
                q = b1 - b2 * Math.Cos(lamda);
                A1 = Math.Atan(p / q);
                if (p > 0 && q > 0)
                    A1 = Math.Abs(A1);
                else if (p > 0 && q < 0)
                    A1 = Math.PI - Math.Abs(A1);
                else if (p < 0 && q < 0)
                    A1 = Math.Abs(A1) + Math.PI;
                else
                    A1 = 2 * Math.PI - Math.Abs(A1);
                if (A1 < 0)
                {
                    A1 = A1 + 2 * Math.PI;
                }
                else if (A1 > 2 * Math.PI)
                {
                    A1 = A1 - 2 * Math.PI;
                }
                Sin_sigema = p * Math.Sin(A1) + q * Math.Cos(A1);
                Cos_sigema = a1 + a2 * Math.Cos(lamda);
                sigema = Math.Atan(Sin_sigema / Cos_sigema);
                if (Cos_sigema > 0)
                {
                    sigema = Math.Abs(sigema);
                }
                else if (Cos_sigema < 0)
                {
                    sigema = Math.PI - Math.Abs(sigema);
                }
                sin_A0 = Math.Cos(u1) * Math.Sin(A1);

                alpha = (input.e2 / 2 + input.e2 * input.e2 / 8 + input.e2 * input.e2 * input.e2 / 16) -
                        (input.e2 * input.e2 / 16 + input.e2 * input.e2 * input.e2 / 16) * (1 - Math.Pow(sin_A0, 2)) +
                        (3 * Math.Pow(input.e2, 3) / 128) * (1 - Math.Pow(sin_A0, 2)) * (1 - Math.Pow(sin_A0, 2));

                beta = (Math.Pow(input.e2, 2) / 16 + Math.Pow(input.e2, 3) / 16) * (1 - Math.Pow(sin_A0, 2)) -
                       (Math.Pow(input.e2, 3) / 32) * (1 - Math.Pow(sin_A0, 2)) * (1 - Math.Pow(sin_A0, 2));

                gamma = Math.Pow(input.e2, 3) / 256 * (1 - Math.Pow(sin_A0, 2)) * (1 - Math.Pow(sin_A0, 2));

                sigma1 = Math.Atan(Math.Tan(u1) / Math.Cos(A1));

                epsilon = ((alpha * sigema) + (beta * Math.Cos(2 * sigma1 + sigema) * Math.Sin(sigema)) +
                        (gamma * Math.Sin(2 * sigema) * Math.Cos(4 * sigma1 + 2 * sigema))) * sin_A0;
                lamda = L + epsilon;
                if (Math.Abs(epsilon-delta0)<= 1e-10)
                {
                    break;
                }
                delta0 = epsilon;
                
            }
            //计算S
            k2 = input.e12 * (1 - Math.Pow(sin_A0, 2));
            A = (1 - k2 / 4 + 7 * k2 * k2 / 64 - 15 * k2 * k2 * k2 / 256) / input.b;
            B = k2 / 4 - k2 * k2 / 8 + 37 * k2 * k2 * k2 / 512;
            C = k2 * k2 / 128 - k2 * k2 * k2 / 128;
            sigma1 = Math.Atan(Math.Tan(u1) / Math.Cos(A1));
            xs = C * Math.Sin(2 * sigema) * Math.Cos(4 * sigma1 + 2 * sigema);
            S = (sigema - B * Math.Sin(sigema) * Math.Cos(2 * sigma1 + sigema) - xs) / A;
            return S;
        }
    }
}

