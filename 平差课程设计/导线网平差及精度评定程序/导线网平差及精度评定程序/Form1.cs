using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;

namespace 导线网平差及精度评定程序
{
    public partial class Form1 : Form
    {
        List<Point> KnownPoints = new List<Point>();
        List<Angle> Angles = new List<Angle>();
        List<Edge> Edges = new List<Edge>();
        List<Point> UnknownPoints = new List<Point>(new Point[4]);
        List<AngleEquation> equations = new List<AngleEquation>();
        List<double> ls = new List<double>();
        Algo Algo = new Algo();
        List<Point> updatedPoints = new List<Point>();
        double Rho = 206265;
        Matrix<double> Qxxquan;
        double seigemaquan;
        public Form1()
        {
            InitializeComponent();
        }


        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件|*.txt";
            openFileDialog.Title = "选择文件";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(openFileDialog.FileName);
                    List<string[]> dataSet1 = new List<string[]>();
                    List<string[]> dataSet2 = new List<string[]>();
                    List<string[]> dataSet3 = new List<string[]>();
                    int dataSetIndex = 0;

                    foreach (string line in lines)
                    {
                        // 跳过空行
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            dataSetIndex++;
                            continue;
                        }

                        // 使用逗号分隔每一行
                        string[] values = line.Split(',');

                        // 检查是否有足够的值
                        if (values.Length == 3)
                        {
                            // 根据dataSetIndex将数据添加到相应的数据集中
                            switch (dataSetIndex)
                            {
                                case 0:
                                    dataSet1.Add(values);
                                    break;
                                case 1:
                                    dataSet2.Add(values);
                                    break;
                                case 2:
                                    dataSet3.Add(values);
                                    break;
                            }
                        }
                    }

                    // 假设每个数据集的大小相同
                    int rowCount = Math.Max(dataSet1.Count, Math.Max(dataSet2.Count, dataSet3.Count));

                    for (int i = 0; i < rowCount; i++)
                    {
                        // 从每个数据集中取出对应行的数据，如果数据集不够大，则使用空数组
                        string[] row1 = i < dataSet1.Count ? dataSet1[i] : new string[3];
                        string[] row2 = i < dataSet2.Count ? dataSet2[i] : new string[3];
                        string[] row3 = i < dataSet3.Count ? dataSet3[i] : new string[3];

                        // 将数据添加到dataGridView1中
                        dataGridView1.Rows.Add(row1[0], row1[1], row1[2], row2[0], row2[1], row2[2], row3[0], row3[1], row3[2]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取文件时出错: " + ex.Message);
                }
            }
        }

        private void 初始化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KnownPoints.Clear();
            Edges.Clear();
            Angles.Clear();
            // 添加已知点
            KnownPoints.Add(new Point(1, 121088.500, 259894.000));
            KnownPoints.Add(new Point(2, 127990.100, 255874.600));

            // 角度列表
            string[] angleStrings = {
                "72.10284",
                "66.27289",
                "88.58295",
                "123.11341",
                "85.13374",
                "132.23352",
                "79.09487",
                "72.24564"
    };

            // 处理角度
            for (int i = 0; i < angleStrings.Length; i++)
            {
                Angle angle = Angle.Parse(angleStrings[i], i + 1);
                Angles.Add(angle);
            }

            String sbhjs = null;
            foreach (var point in KnownPoints)
            {
                sbhjs += ($"点号: {point.PointNumber}, 坐标: ({point.x}, {point.y})");
            }
            toolStripStatusLabel1.Text = sbhjs;

            Edges.Add(new Edge(4, 6, 4451.417));
            Edges.Add(new Edge(2, 4, 5564.592));
            Edges.Add(new Edge(6, 5, 5569.269));
            StringBuilder sb = new StringBuilder();

            // 输出已知点信息
            sb.AppendLine("已知点信息:");
            foreach (var point in KnownPoints)
            {
                sb.AppendLine($"点号: {point.PointNumber}, 坐标: ({point.x}, {point.y})");
            }
            sb.AppendLine();

            // 输出角度信息
            sb.AppendLine("角度信息:");
            foreach (var angle in Angles)
            {
                double roundedRadians = Math.Round(angle.Radians, 3);
                sb.AppendLine($"角度序号: {angle.AngleNumber}, 弧度: {roundedRadians}");
            }
            sb.AppendLine();

            // 输出边信息
            sb.AppendLine("边信息:");
            foreach (var edge in Edges)
            {
                sb.AppendLine(edge.Start + "-" + edge.End + ": " + edge.Length + "m");
            }

            // 更新富文本框
            richTextBox1.Clear();
            richTextBox1.AppendText(sb.ToString());
            sb.Clear();
            tabControl1.SelectedTab = tabPage2;
        }

        private void 平差计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            equations.Clear();

            richTextBox1.Clear();

            //计算方位角
            #region 计算方位角
            List<Angle> CalculatedAzimuths = new List<Angle>();
            if (KnownPoints.Count == 0)
            {
                MessageBox.Show("数据缺失请赋初值!");
                return;
            }
            Angle chushifangweijiao = Algo.coordinateBackwardCalculation(KnownPoints[0], KnownPoints[1]);
            CalculatedAzimuths.Add(chushifangweijiao);

            richTextBox1.AppendText("\n");
            richTextBox1.AppendText("*********************************\n");
            richTextBox1.AppendText("\n");

            richTextBox1.AppendText("1-2方位角为:" + Math.Round(chushifangweijiao.Radians, 3) + "\n");
            Angle fangweijiao24 = Algo.CalculateAzimuth(chushifangweijiao, Angles[1]);
            CalculatedAzimuths.Add(fangweijiao24);
            richTextBox1.AppendText("2-4方位角为:" + Math.Round(fangweijiao24.Radians, 3) + "\n");
            Angle fangweijiao43 = Algo.CalculateAzimuth(fangweijiao24, Angles[5]);
            CalculatedAzimuths.Add(fangweijiao43);
            richTextBox1.AppendText("4-3方位角为:" + Math.Round(fangweijiao43.Radians, 3) + "\n");
            Angle fangweijiao31 = Algo.CalculateAzimuth(fangweijiao43, Angles[2]);
            CalculatedAzimuths.Add(fangweijiao31);
            richTextBox1.AppendText("3-1方位角为:" + Math.Round(fangweijiao31.Radians, 3) + "\n");
            Angle fangweijiao34 = new Angle();
            fangweijiao34.Radians=fangweijiao43.Radians + Math.PI;
            Angle fangweijiao46 = Algo.CalculateAzimuth(fangweijiao34, Angles[4]);
            CalculatedAzimuths.Add(fangweijiao46);
            richTextBox1.AppendText("4-6方位角为:" + Math.Round(fangweijiao46.Radians, 3) + "\n");
            Angle fangweijiao65 = Algo.CalculateAzimuth(fangweijiao46, Angles[7]);
            CalculatedAzimuths.Add(fangweijiao65);
            richTextBox1.AppendText("6-5方位角为:" + Math.Round(fangweijiao65.Radians, 3) + "\n");
            Angle fangweijiao53 = Algo.CalculateAzimuth(fangweijiao65, Angles[6]);
            CalculatedAzimuths.Add(fangweijiao53);
            richTextBox1.AppendText("5-3方位角为:" + Math.Round(fangweijiao53.Radians, 3) + "\n");

            richTextBox1.AppendText("\n");
            richTextBox1.AppendText("*********************************\n");
            richTextBox1.AppendText("\n");
            #endregion

            //计算未知边长
            #region 计算未知边长
            Edge edge12 = new Edge(KnownPoints[0], KnownPoints[1]);
            richTextBox1.AppendText("1-2长度为:" + Math.Round(edge12.Length, 3) + "(m)\n");
            Edge edge14 = Edge.CosineLaw(edge12.Length, Edges[1].Length, Angles[1].Radians);
            richTextBox1.AppendText("1-4长度为:" + Math.Round(edge14.Length, 3) + "(m)\n");
            Edge edge45 = Edge.CosineLaw(Edges[0].Length, Edges[2].Length, Angles[7].Radians);
            richTextBox1.AppendText("4-5长度为:" + Math.Round(edge45.Length, 3) + "(m)\n");
            Angle angle6you = new Angle();
            angle6you.Radians = Math.Asin(edge12.Length * Math.Sin(Angles[1].Radians) / edge14.Length);
            Angle angle6zuo = new Angle();
            angle6zuo.Radians = Angles[5].Radians - angle6you.Radians;
            Edge edge13 = Edge.SineLaw(edge14.Length, Angles[2].Radians, angle6zuo.Radians);
            richTextBox1.AppendText("1-3长度为:" + Math.Round(edge13.Length, 3) + "(m)\n");
            Angle angle1you = new Angle();
            angle1you.Radians = Math.Asin(Edges[1].Length * Math.Sin(Angles[1].Radians) / edge14.Length);
            Angle angle1zuo = new Angle();
            angle1zuo.Radians = Angles[0].Radians - angle1you.Radians;
            Edge edge34 = Edge.SineLaw(edge14.Length, Angles[2].Radians, angle1zuo.Radians);
            richTextBox1.AppendText("3-4长度为:" + Math.Round(edge34.Length, 3) + "(m)\n");
            Angle angle5you = new Angle();
            angle5you.Radians = Math.Asin(Edges[2].Length * Math.Sin(Angles[7].Radians) / edge45.Length);
            Angle angle5zuo = new Angle();
            angle5zuo.Radians = Angles[4].Radians - angle5you.Radians;
            Edge edge35 = Edge.SineLaw(edge45.Length, Angles[3].Radians, angle5zuo.Radians);
            richTextBox1.AppendText("3-5长度为:" + Math.Round(edge35.Length, 3) + "(m)\n");
            #endregion

            //计算坐标初值
            #region 计算未知点坐标
            UnknownPoints[1] = Algo.CoordinateForwardCalculation(KnownPoints[1], Edges[1].Length, fangweijiao24.Radians);
            UnknownPoints[0] = Algo.CoordinateForwardCalculation(UnknownPoints[1], edge34.Length, fangweijiao43.Radians);
            UnknownPoints[3] = Algo.CoordinateForwardCalculation(UnknownPoints[1], Edges[0].Length, fangweijiao46.Radians);
            UnknownPoints[2] = Algo.CoordinateForwardCalculation(UnknownPoints[3], Edges[2].Length, fangweijiao65.Radians);

            richTextBox1.AppendText("\n");
            richTextBox1.AppendText("*********************************\n");
            richTextBox1.AppendText("\n");

            richTextBox1.AppendText("点3坐标初始值:" + '(' + Math.Round(UnknownPoints[0].x, 3) + ',' + Math.Round(UnknownPoints[0].y, 3) + ')' + '\n');
            richTextBox1.AppendText("点4坐标初始值:" + '(' + Math.Round(UnknownPoints[1].x, 3) + ',' + Math.Round(UnknownPoints[1].y, 3) + ')' + '\n');
            richTextBox1.AppendText("点5坐标初始值:" + '(' + Math.Round(UnknownPoints[2].x, 3) + ',' + Math.Round(UnknownPoints[2].y, 3) + ')' + '\n');
            richTextBox1.AppendText("点6坐标初始值:" + '(' + Math.Round(UnknownPoints[3].x, 3) + ',' + Math.Round(UnknownPoints[3].y, 3) + ')' + '\n');
            #endregion

            //计算方程
            #region 计算方程
            //v1的系数
            double v1b1 = Rho * (UnknownPoints[0].y - KnownPoints[0].y) / (edge13.Length * edge13.Length); 
            double v1b2 = -Rho * (UnknownPoints[0].x - KnownPoints[0].x) / (edge13.Length * edge13.Length); 

            AngleEquation angleEquation1= new AngleEquation(v1b1, v1b2, 0, 0, 0, 0,0,0);
            
            //v2系数
            double v2b3 = -Rho * (KnownPoints[1].y - UnknownPoints[1].y) / (Edges[1].Length * Edges[1].Length);
            double v2b4 = Rho * (KnownPoints[1].x - UnknownPoints[1].x) / (Edges[1].Length * Edges[1].Length);
            AngleEquation angleEquation2 = new AngleEquation(0, 0, v2b3, v2b4, 0, 0,0,0);

            //v3系数
            double v3b1 = (Rho * (UnknownPoints[0].y - KnownPoints[0].y) / (edge13.Length * edge13.Length)) + (Rho * (UnknownPoints[1].y - KnownPoints[0].y) / (edge34.Length * edge34.Length));
            double v3b2 = -(Rho * (UnknownPoints[1].x - KnownPoints[0].x) / (edge34.Length * edge34.Length)) + (Rho * (UnknownPoints[0].x - KnownPoints[0].x) / (edge13.Length * edge13.Length));
            double v3b3 = -Rho * (UnknownPoints[1].y - UnknownPoints[0].y) / (edge34.Length * edge34.Length);
            double v3b4 = Rho * (UnknownPoints[1].x - UnknownPoints[0].x) / (edge34.Length * edge34.Length);
            AngleEquation angleEquation3 = new AngleEquation(v3b1, v3b2, v3b3, v3b4, 0, 0, 0, 0);

            //v4系数
            double v4b1 = (Rho * (UnknownPoints[2].y - UnknownPoints[0].y) / (edge35.Length * edge35.Length)) - (Rho * (UnknownPoints[1].y - UnknownPoints[0].y) / (edge34.Length * edge34.Length));
            double v4b2 = (Rho * (UnknownPoints[1].x - UnknownPoints[0].x) / (edge34.Length * edge34.Length)) - (Rho * (UnknownPoints[2].x - UnknownPoints[0].x) / (edge35.Length * edge35.Length));
            double v4b3 = Rho * (UnknownPoints[1].y - UnknownPoints[0].y) / (edge34.Length * edge34.Length);
            double v4b4 = -Rho * (UnknownPoints[1].x - UnknownPoints[0].x) / (edge34.Length * edge34.Length);
            double v4b5 = -Rho * (UnknownPoints[2].y - UnknownPoints[0].y) / (edge35.Length * edge35.Length);
            double v4b6 = Rho * (UnknownPoints[2].x - UnknownPoints[0].x) / (edge35.Length * edge35.Length);
            AngleEquation angleEquation4 = new AngleEquation(v4b1, v4b2, v4b3, v4b4, v4b5, v4b6, 0, 0);

            //v5系数
            double v5b1 = -Rho * (UnknownPoints[1].y - UnknownPoints[0].y) / (edge34.Length * edge34.Length);
            double v5b2 = Rho * (UnknownPoints[1].x - UnknownPoints[0].x) / (edge34.Length * edge34.Length);
            double v5b3 = (Rho * (UnknownPoints[1].y - UnknownPoints[3].y) / (Edges[0].Length * Edges[0].Length)) - (Rho * (UnknownPoints[1].y - UnknownPoints[0].y) / (edge34.Length * edge34.Length));
            double v5b4 = (Rho * (UnknownPoints[1].x - UnknownPoints[0].x) / (edge34.Length * edge34.Length)) - (Rho * (UnknownPoints[1].x - UnknownPoints[3].x) / (Edges[0].Length * Edges[0].Length));
            double v5b7 = -Rho * (UnknownPoints[1].y - UnknownPoints[3].y) / (Edges[0].Length * Edges[0].Length);
            double v5b8 = Rho * (UnknownPoints[1].x - UnknownPoints[3].x) / (Edges[0].Length * Edges[0].Length);
            AngleEquation angleEquation5 = new AngleEquation(v5b1, v5b2, v5b3, v5b4, 0, 0, v5b7, v5b8);

            //v6系数
            double v6b1 = -Rho * (UnknownPoints[1].y - UnknownPoints[0].y) / (edge34.Length * edge34.Length);
            double v6b2 = Rho * (UnknownPoints[1].x - UnknownPoints[0].x) / (edge34.Length * edge34.Length);
            double v6b3 = (Rho * (UnknownPoints[1].y - UnknownPoints[0].y) / (edge34.Length * edge34.Length)) - (Rho * (KnownPoints[1].y - UnknownPoints[1].y) / (Edges[1].Length * Edges[1].Length));
            double v6b4 = -(Rho * (KnownPoints[1].x - UnknownPoints[1].x) / (Edges[1].Length * Edges[1].Length)) - (Rho * (UnknownPoints[1].x - UnknownPoints[0].x) / (edge34.Length * edge34.Length));
            AngleEquation angleEquation6 = new AngleEquation(v6b1, v6b2, v6b3, v6b4, 0, 0, 0, 0);

            //v7参数
            double v7b1 = -Rho * (UnknownPoints[2].y - UnknownPoints[0].y) / (edge35.Length * edge35.Length);
            double v7b2 = Rho * (UnknownPoints[2].x - UnknownPoints[0].x) / (edge35.Length * edge35.Length);
            double v7b5 = (Rho * (UnknownPoints[2].y - UnknownPoints[0].y) / (edge35.Length * edge35.Length)) + (Rho * (UnknownPoints[3].y - UnknownPoints[2].y) / (Edges[2].Length * Edges[2].Length));
            double v7b6 = -(Rho * (UnknownPoints[2].x - UnknownPoints[0].x) / (edge35.Length * edge35.Length)) + (Rho * (UnknownPoints[3].x - UnknownPoints[2].x) / (Edges[2].Length * Edges[2].Length));
            double v7b7 = -Rho * (UnknownPoints[3].y - UnknownPoints[2].y) / (Edges[2].Length * Edges[2].Length);
            double v7b8 = Rho * (UnknownPoints[3].x - UnknownPoints[2].x) / (Edges[2].Length * Edges[2].Length);
            AngleEquation angleEquation7 = new AngleEquation(v7b1, v7b2, 0, 0, v7b5, v7b6, v7b7, v7b8);

            //v8系数
            double v8b3 = Rho * (UnknownPoints[2].y - UnknownPoints[3].y) / (Edges[0].Length * Edges[0].Length);
            double v8b4 = Rho * (UnknownPoints[2].x - UnknownPoints[3].x) / (Edges[0].Length * Edges[0].Length);
            double v8b5 = -Rho * (UnknownPoints[3].y - UnknownPoints[2].y) / (Edges[2].Length * Edges[2].Length);
            double v8b6 = Rho * (UnknownPoints[3].x - UnknownPoints[2].x) / (Edges[2].Length * Edges[2].Length);
            double v8b7 = (Rho * (UnknownPoints[3].y - UnknownPoints[2].y) / (Edges[2].Length * Edges[2].Length)) + (Rho * (UnknownPoints[1].y - UnknownPoints[3].y) / (Edges[0].Length * Edges[0].Length));
            double v8b8 = -(Rho * (UnknownPoints[1].x - UnknownPoints[3].x) / (Edges[0].Length * Edges[0].Length)) + (Rho * (UnknownPoints[3].x - UnknownPoints[2].x) / (Edges[2].Length * Edges[2].Length));
            AngleEquation angleEquation8 = new AngleEquation(0, 0, v8b3, v8b4, v8b5, v8b6, v8b7, v8b8);
            equations.Add(angleEquation1);
            equations.Add(angleEquation2);
            equations.Add(angleEquation3);
            equations.Add(angleEquation4);
            equations.Add(angleEquation5);
            equations.Add(angleEquation6);
            equations.Add(angleEquation7);
            equations.Add(angleEquation8);

            //v9系数
            double v937 = (UnknownPoints[1].x - UnknownPoints[3].x) / Edges[0].Length;
            double v948 = (UnknownPoints[1].y - UnknownPoints[3].y) / Edges[0].Length;
            AngleEquation angleEquation9 = new AngleEquation(0, 0, -v937, -v948, 0, 0, v937, v948);

            //v10系数
            double v10b3 = -(KnownPoints[1].x - UnknownPoints[1].x) / Edges[1].Length;
            double v10b4 = -(UnknownPoints[3].y - KnownPoints[1].y) / Edges[1].Length;
            AngleEquation angleEquation10 = new AngleEquation(0, 0, v10b3, v10b4, 0, 0, 0, 0);

            //v11系数
            double v1157 = (UnknownPoints[2].x - UnknownPoints[3].x) / Edges[2].Length;
            double v1168 = (UnknownPoints[2].y - UnknownPoints[3].y) / Edges[2].Length;
            AngleEquation angleEquation11 = new AngleEquation(0, 0, 0, 0, -v1157, -v1168, v1157, v1168);
            equations.Add(angleEquation9);
            equations.Add(angleEquation10);
            equations.Add(angleEquation11);

            #endregion

            //计算l
            #region 
            double l1 = chushifangweijiao.Radians - fangweijiao31.Radians - Angles[0].Radians-Math.PI;
            double l2 = -chushifangweijiao.Radians - Angles[1].Radians+Math.PI+fangweijiao24.Radians;
            double l3 = -fangweijiao43.Radians +fangweijiao31.Radians - Angles[2].Radians + Math.PI;
            double l4 = fangweijiao43.Radians - fangweijiao53.Radians - Angles[3].Radians;
            double l5 = fangweijiao46.Radians - fangweijiao43.Radians - Angles[4].Radians;
            double l6 = fangweijiao43.Radians + Math.PI - fangweijiao24.Radians - Angles[5].Radians;
            double l7 = fangweijiao53.Radians + Math.PI - fangweijiao65.Radians - Angles[6].Radians;
            double l8 = -fangweijiao46.Radians + fangweijiao65.Radians - Angles[7].Radians + Math.PI;
            ls.Add(-l1);
            ls.Add(-l2);
            ls.Add(-l3);
            ls.Add(-l4);
            ls.Add(-l5);
            ls.Add(-l6);
            ls.Add(-l7);
            ls.Add(-l8);
            double l9 = Algo.CalculateDistance(UnknownPoints[1], UnknownPoints[3]) - Edges[0].Length;
            double l10 = Algo.CalculateDistance(KnownPoints[1], UnknownPoints[1]) - Edges[1].Length;
            double l11 = Algo.CalculateDistance(UnknownPoints[2], UnknownPoints[3]) - Edges[2].Length;
            ls.Add(l9);
            ls.Add(l10);
            ls.Add(l11);
            #endregion

            tabControl1.SelectedTab = tabPage2;


        }

        private void 结果toolStripButton4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
            if (UnknownPoints[1] == null)
            {
                MessageBox.Show("数据缺失!");
                return;
            }
            var B = Matrix<double>.Build.Dense(11, 8); // 使用double类型而不是Complex类型
                                                       
            for (int i = 0; i < equations.Count; i++)
            {
                var equation = equations[i];
                B[i, 0] = equation.CoefficientX1;
                B[i, 1] = equation.CoefficientY1;
                B[i, 2] = equation.CoefficientX2;
                B[i, 3] = equation.CoefficientY2;
                B[i, 4] = equation.CoefficientX3;
                B[i, 5] = equation.CoefficientY3;
                B[i, 6] = equation.CoefficientX4;
                B[i, 7] = equation.CoefficientY4;
            }

            var BTranspose = B.Transpose(); // 转置矩阵B

            double[] l = new double[] { ls[0] * Rho, ls[1] * Rho, ls[2] * Rho, ls[3] * Rho, ls[4] * Rho, ls[5] * Rho, ls[6] * Rho, ls[7] * Rho, ls[8], ls[9], ls[10] };


            double[,] P = new double[11, 11];
            for (int i = 0; i <11 ; i++)
            {
                P[i, i] =1;
            }

            Matrix<double> PMatrix = Matrix<double>.Build.DenseOfArray(P);
            // 计算 NBB = B^T * P * B
            Matrix<double> NBB = B.Transpose().Multiply(PMatrix).Multiply(B); MathNet.Numerics.LinearAlgebra.Vector<double> lVector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(l);
            MathNet.Numerics.LinearAlgebra.Vector<double> w = B.Transpose().Multiply(PMatrix).Multiply(lVector);
            MathNet.Numerics.LinearAlgebra.Vector<double> x = NBB.Inverse().Multiply(w);
            Matrix<double> Qxx = NBB.Inverse();
            MathNet.Numerics.LinearAlgebra.Vector<double> v = B.Multiply(x) - lVector;
            Qxxquan = Qxx;
            for (int i = 0; i < 11; i++)
            {
                dataGridView2.Rows.Add($"v{i + 1}", Math.Round(v[i],3));
            }
            for (int i = 0; i < 8; i++)
            {
                dataGridView3.Rows.Add($"{i + 1}", $"x{i + 1}", Math.Round(x[i],3));
            }
            Matrix<double> vmatrix = Matrix<double>.Build.Dense(11, 1);

            for (int i = 0; i < 11; i++)
            {
                vmatrix[i, 0] = v[i];
            }
            double result = (vmatrix.Transpose().Multiply(PMatrix).Multiply(vmatrix))[0, 0]; 
            var seigema = Math.Round(Math.Sqrt(result) / 3, 1);
            seigemaquan = seigema;
            var Dxx = seigema * seigema * Qxx;
            double p4jingdu = Dxx[3, 3] + Dxx[4, 4];
            double p5jingdu = Dxx[5, 5] + Dxx[6, 6];

            string BOutput = GetFormattedMatrixString(B, "B");
            string NBBOutput = GetFormattedMatrixString(NBB, "NBB");
            string wOutput = GetFormattedVectorString(w, "w");
            string xOutput = GetFormattedVectorString(x, "x");
            string QxxOutput = GetFormattedMatrixString(Qxx, "Qxx");
            string vOutput = GetFormattedVectorString(v, "v");
            string seigemaOutput = GetFormattedScalarString(seigema, "seigema");
            string p4jingduOutput = GetFormattedScalarString(p4jingdu, "4号点精度");
            string p5jingduOutput = GetFormattedScalarString(p5jingdu, "5号点精度");
            richTextBox2.AppendText(BOutput);
            richTextBox3.AppendText(QxxOutput);

            richTextBox1.Clear();
            richTextBox1.AppendText(BOutput);
            richTextBox1.AppendText(NBBOutput);
            richTextBox1.AppendText(wOutput);
            richTextBox4.AppendText(wOutput);
            richTextBox1.AppendText(xOutput);
            richTextBox4.AppendText(xOutput);
            richTextBox1.AppendText(QxxOutput);
            richTextBox1.AppendText(vOutput);
            richTextBox4.AppendText(vOutput);
            richTextBox1.AppendText(seigemaOutput);
            richTextBox1.AppendText(p4jingduOutput);
            richTextBox4.AppendText(p4jingduOutput);
            richTextBox1.AppendText(p5jingduOutput);
            richTextBox4.AppendText(p5jingduOutput);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("更新后的点坐标：");

            for (int i = 0; i < UnknownPoints.Count; i++)
            {
                double originalX = UnknownPoints[i].x;
                double originalY = UnknownPoints[i].y;

                // 获取 v 中对应位置的数值，并乘以 0.001
                double vValue = v[i] ;
                double vValue2 = v[i + 1];

                // 更新点的坐标
                double newX = Math.Round(originalX + vValue, 3);
                double newY = Math.Round(originalY + vValue2, 3);

                // 创建新的点，并将其添加到 updatedPoints 列表中
                updatedPoints.Add(new Point(newX, newY));
                sb.AppendFormat("点 {0}: ({1}, {2})\n", i + 3, updatedPoints[i].x, updatedPoints[i].y);
            }
            dataGridView4.Rows.Add('x', KnownPoints[0].x, KnownPoints[1].x, UnknownPoints[0].x, UnknownPoints[1].x, UnknownPoints[2].x, UnknownPoints[3].x);
            dataGridView4.Rows.Add('y', KnownPoints[0].y, KnownPoints[1].y, UnknownPoints[0].y, UnknownPoints[1].y, UnknownPoints[2].y, UnknownPoints[3].y);



            richTextBox1.AppendText(sb.ToString());
        }

        private string GetFormattedMatrixString(Matrix<double> matrix, string matrixName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"矩阵 {matrixName}:");

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    double roundedValue = Math.Round(matrix[i, j], 3);
                    sb.Append(roundedValue.ToString("0.000") + " ");
                }
                sb.AppendLine();
            }

            sb.AppendLine(); // 添加空行作为分隔
            return sb.ToString();
        }

        private string GetFormattedVectorString(MathNet.Numerics.LinearAlgebra.Vector<double> vector, string vectorName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"向量 {vectorName}:");

            for (int i = 0; i < vector.Count; i++)
            {
                double roundedValue = Math.Round(vector[i], 3);
                sb.Append(roundedValue.ToString("0.000") + " ");
            }

            sb.AppendLine(); // 添加换行符
            sb.AppendLine(); // 添加空行作为分隔
            return sb.ToString();
        }

        private string GetFormattedScalarString(double scalar, string scalarName)
        {
            return $"标量 {scalarName}: {Math.Round(scalar, 3)}\n\n";
        }

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void 新建文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 创建一个 SaveFileDialog 对象
            SaveFileDialog sfd = new SaveFileDialog();

            // 设置对话框的标题和文件扩展名过滤器
            sfd.Title = "选择文件路径";
            sfd.Filter = "文本文件 (*.txt)|*.txt";

            // 显示对话框并获取用户的选择
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件路径和文件名
                string filePath = sfd.FileName;

                // 创建文件
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    // 写入指定格式的内容
                    sw.WriteLine("格式:");
                    sw.WriteLine("点号,x,y");
                    sw.WriteLine("端点1,端点2,角度");
                    sw.WriteLine("端点1,端点2,长度");
                }

                // 提示用户文件已创建
                MessageBox.Show("文件已创建。");
            }
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 假设 richTextBox1 是您的富文本框控件
            StringBuilder sb = new StringBuilder();

            // 添加帮助文档内容
            sb.AppendLine("欢迎使用我们的导线网平差及精度评定程序！");
            sb.AppendLine();
            sb.AppendLine("**打开文件**");
            sb.AppendLine("点击菜单栏上的 '文件' 选项，然后选择 '打开'。");
            sb.AppendLine("您可以选择一个包含导线网数据的文件，例如 '导线网数据.txt'。");
            sb.AppendLine();
            sb.AppendLine("**初始化**");
            sb.AppendLine("在打开文件后，您需要点击 '初始化' 按钮来准备数据。");
            sb.AppendLine("初始化步骤将读取文件中的数据并将其加载到程序中。");
            sb.AppendLine();
            sb.AppendLine("**平差计算**");
            sb.AppendLine("初始化完成后，您需要点击 '平差计算' 按钮来开始计算。");
            sb.AppendLine("平差计算将使用您提供的数据来计算导线网的精度。");
            sb.AppendLine();
            sb.AppendLine("**结果**");
            sb.AppendLine("计算完成后，您可以在界面上直接查看结果。");
            sb.AppendLine("您还可以点击 '保存报告' 或 '保存图片' 按钮来保存结果。");
            sb.AppendLine();
            sb.AppendLine("**新建文件**");
            sb.AppendLine("点击菜单栏上的 '文件' 选项，然后选择 '新建文件'。");
            sb.AppendLine("这将创建一个新的文件，并显示数据格式。");
            sb.AppendLine();
            sb.AppendLine("**图像操作**");
            sb.AppendLine("点击菜单栏上的 '图像' 选项，然后选择 '生成误差椭圆'。");
            sb.AppendLine("这将使用计算结果生成误差椭圆的图像。");

            // 使用 MessageBox 显示帮助文档内容，并设置字体大小
            MessageBox.Show(sb.ToString(), "帮助文档", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly | MessageBoxOptions.RightAlign);
        }

        private void 数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 保存当前报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 创建一个 SaveFileDialog 对象
            SaveFileDialog sfd = new SaveFileDialog();

            // 设置对话框的标题和文件扩展名过滤器
            sfd.Title = "选择文件路径";
            sfd.Filter = "文本文件 (*.txt)|*.txt";

            // 显示对话框并获取用户的选择
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件路径和文件名
                string filePath = sfd.FileName;

                // 创建文件并写入富文本框内容
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write(richTextBox1.Text);
                }

                // 提示用户文件已保存
                MessageBox.Show("文件已保存。");
            }
        }

        private void 保存图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Bitmap Image (*.bmp)|*.bmp|JPEG Image (*.jpg)|*.jpg|PNG Image (*.png)|*.png";
            saveDialog.Title = "保存误差椭圆";
            saveDialog.ShowDialog();

            if (saveDialog.FileName != "")
            {
                Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bitmap, pictureBox1.ClientRectangle);

                string fileName = saveDialog.FileName;
                ImageFormat format = ImageFormat.Jpeg;
                if (fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    format = ImageFormat.Bmp;
                }
                else if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    format = ImageFormat.Png;
                }

                bitmap.Save(saveDialog.FileName, format);
            }
        }

        private void 误差椭圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Properties.Resources.yuan;
            tabControl1.SelectedTab = tabPage3;
        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                pictureBox2.SizeMode = PictureBoxSizeMode.Normal;
            }
        }

        private void 路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "11";
            textBox2.Text = "8";
            textBox3.Text = "3";
            textBox4.Text = "8";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (UnknownPoints == null)
            {
                MessageBox.Show("缺少数据,目前无法显示图像");
                return;
            }
            // 清除Chart控件中的现有系列和ChartArea
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            // 添加一个新的ChartArea
            var chartArea = chart1.ChartAreas.Add("Default");

            // 创建新的数据系列用于已知点
            var knownSeries = chart1.Series.Add("Known Points");
            knownSeries.ChartType = SeriesChartType.Point;
            knownSeries.MarkerStyle = MarkerStyle.Circle;
            knownSeries.MarkerSize = 10;

            // 将已知点添加到已知点系列中
            foreach (var point in KnownPoints)
            {
                knownSeries.Points.AddXY(point.x, point.y);
            }

            // 创建新的数据系列用于未知点
            var unknownSeries = chart1.Series.Add("Unknown Points");
            unknownSeries.ChartType = SeriesChartType.Point;
            unknownSeries.MarkerStyle = MarkerStyle.Cross;
            unknownSeries.MarkerSize = 10;

            // 将未知点添加到未知点系列中
            foreach (var point in UnknownPoints)
            {
                unknownSeries.Points.AddXY(point.x, point.y);
            }

            // 设置X轴和Y轴的标题和间隔
            chartArea.AxisX.Title = "X轴";
            chartArea.AxisY.Title = "Y轴";
            chartArea.AxisX.Interval = 1000; // 根据需要调整间隔
            chartArea.AxisY.Interval = 1000; // 根据需要调整间隔

            // 根据点的范围设置X轴和Y轴的最小值和最大值
            double xMin = KnownPoints.Concat(UnknownPoints).Min(p => p.x);
            double xMax = KnownPoints.Concat(UnknownPoints).Max(p => p.x);
            double yMin = KnownPoints.Concat(UnknownPoints).Min(p => p.y);
            double yMax = KnownPoints.Concat(UnknownPoints).Max(p => p.y);

            chartArea.AxisX.Minimum = xMin - 50; // 根据需要调整最小值
            chartArea.AxisX.Maximum = xMax + 50; // 根据需要调整最大值
            chartArea.AxisY.Minimum = yMin - 50; // 根据需要调整最小值
            chartArea.AxisY.Maximum = yMax + 50; // 根据需要调整最大值
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.Format = "0.000";
            chartArea.AxisY.LabelStyle.Format = "0.000";

            // 更新Chart控件的显示
            chart1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }
    }
}
