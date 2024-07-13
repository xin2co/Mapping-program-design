﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 大地线长计算
{
    class Myfunctions
    {
        public double ddmmssTorad(double dd)
        {
            double rad;
            double deg;
            double min;
            double sec;
            deg = (int)(dd);
            min = (int)((dd - deg) * 100);
            sec = dd * 10000 - deg * 10000 - min * 100;
            rad = (deg + min / 60.0 + sec / 3600.0) / 180.0 * Math.PI;
            return rad;
        }

        public string radToddmmss(double rad)
        {
            double deg;
            double min;
            double sec;
            string dd;
            rad = rad / Math.PI * 180;
            deg = (int)(rad);
            min = (int)((rad - deg) * 60);
            sec = (int)((((rad - deg) * 60) - min) * 60);
            dd = deg.ToString("f0") + "." + min.ToString("f0") + sec.ToString("f0");
            return dd; 
        }
    }
}
