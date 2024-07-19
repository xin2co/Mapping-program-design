using System;

namespace fitceshi
{
    public class MyPoint
    {
        // MyPoint类用于创建和操作二维坐标系中的点。
        public string ID;
        // 字段x表示点在二维坐标系中的x坐标。
        public double x;
        // 字段y表示点在二维坐标系中的y坐标。
        public double y;

        // 默认构造函数，创建一个MyPoint类型的实例。
        public MyPoint()
        {

        }

        // 带参数的构造函数，根据提供的ID, x, y值创建一个MyPoint类型的实例。
        public MyPoint(string id, double x, double y)
        {
            // 将传入的id赋值给字段ID。
            this.ID = id;
            // 将传入的x赋值给字段x。
            this.x = x;
            // 将传入的y赋值给字段y。
            this.y = y;
        }

        // distance方法计算并返回当前点到另一个点po的距离。
        public double distance(MyPoint po)
        {
            // 计算两点在x轴和y轴上的差值平方和。
            double result = (po.x - x) * (po.x - x) + (po.y - y) * (po.y - y);
            // 对差值平方和开平方，得到距离。
            result = Math.Sqrt(result);
            // 返回计算出的距离。
            return result;
        }
    }
}
