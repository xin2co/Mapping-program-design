using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 大地线长省
{
    class MyFunctions
    {
        public double ddmmssTorad(double ddmmss)
        {
            double degrees;
            double minutes;
            double seconds;

            degrees = Math.Floor(ddmmss); 
            minutes = Math.Floor((ddmmss - degrees) * 100); 
            seconds = (ddmmss - degrees - minutes / 100) * 10000; 

            double rad = (degrees + minutes / 60.0 + seconds / 3600.0) * (Math.PI / 180.0);

            return rad;
        }

    }
}
