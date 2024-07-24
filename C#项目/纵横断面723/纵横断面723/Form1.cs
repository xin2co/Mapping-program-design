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

namespace 纵横断面723
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        double H0;
        Point A = new Point();
        Point B = new Point();
        List<Point> allPoints = new List<Point>();
        List<Point> ncPoints = new List<Point>();
        List<Point> M0Points = new List<Point>();
        List<Point> M1Points = new List<Point>();
        List<Point> KPoints = new List<Point>();
        List<Point> KpPoints = new List<Point>();

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
                H0 = double.Parse(all_lines[0].Split(',')[1]);
                A.x = double.Parse(all_lines[2].Split(',')[1]);
                A.y = double.Parse(all_lines[2].Split(',')[2]);
                B.x = double.Parse(all_lines[3].Split(',')[1]);
                B.y = double.Parse(all_lines[3].Split(',')[2]);

                for (int i = 5; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point s = new Point()
                    {
                        id = parts[0],
                        x = double.Parse(parts[1]),
                        y = double.Parse(parts[2]),
                        z = double.Parse(parts[3]),
                    };
                    if (s.id.StartsWith("K"))
                    {
                        KPoints.Add(s);
                    }
                    allPoints.Add(s);
                    dataGridView1.Rows.Add(s.id, s.x, s.y, s.z);
                }
                toolStripStatusLabel1.Text = "文件导入成功";
                toolStripStatusLabel2.Text = "H0:" + H0;
            }
            catch
            {
                MessageBox.Show("文件导入失败");
                return;
            }
        }
        private double Calfangwei(Point p1, Point p2)
        {
            double deltaX = p2.x - p1.x;
            double deltaY = p2.y - p1.y;
            double degree = Math.Atan2(deltaY, deltaX);
            return degree;

        }
        private string Rad2dms(double rad)
        {
            double degrees = rad * 180 / Math.PI;
            double deg = (int)degrees;
            double min = (int)((degrees - deg) * 60);
            double sec = ((degrees - deg) * 60 - min) * 60;
            string str = deg.ToString() + min.ToString() + sec.ToString("F4");
            return str;
        }
        private void 方位角AbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double re = Calfangwei(A, B);
            string str = Rad2dms(re);
            richTextBox1.Text = "AB方位角为:" + str;
            tabControl1.SelectedTab = tabPage2;
            statusStrip1.Text = "方位角计算成功";

        }
        private double Distance(Point p1, Point p2)
        {
            double result = (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
            result = Math.Sqrt(result);
            return result;
        }
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
                MessageBox.Show("无法计算高程，因为权重总和为零。");
                return 0;
            }
            double pHeight = sumWeightedHeights / sumWeights;


            return pHeight;
        }
        private void 插值K0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double re = Neicha(KPoints[1], allPoints);
            richTextBox1.Text = "K1插值高程为:" + re;
            tabControl1.SelectedTab = tabPage2;
            statusStrip1.Text = "插值K1计算成功";
        }

        private double CalArea(List<Point> list)
        {
            double sums = 0;
            for (int i = 0; i < list.Count - 1; i++)
            {
                double L = Distance(list[i], list[i + 1]);
                double dh = list[i].z + list[i + 1].z - 2 * H0;
                double s = dh / 2 * L;
                sums += s;
            }
            return sums;
        }

        private void 梯形面积ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Point> K = new List<Point>();
            K.Add(KPoints[0]);
            K.Add(KPoints[1]);
            double re = CalArea(K);
            richTextBox1.Text = "面积为:" + re;
            tabControl1.SelectedTab = tabPage2;
            statusStrip1.Text = "梯形面积计算成功";
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sumD = 0;

            for (int i = 0; i < KPoints.Count() - 1; i++)
            {
                sumD += Distance(KPoints[i], KPoints[i + 1]);
            }
            richTextBox1.Text = "长度:" + sumD;


            KpPoints.Add(KPoints[0]);
            double buchang = 10;
            for (int j = 0; j < KPoints.Count - 1; j++)
            {
                while (buchang < Distance(KPoints[0], KPoints[j + 1]))
                {
                    double alpha = Calfangwei(KPoints[j], KPoints[j + 1]);
                    double dis = buchang - Distance(KPoints[0], KPoints[j]);
                    Point p = new Point()
                    {
                        id = $"V-{buchang / 10}",
                        x = KPoints[j].x + dis * Math.Cos(alpha),
                        y = KPoints[j].y + dis * Math.Sin(alpha),
                        licheng = buchang
                    };
                    p.z = Neicha(p, allPoints);
                    KpPoints.Add(p);

                    buchang += 10;
                }
                KpPoints.Add(KPoints[j + 1]);
            }
            Shuchu(KpPoints);
            Point M0 = Calmidle(KPoints[0], KPoints[1]);
            Point M1 = Calmidle(KPoints[1], KPoints[2]);
            richTextBox1.AppendText("M0(" + M0.x.ToString("F3") + "," + M0.y.ToString("F3") + ")" + "\n");
            richTextBox1.AppendText("M1(" + M1.x.ToString("F3") + "," + M1.y.ToString("F3") + ")" + "\n");
            double angm1 = Calfangwei(KPoints[0], KPoints[1]) + Math.PI / 2;
            double angm2 = Calfangwei(KPoints[1], KPoints[2]) + Math.PI / 2;
            for (int jj = -5; jj <= 5; jj++)
            {
                Point mp1 = new Point()
                {
                    id = $"M0-{jj + 6}",
                    x = M0.x + jj * 5 * Math.Cos(angm1),
                    y = M0.y + jj * 5 * Math.Sin(angm1),
                };
                Point mp2 = new Point()
                {
                    id = $"M1-{jj + 6}",
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
            richTextBox1.AppendText("  点名   里程   X(m)       Y(m)       Z(m)\n");
            foreach (var point in list)
            {
                string formattedName = point.id.PadLeft(7);
                string formattedlc = point.licheng.ToString("F3").PadLeft(7);
                string formattedX = point.x.ToString("F3").PadLeft(11);
                string formattedY = point.y.ToString("F3").PadLeft(11);
                string formattedZ = point.z.ToString("F3").PadLeft(9);

                richTextBox1.AppendText($"{formattedName}{formattedlc}{formattedX}{formattedY}{formattedZ}\n");
            }
        }

        private void 终点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            Series series0 = new Series()
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
            };
            Series series1 = new Series()
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Blue,
            };
            foreach (Point p in M0Points)
            {
                series0.Points.AddXY(p.licheng, p.z);

            }
            foreach (Point p in M1Points)
            {
                series1.Points.AddXY(p.licheng, p.z);

            }
            chart1.Series.Add(series0);
            chart1.Series.Add(series1);

        }

        private void 插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            Series series = new Series()
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
            };
            foreach(Point p in KpPoints)
            {
                series.Points.AddXY(Distance(KPoints[0], p), p.z);

            }
            chart1.Series.Add(series);


        }
    }
}
