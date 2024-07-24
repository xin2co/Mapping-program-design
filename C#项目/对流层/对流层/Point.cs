using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 对流层
{
    public class Point
    {
        public string id;
        public string time;
    
        public double L;//经度
        public double Lhu;
        public double B;//纬度
        public double Bhu;
        public double H;
        public double E;
        public double Ehu;

        public double MwE { get; internal set; }
        public double MdE { get; internal set; }
        public double ZHD { get; internal set; }
        public double ZWD { get; internal set; }
        public double ds { get; internal set; }
    }
}
