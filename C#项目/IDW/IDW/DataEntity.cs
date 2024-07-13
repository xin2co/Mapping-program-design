using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDW
{
    class DataEntity
    {
        // 公共成员，用于存储Point对象的列表
        public List<Point> Data;

        // 只读属性，返回Data列表中元素的个数
        public int Count => Data.Count;

        // 构造函数，初始化Data列表
        public DataEntity()
        {
            Data = new List<Point>();
        }

        // 公共方法，用于向Data列表中添加一个Point对象
        public void Add(Point pt)
        {
            Data.Add(pt);
        }

        //索引器，允许通过索引访问和修改Data列表中的Point对象
        public Point this[int i]
        {
            get { return Data[i]; } // 获取指定索引处的Point对象
            set { Data[i] = value; } // 设置指定索引处的Point对象
        }

        // 重写ToString方法，返回Data列表中所有Point对象的字符串表示
        public override string ToString()
        {
            string res = "测站    X（m）    Y（m)      H(m)\n"; // 初始化结果字符串
            foreach (var d in Data)
            {
                res += d.ToString() + "\n"; // 将每个Point对象的字符串表示添加到结果字符串中
            }
            return res; // 返回结果字符串
        }
    }
}
