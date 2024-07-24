using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 矩阵计算
{
    class Matrix_Operator
    {
        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrixs MatrixsAdd(Matrixs a, Matrixs b)
        {
            if(a.getCol != b.getCol || a.getRow != b.getRow)
            {
                Exception exception = new Exception("维数不一致！");
                throw exception;
            }

            Matrixs res = new Matrixs(a.getCol, a.getRow);

            var f = a.detail;
            var s = b.detail;
            var c = res.detail;

            for (int i = 0; i < a.getRow; i++)
                for (int j = 0; j < a.getCol; j++)
                    c[i, j] = f[i, j] + s[i, j];

            return res;
        }

        /// <summary>
        /// a - b
        /// </summary>
        /// <param name="a">减数</param>
        /// <param name="b">被减数</param>
        /// <returns>a - b 的结果（Matrix）</returns>
        public static Matrixs MatrixsSub(Matrixs a, Matrixs b)
        {
            if (a.getCol != b.getCol || a.getRow != b.getRow)
            {
                Exception exception = new Exception("维数不一致！");
                throw exception;
            }

            Matrixs res = new Matrixs(a.getCol, a.getRow);

            var f = a.detail;
            var s = b.detail;
            var c = res.detail;

            for (int i = 0; i < a.getRow; i++)
                for (int j = 0; j < a.getCol; j++)
                    c[i, j] = f[i, j] - s[i, j];

            return res;
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="A">左乘矩阵</param>
        /// <param name="B">右乘矩阵</param>
        /// <returns>两矩阵相乘的Result矩阵</returns>
        public static Matrixs MatrixMulti(Matrixs A, Matrixs B)
        {
            if(A.getCol != B.getRow)
            {
                Exception exception = new Exception("矩阵无法进行乘法计算！");
                throw exception;
            }

            Matrixs res = new Matrixs(A.getRow, B.getCol);
            var a = A.detail;
            var b = B.detail;
            var c = res.detail;

            double Col = res.getRow;
            double Row = res.getCol;
            double n = A.getRow;

            for(int i = 0; i<Row; i++)
            {
                for(int j = 0; j<Col; j++)
                {
                    c[i, j] = 0;
                    for (int k = 0; k < n; k++)
                        c[i, j] += a[i, k] * b[j, k];
                }
            }

            return res;
        }

        /// <summary>
        /// 矩阵转置
        /// </summary>
        /// <param name="A">目标矩阵</param>
        /// <returns>目标矩阵的转置矩阵</returns>
        public static Matrixs MatrixsTransPose(Matrixs A)
        {
            Matrixs res = new Matrixs(A.getCol, A.getRow);

            var a = A.detail;
            var b = res.detail;

            for (int i = 0; i < res.getRow; i++)
                for (int j = 0; j < res.getCol; j++)
                    b[i, j] = a[j, i];

            return res;
        }

        /// <summary>
        /// 矩阵求逆
        /// </summary>
        /// <param name="A">目标矩阵</param>
        /// <returns>返回目标矩阵的逆矩阵</returns>
        public static Matrixs MatrixsInverse(Matrixs A)
        {
            if(A.getCol != A.getRow)
            {
                Exception exception = new Exception("矩阵不是方阵！");
                MessageBox.Show("发生错误：" + exception.Message);
            }

            Matrixs res = new Matrixs(A.getRow, A.getCol);

            //深复制
            var a = A.detail;
            var b = res.detail;

            int Col = A.getCol;
            int Row = A.getRow;

            for (int i = 0; i < A.getRow; i++)
                for (int j = 0; j < A.getCol; j++)
                    b[i, j] = (i == j ? 1 : 0);

            for(int i = 0; i<Row; i++)
            {
                //非零化对角线
                double pivot = a[i, i];
                if(pivot == 0)
                {
                    for(int j = i + 1; j<Row; j++)
                        if(a[j, i] != 0)
                        {
                            RowChange(ref A, i, j);
                            RowChange(ref res, i, j);
                            pivot = a[i, i];
                            break;
                        }
                }

                if(pivot == 0)
                {
                    Exception exception = new Exception("该矩阵为奇异矩阵，无法求逆矩阵！");
                    MessageBox.Show("发生错误：" + exception.Message);
                    return res;
                }

                for(int j = 0; j<Row; j++)
                {
                    a[i, j] /= pivot;
                    b[i, j] /= pivot;
                }

                //每一行
                for(int j = 0; j<Row; j++)
                {
                    if(i != j)
                    {
                        double factror = a[j, i];
                        for(int k = 0; k<Col; k++)
                        {
                            a[j, k] -= factror * a[i, k];
                            b[j, k] -= factror * b[i, k];
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 初等行变换
        /// </summary>
        /// <param name="tar">所要进行初等行变化的目标矩阵</param>
        /// <param name="a">初等行变换第一行</param>
        /// <param name="b">初等行变换第二行</param>
        public static void RowChange(ref Matrixs tar, int a, int b)
        {
            var f = tar.detail;

            int Col = tar.getCol;
            int Row = tar.getRow;

            for(int i = 0; i<Col; i++)
            {
                var cur = f[a, i];
                f[a, i] = f[b, i];
                f[b, i] = cur;
            }
        }


        /// <summary>
        /// 矩阵打印函数
        /// </summary>
        /// <param name="a">目标矩阵</param>
        /// <param name="tar">打印的目标空间，默认为richtextbox控件</param>
        public static void Print_Matrix(Matrixs a, RichTextBox tar)
        {
            int Col = a.getCol;
            int Row = a.getRow;
            var r = a.detail;

            tar.AppendText("---Matrix---\n");
            for(int i = 0; i<Row; i++)
            {
                for (int j = 0; j < Col; j++)
                    tar.AppendText($"{r[i, j].ToString("0.000"),-10}");
                tar.AppendText("\n");
            }
        }
    }
}