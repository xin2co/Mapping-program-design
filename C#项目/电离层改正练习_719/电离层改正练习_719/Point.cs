using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 电离层改正练习_719
{
    public class Point
    {
        public String name;
        public double x;
        public double y;
        public double z;
        public double faim;
        public double psiip;

        public double A { get; internal set; }
        public double E { get; internal set; }
        public double Dion { get; internal set; }
        public double lamudaip { get; internal set; }
        public double Tion { get; internal set; }
    }
}
