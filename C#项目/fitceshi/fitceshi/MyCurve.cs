using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitceshi
{
    public class MyCurve
    {
        // 曲线在x轴方向上的系数
        public double p0, p1, p2, p3;

        // 曲线在y轴方向上的系数
        public double q0, q1, q2, q3;

        // 曲线的起点
        public MyPoint mypoint_start;

        // 曲线的终点
        public MyPoint mypoint_end;
    }
}
