using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDW
{
    class Algo
    {
        // DataEntity类型的成员变量，用于存储数据点
        DataEntity Data;

        // N定义了参与计算的最近邻点的数量
        private int N = 5;

        // 构造函数，初始化DataEntity数据以及N的值
        public Algo(DataEntity data, int n)
        {
            Data = data;
            N = n;
        }

        // 计算两点之间的距离
        public double Distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X; // x坐标差
            double dy = p1.Y - p2.Y; // y坐标差
            double ds = Math.Sqrt(dx * dx + dy * dy); // 计算欧氏距离
            return ds;
        }

        // IDW算法的主要实现，返回一个字符串，包含插值点的信息和计算的值
        public string Idw(Point pt)
        {
            string res = $"{pt.Id}  {pt.X:f3}  {pt.Y:f3}  "; // 初始化结果字符串
            for (int i = 0; i < Data.Count; i++)
            {
                double d = Distance(Data[i], pt); // 计算插值点与数据集中每个点的距离
                Data[i].Dist = d; // 将距离存储在数据点的Dist属性中
            }
            var dt = Sort(); // 对数据点根据距离进行排序
            double H = GetH(dt); // 根据排序后的数据点计算插值
            res += $" {H:f3}   "; // 将计算结果添加到结果字符串中
            for (int j = 0; j < N; j++)
            {
                res += $"{dt[j].Id} "; // 将参与计算的最近邻点的ID添加到结果字符串中
            }
            return res; // 返回结果字符串
        }

        // 根据排序后的数据点计算插值
        private double GetH(DataEntity dt)
        {
            double over = 0, under = 0;
            for (int i = 0; i < N; i++)
            {
                over += dt[i].H / dt[i].Dist; // 分子部分，加权高度除以距离
                under += 1 / dt[i].Dist; // 分母部分，距离的倒数之和
            }
            return over / under; // 计算最终的插值结果
        }

        // 对数据点根据距离进行排序
        DataEntity Sort()
        {
            DataEntity dt = Data;
            for (int i = 0; i < Data.Count; i++)
            {
                for (int j = i; j < Data.Count; j++)
                {
                    if (dt[i].Dist > dt[j].Dist) // 如果前一个点的距离大于后一个点的距离
                    {
                        var pt = dt[i]; // 交换两个点
                        dt[i] = dt[j];
                        dt[j] = pt;
                    }
                }
            }
            return dt; // 返回排序后的数据点集
        }
    }
}