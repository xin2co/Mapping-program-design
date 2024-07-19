using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdmWrite
{
    class Algo
    {
        DataEntity Data;

        private int N = 5;
        public Algo (DataEntity data,int n)
        {
            Data = data;
            N = n;
        }

        public double Distance(Point p1,Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double ds = Math.Sqrt(dx * dx + dy * dy);
            return ds;
        }

        public string Idw(Point pt)
        {
            string res = $"{pt.Id}  {pt.X:f3}  {pt.Y:f3}  ";
            for(int i = 0; i < Data.Count; i++)
            {
                double d = Distance(Data[i], pt);

                Data[i].Dist = d;
            }
            var dt = Sort();
            double H = GetH(dt);
            res += $"{H:f3}   ";
            for(int j = 0; j < N; j++)
            {
                res += $"{dt[j].Id}";
            }
            return res;
        }

        private double GetH(DataEntity dt)
        {
            double over = 0, under = 0;
            for(int i = 0; i < N; i++)
            {
                over += dt[i].H / dt[i].Dist; // 分子部分，加权高度除以距离
                under += 1 / dt[i].Dist; // 分母部分，距离的倒数之和
            }

            return over / under;
        }

        DataEntity Sort()
        {
            DataEntity dt = Data;
            for(int i = 0; i < Data.Count; i++)
            {
                for(int j = i; j < Data.Count; j++)
                {
                    if (dt[i].Dist > dt[j].Dist) // 如果前一个点的距离大于后一个点的距离
                    {
                        var pt = dt[i]; // 交换两个点
                        dt[i] = dt[j];
                        dt[j] = pt;
                    }
                }
            }
            return dt;
        }
    }
}
