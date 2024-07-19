using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 五点拟合_5_20
{
    class PointToCurve
    {
        //补充点
        public static List<MyPoint> supplyPoints(List<MyPoint> mypoint_list, bool is_close)
        {
            List<MyPoint> result = new List<MyPoint>();
            foreach (MyPoint po in mypoint_list)
            {
                result.Add(po);
            }

            if (is_close)
            {
                int length = result.Count;
                result.Insert(0, result[length - 1]);
                result.Insert(0, result[length - 1]);
                result.Add(result[2]);
                result.Add(result[3]);
                result.Add(result[4]);
            }
            else
            {
                MyPoint p0 = result[0];
                MyPoint p1 = result[1];
                MyPoint p2 = result[2];
                MyPoint pa = new MyPoint();
                MyPoint pb = new MyPoint();

                pa.x = p2.x - p1.x - (p1.x - p0.x);
                pa.y = p2.y - p1.y - (p1.y - p0.y);

                pb.x = p1.x - p0.x - (p0.x - pa.x);
                pb.y = p1.y - p0.y - (p0.y - pa.y);
                result.Insert(0, pa);
                result.Insert(0, pb);

                int length = result.Count;
                p0 = result[length - 1];
                p1 = result[length - 2];
                p2 = result[length - 3];
                MyPoint pc = new MyPoint();
                MyPoint pd = new MyPoint();

                pc.x = p2.x - p1.x - (p1.x - p0.x);
                pc.y = p2.y - p1.y - (p1.y - p0.y);

                pd.x = p1.x - p0.x - (p0.x - pa.x);
                pd.y = p1.y - p0.y - (p0.y - pa.y);

                result.Add(pc);
                result.Add(pd);
            }

            return result;
        }

        //计算斜率
        public static void calCosSin(MyPoint p1, MyPoint p2, MyPoint p3, MyPoint p4, MyPoint p5,
          out double cos, out double sin)
        {
            MyCurve mycurve = new MyCurve();
            double a0, a1, a2, a3, a4, b0, b1, b2, b3, b4;
            double w2, w3;
            a1 = p2.x - p1.x;
            a2 = p3.x - p2.x;
            a3 = p4.x - p3.x;
            a4 = p5.x - p4.x;

            b1 = p2.y - p1.y;
            b2 = p3.y - p2.y;
            b3 = p4.y - p3.y;
            b4 = p5.y - p4.y;

            w2 = Math.Abs(a3 * b4 - a4 * b3);
            w3 = Math.Abs(a1 * b2 - a2 * b1);

            a0 = w2 * a2 + w3 * a3;
            b0 = w2 * b2 + w3 * b3;

            cos = a0 / (Math.Sqrt(a0 * a0 + b0 * b0));
            sin = b0 / (Math.Sqrt(a0 * a0 + b0 * b0));
        }

        //求解曲线参数
        public static List<MyCurve> builtCurve(List<MyPoint> mypoint_list, bool is_close)
        {
            List<MyCurve> mycurve_list = new List<MyCurve>();
            List<MyPoint> mypoint_list_supply = supplyPoints(mypoint_list, is_close);
            for (int i = 0; i < mypoint_list_supply.Count - 5; i++)
            {
                double r = mypoint_list_supply[i + 3].distance(mypoint_list_supply[i + 2]);
                double cos0, sin0, cos1, sin1;

                calCosSin(mypoint_list_supply[i], mypoint_list_supply[i + 1], mypoint_list_supply[i + 2],
          mypoint_list_supply[i + 3], mypoint_list_supply[i + 4], out cos0, out sin0);

                calCosSin(mypoint_list_supply[i + 1], mypoint_list_supply[i + 2], mypoint_list_supply[i + 3], mypoint_list_supply[i + 4], mypoint_list_supply[i + 5],
   out cos1, out sin1);

                MyCurve mycurve = new MyCurve
                {
                    E0 = mypoint_list_supply[i + 2].x,
                    E1 = r * cos0,
                    E2 = 3 * (mypoint_list_supply[i + 3].x - mypoint_list_supply[i + 2].x) - r * (cos1 + 2 * cos0),
                    E3 = -2 * (mypoint_list_supply[i + 3].x - mypoint_list_supply[i + 2].x) + r * (cos1 + cos0),

                    F0 = mypoint_list_supply[i + 2].y,
                    F1 = r * sin0,
                    F2 = 3 * (mypoint_list_supply[i + 3].y - mypoint_list_supply[i + 2].y) - r * (sin1 + 2 * sin0),
                    F3 = -2 * (mypoint_list_supply[i + 3].y - mypoint_list_supply[i + 2].y) + r * (sin1 + sin0),

                    mypoint_start = mypoint_list_supply[i + 2],
                    mypoint_end = mypoint_list_supply[i + 3]
                };
                mycurve_list.Add(mycurve);
            }
            return mycurve_list;
        }
    } 
}
