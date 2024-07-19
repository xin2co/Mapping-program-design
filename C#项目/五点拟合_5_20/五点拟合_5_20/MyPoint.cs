using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 五点拟合_5_20
{
    public class MyPoint
    {
        public string ID;
        public double x;
        public double y;

        public MyPoint()
        {
        }

        public MyPoint(string id, double x,double y)
        {
            this.ID = id;
            this.x = x;
            this.y = y;
        }

        internal double distance(MyPoint po)
        {
            double result = (po.x - x) * (po.x - x) + (po.y - y) * (po.y - y);
            result = Math.Sqrt(result);
            return result;
        }
    }
}
