using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 电离层改正
{
    public class Point
    {
        public string name;
        public double x;
        public double y;
        public double z;
        public double A;
        public double E;

        public double Phi_IP { get; internal set; }
        public double Lambda_IP { get; internal set; }
        public double Phi_M { get; internal set; }
        public double T_ion { get; internal set; }
        public double D_ion { get; internal set; }
    }
}
