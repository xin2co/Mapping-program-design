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

namespace 纵横断面计算练习718
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        double H0;
        List<Point> allpoints = new List<Point>();
        Point A = new Point();
        Point B = new Point();
        List<Point> ncPoints = new List<Point>();
        List<Point> KPoints = new List<Point>();
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
                string[] firstparts = all_lines[0].Split(',');
                H0 = double.Parse(firstparts[1]);
                string[] aparts = all_lines[2].Split(',');
                string[] bparts = all_lines[3].Split(',');
                A.x = double.Parse(aparts[1]);
                A.y = double.Parse(aparts[2]);
                B.x = double.Parse(bparts[1]);
                B.y = double.Parse(bparts[2]);
                for (int i = 5; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point p = new Point()
                    {
                        name = parts[0],
                        x = double.Parse(parts[1]),
                        y = double.Parse(parts[2]),
                        z = double.Parse(parts[3])
                    };
                    allpoints.Add(p);
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3]);
                }
                foreach (Point p in allpoints)
                {
                    if (p.name.StartsWith("K"))
                    {
                        KPoints.Add(p);
                    }
                    else
                    {
                        continue;
                    }
                }
                toolStripLabel1.Text = "导入成功";
            }
            catch
            {
                MessageBox.Show("导入失败");
                toolStripLabel1.Text = "导入失败";
                return;
            }



            
        }

        private double CalRad(Point p1,Point p2)
        {
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;
            double degree = Math.Atan2(dy, dx);
            return degree;
        }

        private string SetRad2dms(double rad)
        {
            double degrees = rad * 180 / Math.PI;
            double deg = (int)degrees;
            double min = (int)((degrees - deg) * 60);
            double sec = ((degrees - deg) * 60 - min) * 60;
            string str = deg.ToString() + min.ToString() + sec.ToString("F4");
            return str;

        }
        private void 坐标方位角计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double ABangele = CalRad(A, B);
            string re = SetRad2dms(ABangele);
            richTextBox1.Text = "AB方位角为" + re;
            tabControl1.SelectedTab = tabPage2;
        }

        private void p点内插ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point k1 = KPoints[1];
            double k1h = 0;
            k1h = Calneicha(k1, allpoints);
            richTextBox1.Text = "以k1为内插点,最近5个点,坐标及距离如下:"+'\n';
            foreach(Point pp in ncPoints)
            {
                double dis = Distance(k1, pp);
                richTextBox1.Text += pp.name + "\t" + pp.x.ToString("F3") + "\t" + pp.y.ToString("F3") + "\t" + pp.z.ToString("F3") + "\t"+dis.ToString("F3")+ "\n";

            }
            richTextBox1.Text += "k1为内插点的高程为:" + k1h.ToString("F3") + "\n";
            tabControl1.SelectedTab = tabPage2;

        }
        private double Calneicha(Point p,List<Point> list)
        {
            List<Point> zuijin = list.Where(point => point != p).OrderBy(point => Distance(p, point)).Take(5).ToList();
            double sumquangao = 0;
            double sumquan = 0;
            foreach(Point pp in zuijin)
            {
                double dis = Distance(p, pp);
                double quan = 1 / dis;
                sumquangao += pp.z * quan;
                sumquan += quan;
            }
            ncPoints.AddRange(zuijin);
            if (sumquan == 0)
            {
                MessageBox.Show("无法计算K1高程,权重之和为0");
                return 0;
            }
            double h = sumquangao / sumquan;
            return h;
        }
        private double Distance(Point p1, Point p2)
        {
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void 断面面积的计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point k1 = new Point();
            Point k0 = new Point();
            List<Point> KArea = KPoints.Take(2).ToList();
            double s = CalArea(KArea);
            richTextBox1.Text += "k1k2面积为:" + s.ToString("F3")+"m2";
            tabControl1.SelectedTab = tabPage2;
        }
        private double CalArea(List<Point> list)
        {
            double sums = 0;
            for(int i = 0; i < list.Count-1; i++)
            {
                double L = Distance(list[i], list[i + 1]);
                double dh = list[i].z + list[i + 1].z - 2 * H0;
                double s = dh / 2 * L;
                sums += s;
            }
            return sums;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sumD = 0;
            for (int i = 0; i < KPoints.Count - 1; i++)
            {
                double d = Distance(KPoints[i], KPoints[i + 1]);
                sumD += d;
            }
            richTextBox1.Text = "纵断面的总长度为:" + sumD.ToString("F3") + "\n";
            int j = 0;
            kpPoints.Add(KPoints[0]);
            for (double buchang = 10; buchang < sumD; buchang += 10)
            {
                if (buchang < Distance(KPoints[0], KPoints[1]))
                {
                    Point p = new Point()
                    {
                        name = $"V-{(int)buchang / 10}",
                        x = KPoints[0].x + buchang * Math.Cos(CalRad(KPoints[0], KPoints[1])),
                        y = KPoints[0].y + buchang * Math.Sin(CalRad(KPoints[0], KPoints[1])),

                    };
                    p.z = Calneicha(p, allpoints);
                    kpPoints.Add(p);

                }
                else
                {
                    if (j == 0) kpPoints.Add(KPoints[1]);
                    j = 1;
                    Point p = new Point()
                    {
                        name = $"V-{(int)buchang / 10 + 1}",
                        x = KPoints[1].x + (buchang - Distance(KPoints[1], KPoints[0])) * Math.Cos(CalRad(KPoints[1], KPoints[2])),
                        y = KPoints[1].y + (buchang - Distance(KPoints[1], KPoints[0])) * Math.Sin(CalRad(KPoints[1], KPoints[2]))
                    };
                    p.z = Calneicha(p, allpoints);
                    kpPoints.Add(p);
                }

            }
            kpPoints.Add(KPoints[2]);
            Shuchu(kpPoints);
            Point M0 = Calmidle(KPoints[0], KPoints[1]);
            Point M1 = Calmidle(KPoints[1], KPoints[2]);
            richTextBox1.AppendText("M0(" + M0.x.ToString("F3") + "," + M0.y.ToString("F3") + ")" + "\n");
            richTextBox1.AppendText("M1(" + M1.x.ToString("F3") + "," + M1.y.ToString("F3") + ")" + "\n");
            double angm1 = CalRad(KPoints[0], KPoints[1]) + Math.PI / 2;
            double angm2 = CalRad(KPoints[1], KPoints[2]) + Math.PI / 2;
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
                mp1.z = Calneicha(mp1, allpoints);
                mp2.z = Calneicha(mp2, allpoints);
                M0Points.Add(mp1);
                M1Points.Add(mp2);
            }
            Shuchu(M0Points);
            Shuchu(M1Points);
            tabControl1.SelectedTab = tabPage2;
        }
        private Point Calmidle(Point p1, Point p2)
        {
            Point midle = new Point();
            midle.x = (p1.x + p2.x) / 2;
            midle.y = (p1.y + p2.y) / 2;
            return midle;
        }

        private void Shuchu(List<Point> list)
        {
            double listarea = CalArea(list);

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
            tabControl1.SelectedTab = tabPage2;
        }

        private void 内插ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point K0 = KPoints[0];
            ChartArea chartarea = new ChartArea();
            chart1.ChartAreas.Clear();
            chart1.ChartAreas.Add(chartarea);
            Series series = new Series
            {
                Name = "H",
                Color = Color.Blue,
                ChartType = SeriesChartType.Point
            };
            chart1.Series.Clear();
            chart1.Series.Add(series);
            foreach(Point p in kpPoints)
            {
                double dis = Distance(K0, p);
                double element = p.z;
                series.Points.AddXY(dis, element);
            }
            chart1.Titles.Clear();
            chart1.Titles.Add("内插K0");
            chartarea.AxisX.Title = "到K0的距离 (m)";
            chartarea.AxisY.Title = "H (m)";
            tabControl1.SelectedTab = tabPage3;

        }

        private void 终点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (M0Points.Count > 0 && M1Points.Count > 0)
            {
                ChartArea chartArea = new ChartArea();
                chart1.ChartAreas.Clear();
                chart1.ChartAreas.Add(chartArea);
                chart1.Series.Clear();
                Series seriesM0 = new Series
                {
                    Name = "M0",
                    Color = Color.Red,
                    IsVisibleInLegend = true,
                    LegendText = "M0",
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Point
                };
                Series seriesM1 = new Series
                {
                    Name = "M1",
                    Color = Color.Green,
                    IsVisibleInLegend = true,
                    LegendText = "M1",
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Point
                };
                chart1.Series.Add(seriesM0);
                chart1.Series.Add(seriesM1);
                foreach (var point in M0Points)
                {
                    seriesM0.Points.AddXY(Math.Round(Distance(KPoints[0], point), 1), point.z);
                }

                foreach (var point in M1Points)
                {
                    seriesM1.Points.AddXY(Math.Round(Distance(KPoints[0], point), 1), point.z);
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
