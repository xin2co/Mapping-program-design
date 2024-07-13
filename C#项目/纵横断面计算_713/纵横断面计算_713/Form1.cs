using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 纵横断面计算_713
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        double H0;
        string[] ks;
        Point A = new Point();
        Point B = new Point();
        List<Point> allPoints = new List<Point>();
        List<Point> ncPoints = new List<Point>();
        List<Point> kPoints = new List<Point>();
        List<Point> kpPoints = new List<Point>();
        List<Point> M0Points = new List<Point>();
        List<Point> M1Points = new List<Point>();
        public Form1()
        {
            InitializeComponent();
        }


        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                all_lines = File.ReadAllLines(op.FileName, Encoding.Default);
            }
            else
            {
                return;
            }
            try
            {
                string[] fistline = all_lines[0].Split(',');
                H0 = double.Parse(fistline[1]);
                ks = all_lines[1].Split(',');
                string[] Aline = all_lines[2].Split(',');
                A.name = Aline[0];
                A.x = double.Parse(Aline[1]);
                A.y = double.Parse(Aline[2]);

                string[] Bline = all_lines[3].Split(',');
                B.name = Bline[0];
                B.x = double.Parse(Bline[1]);
                B.y = double.Parse(Bline[2]);

                for (int i = 5; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    if (parts.Length == 4)
                    {
                        Point po = new Point
                        {
                            name = parts[0],
                            x = double.Parse(parts[1]),
                            y = double.Parse(parts[2]),
                            z = double.Parse(parts[3]),
                        };
                        allPoints.Add(po);
                        dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3]);
                    }
                    toolStrip1.Text = "H0: " + H0;
                }
                kPoints = allPoints.Where(p => p.name.StartsWith("K")).ToList();
            }
            catch
            {
                MessageBox.Show("读取失败");
                return;
            }
        }

        //计算方位角
        private double Calfangwei(Point p1, Point p2)
        {
            double deltaX = p2.x - p1.x;
            double deltaY = p2.y - p1.y;
            double degree = Math.Atan2(deltaY, deltaX);
            return degree;

        }

        //角度输出规定格式
        private string Rad2dms(double rad)
        {
            double degrees = rad * 180 / Math.PI;
            double deg = (int)degrees;
            double min = (int)((degrees - deg) * 60);
            double sec = ((degrees - deg) * 60 - min) * 60;
            string str = deg.ToString() + min.ToString() + sec.ToString("F4");
            return str;
        }

        //测试AB方位角
        private void AB方位角ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double angAB = Calfangwei(A, B);
            string ang = Rad2dms(angAB);
            richTextBox1.Text = "AB方位角为:" + ang + '\n';
            tabControl1.SelectTab(tabPage2);
        }

        //内插
        private double Neicha(Point p, List<Point> list)
        {
            List<Point> nearestPoints = list.Where(point => point != p).OrderBy(point
                => Distance(p, point)).Take(5).ToList();
            double sumWeightedHeights = 0;
            double sumWeights = 0;

            foreach (Point pp in nearestPoints)
            {
                double distance = Distance(p, pp);
                double weight = 1 / distance;
                sumWeightedHeights += pp.z * weight;
                sumWeights += weight;
            }
            ncPoints.AddRange(nearestPoints);
            if (sumWeights == 0)
            {
                MessageBox.Show("无法计算点K1的高程，因为权重总和为零。");
                return 0;
            }
            double pHeight = sumWeightedHeights / sumWeights;


            return pHeight;
        }

        //计算距离
        private double Distance(Point p1, Point p2)
        {
            double dx = p1.x - p2.x;
            double dy = p1.y - p2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        //测试K1内插
        private void K1内插点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point K1 = allPoints.FirstOrDefault(point => point.name == "K1");

            if (K1 == null)
            {
                MessageBox.Show("未找到点号K1的点。");
                return;
            }

            string output = $"以k1为内插点，最近5个点的点号、坐标(X,Y,H)和距离如下:" + '\n';
            double pHeight = Neicha(K1, allPoints);

            foreach (Point Qi in ncPoints)
            {
                double distance = Distance(K1, Qi);
                output += $"{Qi.name}\t{Qi.x,10:0.000}\t{Qi.y,10:0.000}\t{Qi.z,7:0.000}\t{distance,7:0.000}\n";
            }
            output += $"内插点K1的内插高程为：{pHeight,7:0.000} 米";

            richTextBox1.Text += output;
            tabControl1.SelectTab(tabPage2);
        }

        //计算面积
        private double CalculateTotalArea(List<Point> list)
        {
            double totalArea = 0;
            Point previousPoint = null;

            foreach (var point in list)
            {
                if (previousPoint != null)
                {
                    double deltaL = Distance(previousPoint, point);
                    double currentArea = (point.z + previousPoint.z - 2 * H0) / 2 * deltaL;
                    totalArea += currentArea;
                }

                previousPoint = point;
            }

            return totalArea;
        }

        private void K0k1梯形面积ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Point> k12Points = new List<Point>();
            k12Points.Add(kPoints[0]);
            k12Points.Add(kPoints[1]);
            string output = $"梯形面积计算结果：\n";
            double totalArea = CalculateTotalArea(k12Points);
            output += $"从K0到K1之间的面积为：{totalArea.ToString("F3")} 平方米\n";
            richTextBox1.Text = output;
            tabControl1.SelectTab(tabPage2);
        }

        private void 计算ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            richTextBox1.Clear();
            kpPoints.Add(kPoints[0]);
            double totalDistance = 0;
            for (int i = 0; i < kPoints.Count - 1; i++)
            {
                totalDistance += Distance(kPoints[i], kPoints[i + 1]);
            }
            richTextBox1.Text += "纵断面的长度: " + Math.Round(totalDistance, 3) + "m\n";

            int j = 0;
            for (double buchang = 10; buchang < totalDistance; buchang += 10)
            {
                if (buchang < Distance(kPoints[0], kPoints[1]))
                {
                    Point p = new Point()
                    {
                        name = $"V-{(int)buchang / 10}",
                        x = kPoints[0].x + buchang * Math.Cos(Calfangwei(kPoints[0], kPoints[1])),
                        y = kPoints[0].y + buchang * Math.Sin(Calfangwei(kPoints[0], kPoints[1])),

                    };
                    p.z = Neicha(p, allPoints);
                    kpPoints.Add(p);

                }
                else
                {
                    if (j == 0)
                    {
                        kpPoints.Add(kPoints[1]);
                    }
                    j = 1;
                    Point p = new Point()
                    {
                        name = $"V-{(int)buchang / 10 + 1}",
                        x = kPoints[j].x + (buchang - Distance(kPoints[0], kPoints[j])) * Math.Cos(Calfangwei(kPoints[j], kPoints[j + 1])),
                        y = kPoints[j].y + (buchang - Distance(kPoints[0], kPoints[j])) * Math.Sin(Calfangwei(kPoints[j], kPoints[j + 1])),
                    };
                    p.z = Neicha(p, allPoints);
                    kpPoints.Add(p);
                }

            }
            kpPoints.Add(kPoints[2]);
            Shuchu(kpPoints);
            Point M0 = Calmidle(kPoints[0], kPoints[1]);
            Point M1 = Calmidle(kPoints[1], kPoints[2]);
            richTextBox1.AppendText("M0(" + M0.x.ToString("F3") + "," + M0.y.ToString("F3") + ")" + "\n");
            richTextBox1.AppendText("M1(" + M1.x.ToString("F3") + "," + M1.y.ToString("F3") + ")" + "\n");
            double angm1 = Calfangwei(kPoints[0], kPoints[1]) + Math.PI / 2;
            double angm2 = Calfangwei(kPoints[1], kPoints[2]) + Math.PI / 2;

            for (int jj = -5; jj <= 5; jj++)
            {
                Point mp1 = new Point()
                {
                    name = $"M0-{jj + 6}",
                    x = M0.x + jj * 5 * Math.Cos(angm1),
                    y = M0.y + jj * 5 * Math.Sin(angm1),
                };
                Point mp2 = new Point()
                {
                    name = $"M1-{jj + 6}",
                    x = M1.x + jj * 5 * Math.Cos(angm2),
                    y = M1.y + jj * 5 * Math.Sin(angm2),
                };
                mp1.z = Neicha(mp1, allPoints);
                mp2.z = Neicha(mp2, allPoints);
                M0Points.Add(mp1);
                M1Points.Add(mp2);
            }
            Shuchu(M0Points);
            Shuchu(M1Points);
            tabControl1.SelectedTab = tabPage2;
        }

        //输出格式
        private void Shuchu(List<Point> list)
        {
            double listarea = CalculateTotalArea(list);

            richTextBox1.AppendText("面积为:" + Math.Round(listarea, 3) + "\n");
            richTextBox1.AppendText("  点名      X(m)       Y(m)       Z(m)\n");
            foreach (var point in list)
            {
                string formattedName = point.name.PadLeft(7);
                string formattedX = point.x.ToString("F3").PadLeft(11);
                string formattedY = point.y.ToString("F3").PadLeft(11);
                string formattedZ = point.z.ToString("F3").PadLeft(9);

                richTextBox1.AppendText($"{formattedName}{formattedX}{formattedY}{formattedZ}\n");
            }

        }

        //计算中点
        private Point Calmidle(Point p1, Point p2)
        {
            Point midle = new Point();
            midle.x = (p1.x + p2.x) / 2;
            midle.y = (p1.y + p2.y) / 2;
            return midle;
        }

        private void 内插ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (kPoints.Count > 0)
            {
                Point K0 = kPoints[0];

                ChartArea chartArea = new ChartArea();
                chart1.ChartAreas.Clear();
                chart1.ChartAreas.Add(chartArea);

                Series series = new Series
                {
                    Name = "H",
                    Color = Color.Blue,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Point
                };
                chart1.Series.Clear();
                chart1.Series.Add(series);

                // 添加数据点到 Series
                foreach (var point in kpPoints)
                {
                    double distanceToK0 = Distance(K0, point);
                    double elevation = point.z;
                    series.Points.AddXY(distanceToK0, elevation);
                }

                chart1.Titles.Clear();
                chart1.Titles.Add("内插图");
                chartArea.AxisX.Title = "到K0的距离 (m)";
                chartArea.AxisY.Title = "H (m)";
                tabControl1.SelectedTab = tabPage3;

            }
        }

        private void 中点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (M0Points.Count > 0 && M1Points.Count > 0)
            {
                ChartArea chartArea = new ChartArea();
                chart1.ChartAreas.Clear();
                chart1.ChartAreas.Add(chartArea);
                chart1.Series.Clear();
                // 创建M0Points的Series
                Series seriesM0 = new Series
                {
                    Name = "M0",
                    Color = Color.Red,
                    IsVisibleInLegend = true,
                    LegendText = "M0",
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Point
                };
                chart1.Series.Add(seriesM0);

                // 创建M1Points的Series
                Series seriesM1 = new Series
                {
                    Name = "M1",
                    Color = Color.Green,
                    IsVisibleInLegend = true,
                    LegendText = "M1",
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Point
                };
                chart1.Series.Add(seriesM1);

                foreach (var point in M0Points)
                {
                    seriesM0.Points.AddXY(Math.Round(Distance(kPoints[0], point),1), point.z);
                }

                foreach (var point in M1Points)
                {
                    seriesM1.Points.AddXY(Math.Round(Distance(kPoints[0], point),1), point.z);
                }

                Legend legend = new Legend();
                legend.Name = "MainLegend";
                legend.Docking = Docking.Bottom;
                chart1.Legends.Clear();
                chart1.Legends.Add(legend);

                chart1.Titles.Clear();
                chart1.Titles.Add("中点图");
                chartArea.AxisX.Title = "X坐标 (m)";
                chartArea.AxisY.Title = "H (m)";
                tabControl1.SelectedTab = tabPage3;
            }
        }

    }
}
