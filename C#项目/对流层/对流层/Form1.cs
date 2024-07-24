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

namespace 对流层
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        double pi = Math.PI;
        List<Point> allPoints = new List<Point>();
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
                for (int i = 1; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point s = new Point()
                    {
                        id = parts[0],
                        time = parts[1],
                        L = double.Parse(parts[2]),
                        Lhu = Rad(double.Parse(parts[2])),
                        B = double.Parse(parts[3]),
                        Bhu = Rad(double.Parse(parts[3])),
                        H = double.Parse(parts[4]),
                        E = double.Parse(parts[5]),
                        Ehu = Rad(double.Parse(parts[5])),
                    };
                    allPoints.Add(s);
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
                }
                toolStripStatusLabel1.Text = "文件导入成功";
            }
            catch
            {
                MessageBox.Show("文件导入失败");
                return;
            }

        }
        private double Rad(double d)
        {
            return d * pi / 180;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Point p in allPoints)
            {
                CalMwE(p);
                CalMdE(p);
                Calds(p);
            }
            toolStripStatusLabel1.Text = "计算成功";
        }
        private void Calds(Point p)
        {
            p.ZHD = 2.29951 * Math.Exp(-0.000116 * p.H);
            p.ZWD = 0.1;
            p.ds = p.ZHD * p.MdE + p.ZWD * p.MwE;
        }

        private void CalMwE(Point p)
        {
            double[] re = Calshi(p);
            double aw = re[0];
            double bw = re[1];
            double cw = re[2];
            double sinE = Math.Sin(p.Ehu);
            double fenzi = 1 / (sinE + aw / (sinE + bw / (sinE + cw)));
            double fenmu = 1 / (1 + aw / (1 + bw / (1 + cw)));
            p.MwE = fenzi / fenmu;

        }

        private double[] Calshi(Point p)
        {
            double phi = Math.Abs(p.B);
            double[,] Avg = new double[,]
{
            {15, 0.00058021897, 0.0014275268, 0.043472961},
            {30, 0.00056794847, 0.0015138625, 0.046729510},
            {45, 0.00058118019, 0.0014572752, 0.043908931},
            {60, 0.00059727542, 0.0015007428, 0.044626982},
            {75, 0.00061641693, 0.0017599082, 0.054736038}
};
            double aw, bw, cw;
            phi = Math.Min(Math.Max(15, phi), 75);
            int i = 0;
            while (i < Avg.GetLength(0) - 1 && phi > Avg[i + 1, 0])
            {
                i++;
            }
            if (phi == Avg[i, 0])
            {
                aw = Avg[i, 1];
                bw = Avg[i, 2];
                cw = Avg[i, 3];
            }
            else
            {
                aw = Avg[i, 1] + (Avg[i + 1, 1] - Avg[i, 1]) * (phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]);
                bw = Avg[i, 2] + (Avg[i + 1, 2] - Avg[i, 2]) * (phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]);
                cw = Avg[i, 3] + (Avg[i + 1, 3] - Avg[i, 3]) * (phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]);
            }

            double[] re = { aw, bw, cw };
            return re;
        }

        private void CalMdE(Point p)
        {
            double[] re = Calgan(p);
            double ad = re[0];
            double bd = re[1];
            double cd = re[2];
            double aht = 2.53e-5;
            double bht = 5.49e-3;
            double cht = 1.14e-3;
            double sinE = Math.Sin(p.Ehu);
            double fenzi1 = 1 / (sinE + ad / (sinE + bd / (sinE + cd)));
            double fenmu1 = 1 / (1 + ad / (1 + bd / (1 + cd)));
            double fenzi2 = 1 / (sinE + aht / (sinE + bht / (sinE + cht)));
            double fenmu2 = 1 / (1 + aht / (1 + bht / (1 + cht)));
            p.MdE = fenzi1 / fenmu1 + (1 / sinE - (fenzi2 / fenmu2)) * p.H / 1000;
        }

        private double[] Calgan(Point p)
        {
            double phi = Math.Abs(p.B);
            double[,] Avg = new double[,]
{
        {15, 0.0012769934, 0.0029153695, 0.062610505},
        {30, 0.0012683230, 0.0029152299, 0.062837393},
        {45, 0.0012465397, 0.0029288445, 0.063721774},
        {60, 0.0012196049, 0.0029022565, 0.063824265},
        {75, 0.0012045996, 0.0029024912, 0.064258455}
};

            double[,] Amp = new double[,]
           {
        {15, 0.0, 0.0, 0.0},
        {30, 0.000012709626, 0.000021414979, 0.000090128400},
        {45, 0.000026523662, 0.000030160779, 0.000043497037},
        {60, 0.000034000452, 0.000072562722, 0.00084795348},
        {75, 0.000041202191, 0.00011723375, 0.0017037206}
           };
            int i = 0;
            while (i < Avg.GetLength(0) - 1 && phi > Avg[i + 1, 0])
            {
                i++;
            }
            double ad = 0;
            double bd = 0;
            double cd = 0;
            double t = Calday(p);
            double cos = Math.Cos(2 * pi * (t - 28) / 365.25);
            if (phi < 15)
            {
                ad = Avg[0, 1] + Avg[0, 1] * cos;
                bd = Avg[0, 2] + Avg[0, 2] * cos;
                cd = Avg[0, 3] + Avg[0, 3] * cos;
            }
            else if (phi >= 15 && phi <= 75)
            {
                ad = Avg[i, 1] + (Avg[i + 1, 1] - Avg[i, 1]) * (phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]);
                bd = Avg[i, 2] + (Avg[i + 1, 2] - Avg[i, 2]) * (phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]);
                cd = Avg[i, 3] + (Avg[i + 1, 3] - Avg[i, 3]) * (phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]);
                ad += Amp[i, 1] + (Amp[i + 1, 1] - Amp[i, 1]) * (phi - Amp[i, 0]) / (Amp[i + 1, 0] - Amp[i, 0]) * cos;
                bd += Amp[i, 2] + (Amp[i + 1, 2] - Amp[i, 2]) * (phi - Amp[i, 0]) / (Amp[i + 1, 0] - Amp[i, 0]) * cos;
                cd += Amp[i, 3] + (Amp[i + 1, 3] - Amp[i, 3]) * (phi - Amp[i, 0]) / (Amp[i + 1, 0] - Amp[i, 0]) * cos;
            }
            else
            {
                ad = Avg[4, 1] + Avg[4, 1] * cos;
                bd = Avg[4, 2] + Avg[4, 2] * cos;
                cd = Avg[4, 3] + Avg[4, 3] * cos;
            }
            double[] re = { ad, bd, cd };
            return re;
        }

        private double Calday(Point p)
        {
            double year = double.Parse(p.time.Substring(0, 4));
            double month = double.Parse(p.time.Substring(4, 2));
            double day = double.Parse(p.time.Substring(6, 2));
            double days = day;
            int[] months = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
            {
                months[1] = 29;
            }
            for (int i = 0; i < month-1; i++)
            {
                days += months[i];
            }
            return days;

        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = String.Format("测站名\t高度角\t  ZHD    \tm_d(E)\tZWD\t   m_w(E)\t   延迟改正\n");
            richTextBox1.AppendText(title);

            // 输出数据行
            foreach (Point point in allPoints)
            {
                string output = String.Format("{0,-10}\t{1,6:F3}\t{2,6:F3}\t{3,6:F3}\t{4,6:F3}\t{5,6:F3}\t{6,6:F3}\n", point.id, point.E, point.ZHD, point.MdE, point.ZWD, point.MwE, point.ds);

                richTextBox1.AppendText(output);
            }
            tabControl1.SelectedTab = tabPage2;
        }
    }
}
