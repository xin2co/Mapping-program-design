using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 对流层改正练习720
{
    public class Point
    {
        public string name;
        public string time;
        public double L;
        public double B;
        public double Lhu;
        public double Bhu;
        public double H;
        public double E;
        public double Ehu;
        public double ds;

        public double MwE { get; internal set; }
        public double MdE { get; internal set; }
        public double ZHD { get; internal set; }
        public double ZWD { get; internal set; }
    }
}
