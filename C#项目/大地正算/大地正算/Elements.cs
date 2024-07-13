using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 大地正算
{
    public class Elements
    {
        public string Qidian; 
        public double B1;
        public double L1;
        public string Zhongdian;
        public double A1;
        public double S;

        public Elements(string qidian, double B1, double L1, string zhongdian, double angle, double S)
        {
            Qidian = qidian;
            this.B1 = B1;
            this.L1 = L1;
            Zhongdian = zhongdian;
            A1 = angle;
            this.S = S;
        }

        public Elements()
        {

        }


    }
}
