using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 空间数据分析7._22
{
    class Matrixs
    {
        public List<List<double>> Matrix = new List<List<double>>();
        public int Col, Row;

        public void Init(int Col, int Row)
        {
            this.Col = Col;
            this.Row = Row;

            for(int i = 0; i<this.Row; i++)
            {
                List<double> cur = new List<double>();
                for (int j = 0; j < Col; j++)
                    cur.Add(0);
                this.Matrix.Add(cur);
            }
        }
    }
}
