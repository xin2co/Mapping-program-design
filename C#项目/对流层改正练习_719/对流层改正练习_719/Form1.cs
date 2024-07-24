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

namespace 对流层改正练习_719
{
    public partial class Form1 : Form
    {
        string[] all_lines;
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
                        name = parts[0],
                        time =parts[1],
                        B = Dms2Rad(double.Parse(parts[2])) ,
                        L = double.Parse(parts[3]),
                        Lhu = Dms2Rad(double.Parse(parts[3])),
                        H = double.Parse(parts[4]),
                        E = double.Parse(parts[5]),
                        Ehu = Dms2Rad(double.Parse(parts[5]))
                    };
                    allPoints.Add(s);
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
                }
                toolStripLabel1.Text = "文件导入成功";
            }
            catch
            {
                MessageBox.Show("文件导入失败");
                return;
            }
        }
        private double Dms2Rad(double dms)
        {
            return dms * Math.PI / 180;
        }
        private double[] Calshi(Point p)
        {
            double phi = p.L;
            double[,] Avg = new double[,]
            {
            {15, 0.00058021897, 0.0014275268, 0.043472961},
            {30, 0.00056794847, 0.0015138625, 0.046729510},
            {45, 0.00058118019, 0.0014572752, 0.043908931},
            {60, 0.00059727542, 0.0015007428, 0.044626982},
            {75, 0.00061641693, 0.0017599082, 0.054736038}
            };
            phi = Math.Max(15, Math.Min(75, phi));
            double aw, bw, cw;
            int i = 0;
            while (i < Avg.GetLength(0) - 1 && phi >= Avg[i+1, 0])
            {
                i++;
            }

            aw = Avg[i, 1] + (Avg[i + 1, 1] - Avg[i, 1]) * ((p.L - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]));
            bw = Avg[i, 2] + (Avg[i + 1, 2] - Avg[i, 2]) * ((p.L - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]));
            cw = Avg[i, 3] + (Avg[i + 1, 3] - Avg[i, 3]) * ((p.L - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0]));
            
            double[] xishu = { aw, bw, cw };
            return xishu;
        }
        private double CalMwE(Point p)
        {

            double[] xishu = Calshi(p);
            double fenzi = 1 / (Math.Sin(p.Ehu) + (xishu[0] / (Math.Sin(p.Ehu) + (xishu[1] / (Math.Sin(p.Ehu) + xishu[2])))));
            double fenmu = 1 / (1 + (xishu[0] / (1 + (xishu[1] / (1 + xishu[2])))));
            double re = fenzi / fenmu;
            return re;
        }
        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Point p in allPoints)
            {
                p.MwE = CalMwE(p);
                p.MdE = CalMdE(p);
                p.ds = Calds(p);
            }
            toolStripLabel1.Text = "计算成功";
        }
        private double Calds(Point p)
        {
            p.ZHD = 2.29951 * Math.Exp(-0.000116 * p.H);
            p.ZWD = 0.1;
            double deltaS = p.ZHD * p.MdE + p.ZWD * p.MwE;
            return deltaS;
        }
        private double[] Calgan(Point p)
        {
            double phi = Math.Abs(p.L);
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
            double ad = 0;
            double bd = 0;
            double cd = 0;
            while (i < Avg.GetLength(0) - 1 && phi >= Avg[i+1, 0])
            {
                i++;
            }
            int t = Calday(p);
            if (phi <= 15)
            {
                ad = Avg[0, 1] + Avg[0, 1] * (Math.Cos(2 * Math.PI * ((t - 28) / 365.25)));
                bd = Avg[0, 2] + Avg[0, 2] * (Math.Cos(2 * Math.PI * ((t - 28) / 365.25)));
                cd = Avg[0, 3] + Avg[0, 3] * (Math.Cos(2 * Math.PI * ((t - 28) / 365.25)));
            }
            else if (phi > 15 && phi < 75)
            {
                ad = Avg[i, 1] + (Avg[i + 1, 1] - Avg[i, 1]) * ((phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0])) + (Amp[i, 1] + (Amp[i + 1, 1] - Amp[i, 1]) * ((phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0])) * Math.Cos(2 * Math.PI * (t - 28) / 365.25));
                bd = Avg[i, 2] + (Avg[i + 1, 2] - Avg[i, 2]) * ((phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0])) + (Amp[i, 2] + (Amp[i + 1, 2] - Amp[i, 2]) * ((phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0])) * Math.Cos(2 * Math.PI * (t - 28) / 365.25));
                cd = Avg[i, 3] + (Avg[i + 1, 3] - Avg[i, 3]) * ((phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0])) + (Amp[i, 3] + (Amp[i + 1, 3] - Amp[i, 3]) * ((phi - Avg[i, 0]) / (Avg[i + 1, 0] - Avg[i, 0])) * Math.Cos(2 * Math.PI * (t - 28) / 365.25));
            }
            else
            {
                ad = Avg[i, 1] + Avg[i, 1] * Math.Cos(2 * Math.PI * (t - 28) / 365.25);
                bd = Avg[i, 2] + Avg[i, 2] * Math.Cos(2 * Math.PI * (t - 28) / 365.25);
                cd = Avg[i, 3] + Avg[i, 3] * Math.Cos(2 * Math.PI * (t - 28) / 365.25);

            }
            double[] re = { ad, bd, cd };
            return re;

        }
        private double CalMdE(Point p)
        {
            double aht = 2.53e-5;
            double bht = 5.49e-3;
            double cht = 1.14e-3;
            double[] xishu = Calgan(p);
            double fenzi1 = 1 / (Math.Sin(p.Ehu) + (xishu[0] / (Math.Sin(p.Ehu) + (xishu[1] / (Math.Sin(p.Ehu) + xishu[2])))));
            double fenmu1 = 1 / (1 + (xishu[0] / (1 + (xishu[1] / (1 + xishu[2])))));
            double fenzi2 = 1 / (Math.Sin(p.Ehu) + (aht / (Math.Sin(p.Ehu) + (bht / (Math.Sin(p.Ehu) + cht)))));
            double fenmu2 = 1 / (1 + (aht / (1 + (bht / (1 + cht)))));
            double MdE = fenzi1 / fenmu1 + (1 / Math.Sin(p.Ehu) - fenzi2 / fenmu2) * p.H / 1000;
            return MdE;
        }
        private int Calday(Point p)
        {
            string date = p.time;
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(4, 2));
            int day = int.Parse(date.Substring(6, 2));
            int days = day;
            int[] daysInMonth = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if (IsLeapYear(year))
            {
                daysInMonth[1] = 29;
            }

            for (int i = 0; i < month - 1; i++)
            {
                days += daysInMonth[i];
            }
            return days;
        }
        private static bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = String.Format("测站名\t高度角\tZHD\tm_d(E)\tZWD\tm_w(E)\t延迟改正\n");
            richTextBox1.AppendText(title);

            // 输出数据行
            foreach (Point point in allPoints)
            {
                string output = String.Format("{0,-5}\t{1,6:F3}\t{2,6:F3}\t{3,6:F3}\t{4,6:F3}\t{5,6:F3}\t{6,6:F3}\n", point.name, point.E, point.ZHD, point.MdE, point.ZWD, point.MwE, point.ds);

                richTextBox1.AppendText(output);
            }


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
    }
}
