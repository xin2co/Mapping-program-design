using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 对流层改正练习_719
{
    public class Point
    {
        public string name;
        public string time;
        public double B;
        public double L;
        public double H;
        public double E;

        public double Lhu { get; internal set; }
        public double Ehu { get; internal set; }
        public double MwE { get; internal set; }
        public double MdE { get; internal set; }
        public double ZHD { get; internal set; }
        public double ZWD { get; internal set; }
        public double ds { get; internal set; }
    }

}
