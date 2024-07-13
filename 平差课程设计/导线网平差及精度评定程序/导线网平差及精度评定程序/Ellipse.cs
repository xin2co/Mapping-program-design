using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 导线网平差及精度评定程序
{
    public class Ellipse
    {
        public double E; // 长轴长度
        public double F; // 短轴长度
        public Angle orientation; // 长轴与X轴的夹角
        public Point center; // 椭圆中心点

        public Ellipse(double E, double F, Angle orientation, Point center)
        {
            this.E = E;
            this.F = F;
            this.orientation = orientation;
            this.center = center;
        }

    }
}
