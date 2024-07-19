using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 大地线长省
{
    class Algo
    {
        private static bool isFirstCall = true;
        public struct Input
        {
            public double a;
            public double fdao;
            public double b;
            public double e2;
            public double epie2;
            public double L1;
            public double B1;
            public double L2;
            public double B2;
        }

        public static class GlobalVariables
        {
            public static Dictionary<string, string> Variables { get; set; }
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
            double temp;
            #endregion
            input.b = input.a * (1 - 1 / input.fdao);
            input.e2 = ((input.a * input.a) - (input.b * input.b)) / (input.a * input.a);
            input.epie2 = (input.a * input.a - input.b * input.b) / (input.b * input.b);
            //辅助计算
            u1 = Math.Atan(Math.Sqrt(1 - input.e2) * Math.Tan(input.B1));
            u2 = Math.Atan(Math.Sqrt(1 - input.e2) * Math.Tan(input.B2));
            L = input.L2 - input.L1;
            a1 = Math.Sin(u1) * Math.Sin(u2);
            a2 = Math.Cos(u1) * Math.Cos(u2);
            b1 = Math.Cos(u1) * Math.Sin(u2);
            b2 = Math.Sin(u1) * Math.Cos(u2);

            //逐次趋近法
            lamda = L;
            temp = 0;
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
                if (Math.Abs(epsilon - temp) <= 1e-10)
                {
                    temp = epsilon;
                    break;
                }
                temp = epsilon;

            }
            //计算S
            k2 = input.epie2 * (1 - Math.Pow(sin_A0, 2));
            A = (1 - k2 / 4 + 7 * k2 * k2 / 64 - 15 * k2 * k2 * k2 / 256) / input.b;
            B = k2 / 4 - k2 * k2 / 8 + 37 * k2 * k2 * k2 / 512;
            C = k2 * k2 / 128 - k2 * k2 * k2 / 128;
            sigma1 = Math.Atan(Math.Tan(u1) / Math.Cos(A1));
            xs = C * Math.Sin(2 * sigema) * Math.Cos(4 * sigma1 + 2 * sigema);
            S = (sigema - B * Math.Sin(sigema) * Math.Cos(2 * sigma1 + sigema) - xs) / A;
            if (isFirstCall)
            {
                GlobalVariables.Variables = new Dictionary<string, string>
                {
                    { "1，椭球长半轴 a", input.a.ToString("F0") },
                    { "2，扁率倒数 1/f",input.fdao.ToString("F3") },
                    { "3，扁率 f",  (1 / input.fdao).ToString("F8") },
                    { "4，椭球短半轴 b", input.b.ToString("F3") },
                    { "5，第一偏心率平方e2", input.e2.ToString("F8") },
                    { "6，第二偏心率平方e'2", input.epie2.ToString("F8") },
                    { "7，第1条大地线u1", u1.ToString("F8") },
                    { "8，第1条大地线u2", u2.ToString("F8")},
                    { "9，第1条大地线经差l(弧度)", L.ToString("F8") },
                    { "10，第1条大地线a1", a1.ToString("F8") },
                    { "11，第1条大地线a2", a2.ToString("F8") },
                    { "12，第1条大地线b1", b1.ToString("F8") },
                    { "13，第1条大地线b2", b2.ToString("F8") },
                    { "14，第1条大地线系数α", alpha.ToString("F8") },
                    { "15，第1条大地线系数β", beta.ToString("F8") },
                    { "16，第1条大地线系数γ", gamma.ToString("F8") },
                    { "17，第1条大地线系数A1（弧度）", A1.ToString("F8") },
                    { "18，第1条大地线系数λ", lamda.ToString("F8") },
                    { "19，第1条大地线系数σ", sigema.ToString("F8") },
                    { "20，第1条大地线系数sinA0", sin_A0.ToString("F8") },
                    { "21，第1条大地线系数系数A", A.ToString("F8") },
                    { "22，第1条大地线系数系数B", B.ToString("F8") },
                    { "23，第1条大地线系数系数C", C.ToString("F8") },
                    { "24，第1条大地线系数σ1", sigma1.ToString("F8") },
                };
                isFirstCall = false;
            }

            return S;
        }
    }
}
