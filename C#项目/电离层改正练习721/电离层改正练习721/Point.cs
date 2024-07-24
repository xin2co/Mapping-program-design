using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 电离层改正练习721
{
    public class Point
    {
        public string name;
        public double x;
        public double y;
        public double z;

        public double A { get; internal set; }
        public double E { get; internal set; }
        public double phiIP { get; internal set; }
        public double lamudaIP { get; internal set; }
        public double Dion { get; internal set; }
        public double phim { get; internal set; }
        public double Tion { get; internal set; }
    }
}
