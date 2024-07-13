using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitceshi
{
    class PointToCurve
    {
        public static List<MyCurve> builtCurve(List<MyPoint> mypoint_list, bool is_close)
        {
            List<MyCurve> mycurve_list = new List<MyCurve>();
            // 补充点集，以便于计算曲线
            List<MyPoint> mypoint_list_supply = supplyPoints(mypoint_list, is_close);
            // 遍历点集，计算每一段曲线
            for (int i = 0; i < mypoint_list_supply.Count - 5; i++)
            {
                // 计算点i + 2到点i + 3之间的曲线
                double cos0 = 0, cos1 = 0, sin0 = 0, sin1 = 0, r = 0;
                // 计算点i + 2和点i + 3之间的距离
                r = mypoint_list_supply[i + 3].distance(mypoint_list_supply[i + 2]);

                // 计算点i + 1和点i + 2之间的斜率
                calCosSin(mypoint_list_supply[i + 1], mypoint_list_supply[i + 2], mypoint_list_supply[i + 3], mypoint_list_supply[i + 4], mypoint_list_supply[i + 5],
   ref cos1, ref sin1);
                // 计算点i和点i + 1之间的斜率
                calCosSin(mypoint_list_supply[i], mypoint_list_supply[i + 1], mypoint_list_supply[i + 2], mypoint_list_supply[i + 3], mypoint_list_supply[i + 4],
   ref cos0, ref sin0);

                MyCurve mycurve = new MyCurve();

                // 设置曲线的x轴系数
                mycurve.p0 = mypoint_list_supply[i + 2].x;
                mycurve.p1 = r * cos0;
                mycurve.p2 = 3 * (mypoint_list_supply[i + 3].x - mypoint_list_supply[i + 2].x) - r * (cos1 + 2 * cos0);
                mycurve.p3 = -2 * (mypoint_list_supply[i + 3].x - mypoint_list_supply[i + 2].x) + r * (cos1 + cos0);

                // 设置曲线的y轴系数
                mycurve.q0 = mypoint_list_supply[i + 2].y;
                mycurve.q1 = r * sin0;
                mycurve.q2 = 3 * (mypoint_list_supply[i + 3].y - mypoint_list_supply[i + 2].y) - r * (sin1 + 2 * sin0);
                mycurve.q3 = -2 * (mypoint_list_supply[i + 3].y - mypoint_list_supply[i + 2].y) + r * (sin1 + sin0);

                // 设置曲线的起点和终点
                mycurve.mypoint_start = mypoint_list_supply[i + 2];
                mycurve.mypoint_end = mypoint_list_supply[i + 3];

                // 将计算出的曲线添加到曲线列表中
                mycurve_list.Add(mycurve);
            }

            // 返回构建好的曲线列表
            return mycurve_list;
        }

        /// <summary>
        /// 计算p3处的斜率
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        /// <returns></returns>
        public static void calCosSin(MyPoint p1, MyPoint p2, MyPoint p3, MyPoint p4, MyPoint p5,
           ref double cos, ref double sin)
        {
            // 计算点p3处的斜率，即曲线在该点的切线方向
            MyCurve mycurve = new MyCurve();
            double a0, a1, a2, a3, a4, b0, b1, b2, b3, b4;
            double w2, w3;

            // 计算x和y方向的差分
            a1 = p2.x - p1.x;
            a2 = p3.x - p2.x;
            a3 = p4.x - p3.x;
            a4 = p5.x - p4.x;

            b1 = p2.y - p1.y;
            b2 = p3.y - p2.y;
            b3 = p4.y - p3.y;
            b4 = p5.y - p4.y;

            // 计算权重因子
            w2 = Math.Abs(a3 * b4 - a4 * b3);
            w3 = Math.Abs(a1 * b2 - a2 * b1);

            // 计算加权平均斜率
            a0 = w2 * a2 + w3 * a3;
            b0 = w2 * b2 + w3 * b3;

            // 计算斜率的余弦和正弦值
            cos = a0 / (Math.Sqrt(a0 * a0 + b0 * b0));
            sin = b0 / (Math.Sqrt(a0 * a0 + b0 * b0));
        }

        /// <summary>
        /// 补充前后的点
        /// </summary>
        /// <param name="mypoint_list">原始点集</param>
        /// <param name="is_close">是否闭合曲线</param>
        /// <returns>补充后的点集</returns>
        public static List<MyPoint> supplyPoints(List<MyPoint> mypoint_list, bool is_close)
        {
            List<MyPoint> result = new List<MyPoint>();
            foreach (MyPoint point in mypoint_list)
            {
                result.Add(point);
            }
            // 如果是闭合曲线，需要在点集的开始和结束处添加额外的点
            if (is_close)
            {
                int length = result.Count;
                // 在开始处添加两个重复的最后一个点
                result.Insert(0, result[length - 1]);
                result.Insert(0, result[length - 1]);
                // 在结束处添加三个重复的前三个点
                result.Add(result[2]);
                result.Add(result[3]);
                result.Add(result[4]);
            }
            else
            {
                // 如果不是闭合曲线，需要在开始和结束处分别添加两个点
                MyPoint p0 = result[0];
                MyPoint p1 = result[1];
                MyPoint p2 = result[2];
                MyPoint pa = new MyPoint();
                MyPoint pb = new MyPoint();
                // 使用差分来计算新点的位置
                pa.x = p2.x - 3 * p1.x + 3 * p0.x;
                pa.y = p2.y - 3 * p1.y + 3 * p0.y;

                pb.x = p1.x - 3 * p0.x + 3 * pa.x;
                pb.y = p1.y - 3 * p0.y + 3 * pa.y;

                // 在开始处添加两个新点
                result.Insert(0, pa);
                result.Insert(0, pb);

                int length = result.Count;
                p0 = result[length - 1];
                p1 = result[length - 2];
                p2 = result[length - 3];
                MyPoint pc = new MyPoint();
                MyPoint pd = new MyPoint();

                // 使用差分来计算新点的位置
                pc.x = p2.x - 3 * p1.x + 3 * p0.x;
                pc.y = p2.y - 3 * p1.y + 3 * p0.y;

                pd.x = p1.x - 3 * p0.x + 3 * pc.x;
                pd.y = p1.y - 3 * p0.y + 3 * pc.y;

                // 在结束处添加两个新点   
                result.Add(pc);
                result.Add(pd);
            }
            // 返回补充后的点集
            return result;
        }
    }
}
