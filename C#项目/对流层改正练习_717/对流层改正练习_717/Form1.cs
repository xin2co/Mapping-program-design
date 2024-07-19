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

namespace 对流层改正练习_717
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> allpoints = new List<Point>();

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
                for(int i = 1; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point p = new Point()
                    {
                        name = parts[0],
                        time = parts[1],
                        B = double.Parse(parts[2]),
                        L = double.Parse(parts[3]),
                        h = double.Parse(parts[4]),
                        E = double.Parse(parts[5]),
                        Ehu = Dms2Rad(double.Parse(parts[5]))
                    };
                    allpoints.Add(p);
                    dataGridView1.Rows.Add(p.name, p.time, p.B, p.L, p.h, p.E);
                }
                toolStripLabel1.Text = "当前状态:导入成功";
            }
            catch
            {
                MessageBox.Show("导入失败");
                return;
            }
        }

        private double Dms2Rad(double d)
        {
            return d * Math.PI / 180;
        }
        public double CalDeltaS(Point p)
        {
            p.ZWD = 0.1;
            double H = p.h;
            p.ZHD = 2.29951 * Math.Exp(-0.000116 * H);
            double deltaS = p.ZHD * p.MdE + p.ZWD * p.MwE;
            return deltaS;
        }
        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(Point p in allpoints)
            {
                p.MwE = CalMwE(p);
                p.MdE = CalMdE(p);
                p.ds = CalDeltaS(p);
            }
            toolStripLabel1.Text = "当前状态:计算成功";

        }

        private double CalMwE(Point p)
        {
            double[] re = Calshixishu(p);
            double aw = re[0];
            double bw = re[1];
            double cw = re[2];
            double fenzi = 1 / (Math.Sin(p.Ehu) + (aw / (Math.Sin(p.Ehu) + (bw / (Math.Sin(p.Ehu) + cw)))));
            double fenmu = 1 / (1 + (aw / (1 + (bw / (1 + cw)))));
            return fenzi / fenmu;
        }

        private double[] Calshixishu(Point p)
        {
            double phi = p.L;
            double[,] shibiao = new double[,]
            {
            {15, 0.00058021897, 0.0014275268, 0.043472961},
            {30, 0.00056794847, 0.0015138625, 0.046729510},
            {45, 0.00058118019, 0.0014572752, 0.043908931},
            {60, 0.00059727542, 0.0015007428, 0.044626982},
            {75, 0.00061641693, 0.0017599082, 0.054736038}
            };
            double aw, bw, cw;
            phi = Math.Max(15, Math.Min(75, phi));
            int i = 0;
            while (i < shibiao.GetLength(0) - 1 && phi >= shibiao[i + 1, 0])
            {
                i++;
            }
            double duiyingdushu = shibiao[i, 0];
            if (phi == duiyingdushu)
            {
                aw = shibiao[i, 1];
                bw = shibiao[i, 2];
                cw = shibiao[i, 3];
            }
            else
            {
                double awi = shibiao[i, 1];
                double bwi = shibiao[i, 2];
                double cwi = shibiao[i, 3];
                double awi1 = shibiao[i + 1, 1];
                double bwi1 = shibiao[i + 1, 2];
                double cwi1 = shibiao[i + 1, 3];
                aw = awi + (awi1 - awi) * ((phi - shibiao[i, 0]) / (shibiao[i + 1, 0] - shibiao[i, 0]));
                bw = bwi + (bwi1 - bwi) * ((phi - shibiao[i, 0]) / (shibiao[i + 1, 0] - shibiao[i, 0]));
                cw = cwi + (cwi1 - cwi) * ((phi - shibiao[i, 0]) / (shibiao[i + 1, 0] - shibiao[i, 0]));
                
            }
            double[] re= { aw, bw, cw };
            return  re;
        }
        public double CalMdE(Point p)
        {
            double aht = 2.53e-5;
            double bht = 5.49e-3;
            double cht = 1.14e-3;
            double[] re = Calganxishu(p);
            double ad = re[0];
            double bd = re[1];
            double cd = re[2];
            double E = p.Ehu;
            double H = p.h;
            double yifenmu = 1 / (1 + ad / (1 + bd / (1 + cd)));
            double yifenzi = 1 / (Math.Sin(E) + ad / (Math.Sin(E) + bd / (Math.Sin(E) + cd)));
            double firstTerm = yifenzi / yifenmu;

            double erfenmu = 1 / (1 + aht / (1 + bht / (1 + cht)));
            double erfenzi = 1 / (Math.Sin(E) + aht / (Math.Sin(E) + bht / (Math.Sin(E) + cht)));
            double secondTerm = (1 / Math.Sin(E) - erfenzi / erfenmu) * (H / 1000);

            return firstTerm + secondTerm;
        }
        private double[] Calganxishu(Point p)
        {
            double phi = p.L;
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
            while (i < Avg.GetLength(0) - 1 && phi >= Avg[i + 1, 0])
            {
                i++;
            }
            double duiyingdushu = Avg[i, 0];
            double ad = 0;
            double bd = 0;
            double cd = 0;
            double yearFraction = (Calday(p) - 28) / 365.25;
            if (phi < 15)
            {
                ad = Avg[0, 1] + Avg[0, 1] * Math.Cos(2 * Math.PI * yearFraction);
                bd = Avg[0, 2] + Avg[0, 2] * Math.Cos(2 * Math.PI * yearFraction);
                cd = Avg[0, 3] + Avg[0, 3] * Math.Cos(2 * Math.PI * yearFraction);
            }
            else if (phi >= 15 && phi <= 75)
            {
                if (phi == duiyingdushu)
                {
                    ad = Avg[i, 1] + Amp[i, 1] * Math.Cos(2 * Math.PI * yearFraction);
                    bd = Avg[i, 2] + Amp[i, 2] * Math.Cos(2 * Math.PI * yearFraction);
                    cd = Avg[i, 3] + Amp[i, 3] * Math.Cos(2 * Math.PI * yearFraction);
                }
                else
                {
                    double latitude_i1 = Avg[i + 1, 0];
                    double a_avg_i = Avg[i, 1];
                    double a_avg_i1 = Avg[i + 1, 1];
                    double b_avg_i = Avg[i, 2];
                    double b_avg_i1 = Avg[i + 1, 2];
                    double c_avg_i = Avg[i, 3];
                    double c_avg_i1 = Avg[i + 1, 3];

                    double a_amp_i = Amp[i, 1];
                    double a_amp_i1 = Amp[i + 1, 1];
                    double b_amp_i = Amp[i, 2];
                    double b_amp_i1 = Amp[i + 1, 2];
                    double c_amp_i = Amp[i, 3];
                    double c_amp_i1 = Amp[i + 1, 3];

                    ad = a_avg_i + (a_avg_i1 - a_avg_i) * (phi - duiyingdushu) / (latitude_i1 - duiyingdushu) + (a_amp_i + (a_amp_i1 - a_amp_i) * (phi - duiyingdushu) / (latitude_i1 - duiyingdushu)) * Math.Cos(2 * Math.PI * yearFraction);
                    bd = b_avg_i + (b_avg_i1 - b_avg_i) * (phi - duiyingdushu) / (latitude_i1 - duiyingdushu) +
                      (b_amp_i + (b_amp_i1 - b_amp_i) * (phi - duiyingdushu) / (latitude_i1 - duiyingdushu)) * Math.Cos(2 * Math.PI * yearFraction);
                    cd = c_avg_i + (c_avg_i1 - c_avg_i) * (phi - duiyingdushu) / (latitude_i1 - duiyingdushu) +
                          (c_amp_i + (c_amp_i1 - c_amp_i) * (phi - duiyingdushu) / (latitude_i1 - duiyingdushu)) * Math.Cos(2 * Math.PI * yearFraction);
                }
            }
            else if (phi > 75)
            {
                ad = Avg[4, 1] + Avg[4, 1] * Math.Cos(2 * Math.PI * yearFraction);
                bd = Avg[4, 2] + Avg[4, 2] * Math.Cos(2 * Math.PI * yearFraction);
                cd = Avg[4, 3] + Avg[4, 3] * Math.Cos(2 * Math.PI * yearFraction);
            }
            double[] re = { ad, bd, cd };
            return re;
        }


        private int Calday(Point p)
        {
            string date = p.time;
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(4, 2));
            int day = int.Parse(date.Substring(6, 2));

            int[] daysInMonth = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            if (IsLeapYear(year))
            {
                daysInMonth[1] = 29;
            }

            int dayOfYear = day;
            for (int i = 0; i < month - 1; i++)
            {
                dayOfYear += daysInMonth[i];
            }
            
            return dayOfYear;
        }

        private bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = String.Format("测站名\t高度角\t  ZHD    \tm_d(E)\tZWD\t   m_w(E)\t   延迟改正\n");
            richTextBox1.AppendText(title);

            
            foreach (Point point in allpoints)
            {
                string output = String.Format("{0,-10}\t{1,6:F3}\t{2,6:F3}\t{3,6:F3}\t{4,6:F3}\t{5,6:F3}\t{6,6:F3}\n", point.name, point.E, point.ZHD, point.MdE, point.ZWD, point.MwE, point.ds);

                richTextBox1.AppendText(output);
            }


            tabControl1.SelectedTab = tabPage2;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sa = new SaveFileDialog();
            sa.FileName = "result.txt";
            sa.Filter = "Text Files(*. txt)|(*. txt)";
            if (sa.ShowDialog() == DialogResult.OK)
            {
                string filePath = sa.FileName;
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(richTextBox1.Text);
                }
            }
        }
    }
}
