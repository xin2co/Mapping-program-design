using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace 矩阵计算
{
    public class Matrixs
    {
        double[,] Matrix;
        int Col, Row;
        string name;
        public Matrixs(int Col, int Row)
        {
            this.Col = Col;
            this.Row = Row;
            this.name = "Result";
            this.Matrix = new double[Row, Col];
        }

        public Matrixs(int Col, int Row, string name)
        {
            this.Col = Col;
            this.Row = Row;
            this.name = name;
            this.Matrix = new double[Row, Col];
        }

        public Matrixs() { }

        public int getCol
        {
            get { return this.Col; }
            set { this.Col = value; }
        }

        public int getRow
        {
            get { return this.Row; }
            set { this.Row = value; }
        }

        public double [,] detail
        {
            get { return Matrix; }
            set { Matrix = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}