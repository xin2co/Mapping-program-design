using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 纵横断面练习_719
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> allpoints = new List<Point>();
        double H0;
        Point A = new Point();
        Point B = new Point();
        List<Point> ncpoints = new List<Point>();
        List<Point> Kpoints = new List<Point>();
        List<Point> KPpoints = new List<Point>();
        List<Point> M0points = new List<Point>();
        List<Point> M1points = new List<Point>();

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
                A.x= double.Parse(all_lines[2].Split(',')[1]);
                A.y= double.Parse(all_lines[2].Split(',')[2]);
                B.x = double.Parse(all_lines[3].Split(',')[1]);
                B.y = double.Parse(all_lines[3].Split(',')[2]);
                for (int i = 5; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point s = new Point()
                    {
                        name = parts[0],
                        x = double.Parse(parts[1]),
                        y = double.Parse(parts[2]),
                        z = double.Parse(parts[3]),
                    };
                    if (s.name.StartsWith("K"))
                    {
                        Kpoints.Add(s);
                    }
                    allpoints.Add(s);
                    dataGridView1.Rows.Add(s.name, s.x, s.y, s.z);
                }
                
                toolStripLabel1.Text = "文件导入成功";
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

        private void AB方位角ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double rad = Calfangwei(A, B);
            string re = Rad2dms(rad);
            richTextBox1.Text = re;
            tabControl1.SelectedTab = tabPage2;
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
            ncpoints.AddRange(nearestPoints);
            if (sumWeights == 0)
            {
                MessageBox.Show("无法计算高程，因为权重总和为零。");
                return 0;
            }
            double pHeight = sumWeightedHeights / sumWeights;


            return pHeight;
        }

        private void K1内插ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double k1h = Neicha(Kpoints[1], allpoints);
            richTextBox1.Text = "以K1插值高程为:" + k1h.ToString("F3")+"m";
            tabControl1.SelectedTab = tabPage2;
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
            List<Point> K0k1 = new List<Point>();
            K0k1.Add(Kpoints[0]);
            K0k1.Add(Kpoints[1]);
            double re = CalArea(K0k1);
            richTextBox1.Text = "K0K1面积为:" + re.ToString("F3");
            tabControl1.SelectedTab = tabPage2;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sumD = 0;
            for (int i = 0; i < Kpoints.Count - 1; i++)
            {
                sumD += Distance(Kpoints[i], Kpoints[i + 1]);
            }
            richTextBox1.Text = "纵断面总长度为:" + sumD.ToString("F3") + "\n";
            int j = 0;
            Kpoints[0].lc = 0;
            KPpoints.Add(Kpoints[0]);
            for (double buchang = 10; buchang < Distance(Kpoints[0], Kpoints[2]); buchang += 10)
            {
                if (buchang < Distance(Kpoints[0], Kpoints[1]))
                {
                    Point p = new Point()
                    {
                        name = $"V-{buchang / 10}",
                        x = Kpoints[0].x + buchang * Math.Cos(Calfangwei(Kpoints[0], Kpoints[1])),
                        y = Kpoints[0].y + buchang * Math.Sin(Calfangwei(Kpoints[0], Kpoints[1])),
                        lc = buchang,
                    };
                    p.z = Neicha(p, allpoints);
                    KPpoints.Add(p);
                }
                else
                {
                    if (j == 0)
                    {
                        Kpoints[1].lc = Distance(Kpoints[1], Kpoints[0]);
                        KPpoints.Add(Kpoints[1]);

                    }
                    j = 1;
                    Point p = new Point()
                    {
                        name = $"V-{buchang / 10}",
                        x = Kpoints[j].x + (buchang - Distance(Kpoints[j], Kpoints[0])) * Math.Cos(Calfangwei(Kpoints[0], Kpoints[1])),
                        y = Kpoints[j].y + (buchang - Distance(Kpoints[j], Kpoints[0])) * Math.Sin(Calfangwei(Kpoints[0], Kpoints[1])),
                        lc = buchang
                    };
                    p.z = Neicha(p, allpoints);
                    KPpoints.Add(p);
                }
                Kpoints[2].lc = Distance(Kpoints[2], Kpoints[0]);

            }
            KPpoints.Add(Kpoints[2]);
            richTextBox1.Text += "内插点信息如下:\n";
            Shuchu(KPpoints);
            Point M0 = Zhongdian(Kpoints[0], Kpoints[1]);
            Point M1 = Zhongdian(Kpoints[1], Kpoints[2]);
            richTextBox1.Text += "M0(" + M0.x.ToString("F3") + ',' + M0.y.ToString("F3") + ")\n";
            richTextBox1.Text += "M1(" + M1.x.ToString("F3") + ',' + M1.y.ToString("F3") + ")\n";
            double angm1 = Calfangwei(Kpoints[0], Kpoints[1]) + Math.PI / 2;
            double angm2 = Calfangwei(Kpoints[1], Kpoints[2]) + Math.PI / 2;
            for (int jj = -5; jj <= 5; jj++)
            {
                Point mp1 = new Point()
                {
                    name = $"M0-{jj + 6}",
                    x = M0.x + jj * 5 * Math.Cos(angm1),
                    y = M0.y + jj * 5 * Math.Sin(angm1),
                    lc = jj * 5
                };
                Point mp2 = new Point()
                {
                    name = $"M1-{jj + 6}",
                    x = M1.x + jj * 5 * Math.Cos(angm2),
                    y = M1.y + jj * 5 * Math.Sin(angm2),
                    lc = jj * 5
                };
                mp1.z = Neicha(mp1, allpoints);
                mp2.z = Neicha(mp2, allpoints);
                M0points.Add(mp1);
                M1points.Add(mp2);
            }
            Shuchu(M0points);
            Shuchu(M1points);
            tabControl1.SelectedTab = tabPage2;
        }

        private Point Zhongdian(Point p1,Point p2)
        {
            Point p = new Point
            {
                x = (p1.x + p2.x) / 2,
                y = (p1.y + p2.y) / 2
            };
            return p;
        }

        private void Shuchu(List<Point> list)
        {
            richTextBox1.Text += "点名\t里程\tx\ty\tz\t\n";
            foreach (Point p in list)
            {
                richTextBox1.Text += p.name + "\t" + p.lc.ToString("F3") + "\t" + p.x.ToString("F3") + "\t" + p.y.ToString("F3") + "\t" + p.z.ToString("F3") + "\n";

            }
        }

        private void 插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            Series series = new Series
            {
                Name = "H",
                Color = Color.Blue,
                ChartType = SeriesChartType.Point
            };
            chart1.Series.Clear();
            chart1.Series.Add(series);
            foreach(Point p in KPpoints)
            {
                series.Points.AddXY(Distance(KPpoints[0], p), p.z);

            }
            tabControl1.SelectedTab = tabPage3;
        }

        private void 终点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            Series M0series = new Series
            {
                Name = "M0",
                Color = Color.Blue,
                ChartType = SeriesChartType.Point
            };
            Series M1series = new Series
            {
                Name = "M1",
                Color = Color.Red,
                ChartType = SeriesChartType.Point
            };
            chart1.Series.Add(M0series);
            chart1.Series.Add(M1series);
            foreach (Point p in M0points)
            {
                M0series.Points.AddXY(p.lc, p.z);
            }
            foreach (Point p in M1points)
            {
                M1series.Points.AddXY(p.lc, p.z);
            }
            Legend legend = new Legend();
            chart1.Legends.Add(legend);
            tabControl1.SelectedTab = tabPage3;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.FileName = "result";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(richTextBox1.Text);
                }
            }
        }

        private void 图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = "拟合曲线.jpg"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                chart1.SaveImage(saveFileDialog.FileName, ImageFormat.Jpeg);
            }
        }
    }
}
