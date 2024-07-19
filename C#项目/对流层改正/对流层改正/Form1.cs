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

namespace 对流层改正
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
                for (int i = 1; i < all_lines.Length; i++)
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
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
                }
                toolStripLabel1.Text = "导入成功";
            }
            catch
            {
                MessageBox.Show("导入失败");
                return;
            }
        }

        private double Dms2Rad(double dms)
        {
            return dms * Math.PI / 180;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            foreach (Point p in allpoints)
            {
                p.Mw_E = CalMw(p);
                p.Md_E = CalMd(p);
                p.DeltaS = CalDeltaS(p);
            }
            MessageBox.Show("计算成功");
            toolStripLabel1.Text = "计算成功";
        }

        //deltas
        public double CalDeltaS(Point p)
        {
            p.ZWD = 0.1;
            double H = p.h;
            p.ZHD = 2.29951 * Math.Exp(-0.000116 * H);
            double deltaS = p.ZHD * p.Md_E + p.ZWD * p.Mw_E;
            return deltaS;
        }

        //Mw_E
        private double CalMw(Point p)
        {
            // 获取对应的系数
            double[] xishu = Calshixishu(p);
            double a_w = xishu[0];
            double b_w = xishu[1];
            double c_w = xishu[2];
            // 计算分子
            double fenzi = 1 / (Math.Sin(p.Ehu) + (a_w / (Math.Sin(p.Ehu) + (b_w / (Math.Sin(p.Ehu) + c_w)))));

            // 计算分母
            double fenmu = 1 / (1 + a_w / (1 + b_w / (1 + c_w)));

            // 计算结果
            double m_w = fenzi / fenmu;

            return m_w;
        }

        //湿系数
        private double[] Calshixishu(Point p)
        {
            double phi = p.L;
            double[,] Coefficients = new double[,]
{
            {15, 0.00058021897, 0.0014275268, 0.043472961},
            {30, 0.00056794847, 0.0015138625, 0.046729510},
            {45, 0.00058118019, 0.0014572752, 0.043908931},
            {60, 0.00059727542, 0.0015007428, 0.044626982},
            {75, 0.00061641693, 0.0017599082, 0.054736038}
};
            phi = Math.Max(15, Math.Min(75, phi));

            int i = 0;
            while (i < Coefficients.GetLength(0) - 1 && phi >= Coefficients[i + 1, 0])
            {
                i++;
            }
            double a_w;
            double b_w;
            double c_w;
            double latitude_i = Coefficients[i, 0];
            if (phi == latitude_i)
            {
                a_w = Coefficients[i, 1];
                b_w = Coefficients[i, 2];
                c_w = Coefficients[i, 3];
            }
            else
            {
                double latitude_i1 = Coefficients[i + 1, 0];
                double a_w_avg_i = Coefficients[i, 1];
                double a_w_avg_i1 = Coefficients[i + 1, 1];
                double b_w_avg_i = Coefficients[i, 2];
                double b_w_avg_i1 = Coefficients[i + 1, 2];
                double c_w_avg_i = Coefficients[i, 3];
                double c_w_avg_i1 = Coefficients[i + 1, 3];

                a_w = a_w_avg_i + (a_w_avg_i1 - a_w_avg_i) * (phi - latitude_i) / (latitude_i1 - latitude_i);
                b_w = b_w_avg_i + (b_w_avg_i1 - b_w_avg_i) * (phi - latitude_i) / (latitude_i1 - latitude_i);
                c_w = c_w_avg_i + (c_w_avg_i1 - c_w_avg_i) * (phi - latitude_i) / (latitude_i1 - latitude_i);
            }

            double[] re = { a_w, b_w, c_w };
            return re;
        }

        //干系数
        public double[] Calganxishu(Point p)
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

            int dayOfYear = GetDayOfYear(p.time);
            double t0 = 28;
            double yearFraction = (dayOfYear - t0) / 365.25;

            int i = 0;
            while (i < Avg.GetLength(0) - 1 && phi >= Avg[i + 1, 0])
            {
                i++;
            }

            double a_d = 0;
            double b_d = 0;
            double c_d = 0;
            double latitude_i = Avg[i, 0];
            if (phi < 15)
            {
                a_d = Avg[0, 1] + Avg[0, 1] * Math.Cos(2 * Math.PI * yearFraction);
                b_d = Avg[0, 2] + Avg[0, 2] * Math.Cos(2 * Math.PI * yearFraction);
                c_d = Avg[0, 3] + Avg[0, 3] * Math.Cos(2 * Math.PI * yearFraction);
            }
            else if (phi > 15 && phi < 75)
            {
                if (phi == latitude_i)
                {
                    a_d = Avg[i, 1] + Amp[i, 1] * Math.Cos(2 * Math.PI * yearFraction);
                    b_d = Avg[i, 2] + Amp[i, 2] * Math.Cos(2 * Math.PI * yearFraction);
                    c_d = Avg[i, 3] + Amp[i, 3] * Math.Cos(2 * Math.PI * yearFraction);
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

                    a_d = a_avg_i + (a_avg_i1 - a_avg_i) * (phi - latitude_i) / (latitude_i1 - latitude_i) + (a_amp_i + (a_amp_i1 - a_amp_i) * (phi - latitude_i) / (latitude_i1 - latitude_i)) * Math.Cos(2 * Math.PI * yearFraction);
                    b_d = b_avg_i + (b_avg_i1 - b_avg_i) * (phi - latitude_i) / (latitude_i1 - latitude_i) +
                      (b_amp_i + (b_amp_i1 - b_amp_i) * (phi - latitude_i) / (latitude_i1 - latitude_i)) * Math.Cos(2 * Math.PI * yearFraction);
                    c_d = c_avg_i + (c_avg_i1 - c_avg_i) * (phi - latitude_i) / (latitude_i1 - latitude_i) +
                          (c_amp_i + (c_amp_i1 - c_amp_i) * (phi - latitude_i) / (latitude_i1 - latitude_i)) * Math.Cos(2 * Math.PI * yearFraction);
                }
            }
            else if (phi > 75)
            {
                a_d = Avg[4, 1] + Avg[4, 1] * Math.Cos(2 * Math.PI * yearFraction);
                b_d = Avg[4, 2] + Avg[4, 2] * Math.Cos(2 * Math.PI * yearFraction);
                c_d = Avg[4, 3] + Avg[4, 3] * Math.Cos(2 * Math.PI * yearFraction);
            }

            double[] re = { a_d, b_d, c_d };
            return re;
        }

        //Md_E
        public double CalMd(Point p)
        {
            double a_ht = 2.53e-5;
            double b_ht = 5.49e-3;
            double c_ht = 1.14e-3;
            double[] re = Calganxishu(p);
            double a_d = re[0];
            double b_d = re[1];
            double c_d = re[2];
            double E = p.Ehu;
            double H = p.h;
            double yifenmu = 1 / (1 + a_d / (1 + b_d / (1 + c_d)));
            double yifenzi = 1 / (Math.Sin(E) + a_d / (Math.Sin(E) + b_d / (Math.Sin(E) + c_d)));
            double firstTerm = yifenzi / yifenmu;

            double erfenmu = 1 / (1 + a_ht / (1 + b_ht / (1 + c_ht)));
            double erfenzi = 1 / (Math.Sin(E) + a_ht / (Math.Sin(E) + b_ht / (Math.Sin(E) + c_ht)));
            double secondTerm = (1 / Math.Sin(E) - erfenzi / erfenmu) * (H / 1000);

            return firstTerm + secondTerm;
        }

        //年积日
        public static int GetDayOfYear(string date)
        {

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

        //润否
        private static bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 假设你的窗体上有一个名为richTextBox1的RichTextBox控件
            string title = String.Format("测站名\t高度角\t  ZHD    \tm_d(E)\tZWD\t   m_w(E)\t   延迟改正\n");
            richTextBox1.AppendText(title);

            // 输出数据行
            foreach (Point point in allpoints)
            {
                string output = String.Format("{0,-10}\t{1,6:F3}\t{2,6:F3}\t{3,6:F3}\t{4,6:F3}\t{5,6:F3}\t{6,6:F3}\n",point.name, point.E, point.ZHD, point.Md_E, point.ZWD, point.Mw_E, point.DeltaS);

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
