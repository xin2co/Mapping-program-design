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

namespace 第三次模拟
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        double a;
        double fdao;
        List<Point> allpoints = new List<Point>();
        string report;
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
                string[] firstlines = all_lines[0].Split(',');
                a = double.Parse(firstlines[0]);
                fdao = double.Parse(firstlines[1]);
                for (int i = 2; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point p = new Point()
                    {
                        name1 = parts[0],
                        B1 = Dms2Rad(double.Parse(parts[1])),
                        L1 = Dms2Rad(double.Parse(parts[2])),
                        name2 = parts[3],
                        B2 = Dms2Rad(double.Parse(parts[4])),
                        L2 = Dms2Rad(double.Parse(parts[5]))
                    };
                    allpoints.Add(p);
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
                    toolStripLabel1.Text = "当前状态:成功读入数据!";
                    toolStripLabel2.Text = "椭球长半轴:" + a + " 扁率倒数:" + fdao;
                }
                report += "1,椭球长半轴 a," + a + "\n";
                report += "2,扁率倒数 1/f," + fdao + "\n";
                report += "3,扁率 f," + 1 / fdao + "\n";

            }
            catch
            {
                MessageBox.Show("导入失败");
                return;
            }
        }

        private double Dms2Rad(double ddmmss)
        {
            double degrees;
            double minutes;
            double seconds;

            degrees = Math.Floor(ddmmss);
            minutes = Math.Floor((ddmmss - degrees) * 100);
            seconds = (ddmmss - degrees - minutes / 100) * 10000;

            double rad = (degrees + minutes / 60.0 + seconds / 3600.0) * (Math.PI / 180.0);

            return rad;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double b = a * (1 - 1 / fdao);
            double e12 = (a * a - b * b) / (a * a);
            double e22 = (a * a - b * b) / (b * b);
            report += "4,椭球短半轴 b," + b.ToString("F8") + "\n";
            report += "5,第一偏心率平方 e2," + e12.ToString("F8") + "\n";
            report += "6,第二偏心率平方 e'2," + e22.ToString("F8") + "\n";
            for (int i = 0; i < allpoints.Count; i++)
            {

                Point p0 = allpoints[i];
                double u1 = Math.Atan(Math.Sqrt(1 - e12) * Math.Tan(p0.B1));
                double u2 = Math.Atan(Math.Sqrt(1 - e12) * Math.Tan(p0.B2));
                double l = p0.L2 - p0.L1;
                double a1 = Math.Sin(u1) * Math.Sin(u2);
                double a2 = Math.Cos(u1) * Math.Cos(u2);
                double b1 = Math.Cos(u1) * Math.Sin(u2);
                double b2 = Math.Sin(u1) * Math.Cos(u2);
                double seita = 0;
                double temp;
                double lamuda = l;
                double p, q, A1 = 0, sinseigema, cosseigema, seigema = 0, sinA0 = 0, cosA02 = 0, segema1, alpha = 0, beta = 0, gama = 0;
                while (true)
                {
                    p = Math.Cos(u2) * Math.Sin(lamuda);
                    q = b1 - b2 * Math.Cos(lamuda);
                    A1 = Math.Atan(p / q);
                    if (p > 0 && q > 0)
                    {
                        A1 = Math.Abs(A1);
                    }
                    else if (p > 0 && q < 0)
                    {
                        A1 = Math.PI - Math.Abs(A1);
                    }
                    else if (p < 0 && q < 0)
                    {
                        A1 = Math.PI + Math.Abs(A1);
                    }
                    else
                    {
                        A1 = 2 * Math.PI - Math.Abs(A1);
                    }
                    if (A1 < 0)
                    {
                        A1 += 2 * Math.PI;
                    }
                    else if (A1 > 2 * Math.PI)
                    {
                        A1 = A1 - 2 * Math.PI;
                    }
                    sinseigema = p * Math.Sin(A1) + q * Math.Cos(A1);
                    cosseigema = a1 + a2 * Math.Cos(lamuda);
                    seigema = Math.Atan2(sinseigema, cosseigema);
                    if (cosseigema > 0)
                    {
                        seigema = Math.Abs(seigema);
                    }
                    else
                    {
                        seigema = Math.PI - Math.Abs(seigema);
                    }
                    sinA0 = Math.Cos(u1) * Math.Sin(A1);
                    cosA02 = 1 - sinA0 * sinA0;
                    segema1 = Math.Atan(Math.Tan(u1) / Math.Cos(A1));
                    alpha = (e12 / 2 + e12 * e12 / 8 + e12 * e12 * e12 / 16) - (e12 * e12 / 16 + e12 * e12 * e12 / 16) * cosA02 + (3 * e12 * e12 * e12 / 128) * cosA02 * cosA02;
                    beta = (e12 * e12 / 16 + e12 * e12 * e12 / 16) * cosA02 - (e12 * e12 * e12 / 32) * cosA02 * cosA02;
                    gama = (e12 * e12 * e12 / 256) * cosA02 * cosA02;

                    temp = seita;
                    seita = (alpha * seigema + beta * Math.Cos(2 * segema1 + seigema) * Math.Sin(seigema) + gama * Math.Sin(2 * seigema) * Math.Cos(4 * segema1 + 2 * seigema)) * sinA0;
                    lamuda = l + seita;
                    if (Math.Abs(seita - temp) < 1e-10)
                    {
                        break;
                    }

                }

                double seigema1 = Math.Atan(Math.Tan(u1) / Math.Cos(A1));
                double k2 = e22 * cosA02;
                double A = (1 - k2 / 4 + 7 * k2 * k2 / 64 - 15 * k2 * k2 * k2 / 256) / b;
                double B = (k2 / 4 - k2 * k2 / 8 + 37 * k2 * k2 * k2 / 512);
                double C = (k2 * k2 / 128 - k2 * k2 * k2 / 128);
                double xs = C * Math.Sin(2 * seigema) * Math.Cos(4 * seigema1 + 2 * seigema);
                double S = (seigema - B * Math.Sin(seigema) * Math.Cos(2 * seigema1 + seigema) - xs) / A;
                if (i == 0)
                {
                    report += "7,第 1 条大地线 u1," + u1.ToString("F8") + "\n";
                    report += "8,第 1 条大地线 u2," + u2.ToString("F8") + "\n";
                    report += "9,第 1 条大地线经差 l（弧度)," + l.ToString("F8") + "\n";
                    report += "10,第 1 条大地线 a1," + a1.ToString("F8") + "\n";
                    report += "11,第 1 条大地线 a2," + a2.ToString("F8") + "\n";
                    report += "12,第 1 条大地线 b1," + b1.ToString("F8") + "\n";
                    report += "13,第 1 条大地线 b2," + b2.ToString("F8") + "\n";
                    report += "14,第 1 条大地线系数α," + alpha.ToString("F8") + "\n";
                    report += "15,第 1 条大地线系数β," + beta.ToString("F8") + "\n";
                    report += "16,第 1 条大地线系数γ," + gama.ToString("F8") + "\n";
                    report += "17,第 1 条大地线 A1（弧度）," + A1.ToString("F8") + "\n";
                    report += "18,第 1 条大地线λ," + lamuda.ToString("F8") + "\n";
                    report += "19,第 1 条大地线σ ," + seigema.ToString("F8") + "\n";
                    report += "20,第 1 条大地线 sinA0," + sinA0.ToString("F8") + "\n";
                    report += "21,第 1 条大地线系数 A," + A.ToString("F8") + "\n";
                    report += "22,第 1 条大地线系数 B," + B.ToString("F8") + "\n";
                    report += "23,第 1 条大地线系数 C," + C.ToString("F8") + "\n";
                    report += "24,第 1 条大地线σ1," + seigema1.ToString("F8") + "\n";
                }

                report += (25 + i) + ",第 " + (i + 1) + " 条大地线长 S," + S.ToString("F3") + "\n";




            }
            richTextBox1.Text = report;
            tabControl1.SelectedTab = tabPage2;
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sa = new SaveFileDialog();
            sa.Filter ="文本文件(*.txt) | *.txt | 所有文件(*.*) | *.*";


            sa.FileName = "result";
            if (sa.ShowDialog() == DialogResult.OK)
            {
                string filepath = sa.FileName;
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    sw.Write(richTextBox1.Text);
                }
            }

        }
    }
}
