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

namespace 纵横断面练习721
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> allPoints = new List<Point>();
        Point A = new Point();
        Point B = new Point();
        double H0;
        List<Point> ncPoints = new List<Point>();
        List<Point> KPoints = new List<Point>();
        List<Point> KpPoints = new List<Point>();
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
                H0 = double.Parse(all_lines[0].Split(',')[1]);
                string[] Aparts = all_lines[2].Split(',');
                string[] Bparts = all_lines[3].Split(',');
                A.x = double.Parse(Aparts[1]);
                A.y = double.Parse(Aparts[2]);
                B.x = double.Parse(Bparts[1]);
                B.y = double.Parse(Bparts[2]);

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
                        KPoints.Add(s);
                    }
                    allPoints.Add(s);
                    dataGridView1.Rows.Add(s.name, s.x, s.y, s.z);
                }
                toolStripLabel1.Text = "文件导入成功";
                toolStripLabel2.Text = "参考H0:"+H0;
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
        private void aB方位角ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double rad = Calfangwei(A, B);
            string str = Rad2dms(rad);
            richTextBox1.Text = "AB方位角是:" + str;
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
            ncPoints.AddRange(nearestPoints);
            if (sumWeights == 0)
            {
                MessageBox.Show("无法计算高程，因为权重总和为零。");
                return 0;
            }
            double pHeight = sumWeightedHeights / sumWeights;


            return pHeight;
        }
        private void 内插K1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double K1h = Neicha(KPoints[1], allPoints);
            string str = "K1内插高程为:" + K1h + "\n";
            str += "最近五点为:" + "\n";
            foreach(Point p in ncPoints)
            {
                str += p.name + ',' + p.x + ',' + p.y + ',' + p.z + ',' + '\n';
            }
            ncPoints.Clear();
            richTextBox1.Text = str;
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
        private void 面积K1K2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Point> K0K1 = new List<Point>();
            K0K1.Add(KPoints[0]);
            K0K1.Add(KPoints[1]);
            double re = CalArea(K0K1);
            string str = "K0K1梯形面积为:" + re + "\n";
            richTextBox1.Text = str;
            tabControl1.SelectedTab = tabPage2;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sumD = 0;
            for(int i = 0; i < KPoints.Count - 1; i++)
            {
                sumD += Distance(KPoints[i], KPoints[i + 1]);
            }
            string str = "纵横断面总长度是:" + sumD + "\n";
            KpPoints.Add(KPoints[0]);
            int j = 0;

            for (double buchang = 10; buchang < sumD; buchang += 10)
            {
                if (buchang < Distance(KPoints[0], KPoints[1]))
                {
                    double alpha01 = Calfangwei(KPoints[0], KPoints[1]);
                    Point p = new Point()
                    {
                        name = $"V-{buchang / 10}",
                        x = KPoints[0].x + buchang * Math.Cos(alpha01),
                        y = KPoints[0].y + buchang * Math.Sin(alpha01),
                    };
                    p.z = Neicha(p, allPoints);
                    KpPoints.Add(p);

                }
                else if(buchang> Distance(KPoints[0], KPoints[1])&&buchang< Distance(KPoints[0], KPoints[2]))
                {
                    if (j == 0)
                    {
                        KpPoints.Add(KPoints[1]);
                        j = 1;
                    }
                    double alpha02 = Calfangwei(KPoints[1], KPoints[2]);
                    Point p = new Point()
                    {
                        name = $"V-{buchang / 10}",
                        x = KPoints[1].x + (buchang - Distance(KPoints[j], KPoints[0])) * Math.Cos(alpha02),
                        y = KPoints[1].y + (buchang - Distance(KPoints[j], KPoints[0])) * Math.Sin(alpha02),
                    };
                    p.z = Neicha(p, allPoints);
                    KpPoints.Add(p);
                }
            }
            KpPoints.Add(KPoints[2]);
            Shuchu(KpPoints);
            Point M0 = zhongdian(KPoints[0], KPoints[1]);
            Point M1 = zhongdian(KPoints[1], KPoints[2]);
            richTextBox1.Text += "M0坐标" + $"({M0.x},{M0.y})";
            richTextBox1.Text += "M1坐标" + $"({M1.x},{M1.y})";
            double angm1 = Calfangwei(KPoints[0], KPoints[1]) + Math.PI / 2;
            double angm2 = Calfangwei(KPoints[1], KPoints[2]) + Math.PI / 2;
            for(int jj = -5; jj <= 5; jj++)
            {
                Point p = new Point()
                {
                    name = $"M0-{jj + 5}",
                    x = M0.x + jj * 5 * Math.Cos(angm1),
                    y = M0.y + jj * 5 * Math.Sin(angm1),
                };
                p.z = Neicha(p, allPoints);
                M0Points.Add(p);
                Point p1 = new Point()
                {
                    name = $"M1-{jj + 5}",
                    x = M1.x + jj * 5 * Math.Cos(angm2),
                    y = M1.y + jj * 5 * Math.Sin(angm2),
                };
                p1.z = Neicha(p1, allPoints);
                M1Points.Add(p1);
            }
            Shuchu(M0Points);
            Shuchu(M1Points);
            tabControl1.SelectedTab = tabPage2;
        }
        private Point zhongdian(Point p1,Point p2)
        {
            Point p = new Point()
            {
                x = (p1.x + p2.x) / 2,
                y = (p1.y + p2.y) / 2,
            };
            return p;
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

        }

        private void 内插ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            Series series = new Series
            {
                Color = Color.Red,
                ChartType = SeriesChartType.Line,

            };
            foreach (Point p in KpPoints)
            {
                series.Points.AddXY(Distance(p, KPoints[0]), p.z);
            }
            chart1.Series.Add(series);
            tabControl1.SelectedTab = tabPage3;

        }

        private void 终点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            Series seriesM0 = new Series
            {
                Name="M0",
                Color = Color.Red,
                ChartType = SeriesChartType.Point,

            };
            for(int i=-25;i<=25;i+=5)
            {
                Point p = M0Points[(25 + i) / 5];
                seriesM0.Points.AddXY(i, p.z);
            }

            chart1.Series.Add(seriesM0);
            Series seriesM1 = new Series
            {
                Name = "M1",

                Color = Color.Blue,
                ChartType = SeriesChartType.Point,

            };
            for (int i = -25; i <= 25; i += 5)
            {
                Point p = M1Points[(25 + i) / 5];
                seriesM1.Points.AddXY(i, p.z);
            }
            chart1.Series.Add(seriesM1);
            Legend legend = new Legend();
            chart1.Legends.Add(legend);
            tabControl1.SelectedTab = tabPage3;
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
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

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 图像ToolStripMenuItem_Click(object sender, EventArgs e)
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
