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

namespace 电离层改正722
{
    public partial class Form1 : Form
    {
        double pi = Math.PI;
        string[] all_lines;
        List<Point> allPoints = new List<Point>();
        Time time = new Time();
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
                string[] firstline = all_lines[0].Split().Where(s =>! string.IsNullOrWhiteSpace(s)).ToArray();

                time.year =int.Parse(firstline[1]);
                time.month = int.Parse(firstline[2]);
                time.day = int.Parse(firstline[3]);
                time.hour = int.Parse(firstline[4]);
                time.min = int.Parse(firstline[5]);
                time.sec = double.Parse(firstline[6]);


                for (int i = 1; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split().Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
                    Point s = new Point()
                    {
                        name = parts[0],
                        x = double.Parse(parts[1]) * 1000,
                        y = double.Parse(parts[2]) * 1000,
                        z = double.Parse(parts[3]) * 1000,
                    };
                    allPoints.Add(s);
                    dataGridView1.Rows.Add(s.name, s.x, s.y, s.z);
                }
                toolStripLabel1.Text = "文件导入成功";
            }
            catch(Exception ex)
            {
                MessageBox.Show("文件导入失败"+ex);
                return;
            }
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double Xp = -2225669.7744, Yp = 4998936.1598, Zp = 3265908.9678;
            double Bp = 30 * pi / 180;
            double Lp = 114 * pi / 180;
            double sinBp = Math.Sin(Bp);
            double cosBp = Math.Cos(Bp);
            double sinLp = Math.Sin(Lp);
            double cosLp = Math.Cos(Lp);
            double[,] H = new double[,]
            {
                {-sinBp*cosLp,-sinBp*sinLp,cosBp },
                {-sinLp,cosLp,0 },
                {cosBp*cosLp,cosBp*sinLp,sinBp }
            };
            foreach(Point s in allPoints)
            {
                double dx = s.x - Xp;
                double dy = s.y - Yp;
                double dz = s.z - Zp;
                double X = H[0, 0] * dx + H[0, 1] * dy + H[0, 2] * dz;
                double Y = H[1, 0] * dx + H[1, 1] * dy + H[1, 2] * dz;
                double Z = H[2, 0] * dx + H[2, 1] * dy + H[2, 2] * dz;
                s.A = Math.Atan2(Y, X);
                if (s.A < 0)
                {
                    s.A += 2 * Math.PI;
                }
                double re = Z / Math.Sqrt(X * X + Y * Y);
                s.E = Math.Atan(re);
                if (s.E < 0)
                {
                    s.Dion = 0;
                    continue;
                }
                double Ecp = s.E / pi;
                double phi = 0.0137 / (Ecp + 0.11) - 0.022;
                s.phiIp = Bp / pi + phi * Math.Cos(s.A);
                if (s.phiIp > 0.416) s.phiIp = 0.416;
                if (s.phiIp < -0.416) s.phiIp = -0.416;
                s.lamudaIp = Lp / pi + phi * Math.Sin(s.A)/Math.Cos(s.phiIp*pi);
                s.phim = s.phiIp + 0.064 * Math.Cos((s.lamudaIp - 1.617) * pi);
                double[] alpha = { 0.1397e-7, -0.7451e-8, -0.5960e-7, 0.1192e-6 };
                double[] beta = { 0.127e6, -0.1966e6, 0.6554e5, 0.2621e6 };
                double c = 299792458;
                double t = 43200 * s.lamudaIp + time.hour * 3600 + time.min * 60 + time.sec;
                double A2 = alpha[0] + alpha[1] * s.phim + alpha[2] * s.phim * s.phim + alpha[3] * s.phim * s.phim * s.phim;
                double A4 = beta[0] + beta[1] * s.phim + beta[2] * s.phim * s.phim + beta[3] * s.phim * s.phim * s.phim;
                double panduan = 2 * pi * (t - 50400) / A4;

                double F = 1 + 16 * (0.53 - Ecp) * (0.53 - Ecp) * (0.53 - Ecp);
                if (Math.Abs(panduan)<1.57)
                {
                    s.Tion = F * (5e-9 + A2 * Math.Cos(panduan));
                }
                else
                {
                    s.Tion = F * 5e-9;
                }
                s.Dion = s.Tion * c;
            }
            toolStripLabel1.Text = "计算成功";
        }

        private double Rad2Deg(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string reports = null;
            reports += "卫星名称".PadRight(8) + "高度角 (度)".PadRight(10) + "方位角 (度)".PadRight(15) + "电离层延迟 (秒)".PadRight(20) + "\n";
            foreach (Point satellite in allPoints)
            {
                reports += satellite.name.PadRight(5) +
                           Math.Round(Rad2Deg(satellite.E), 3).ToString().PadLeft(15) +
                           Math.Round(Rad2Deg(satellite.A), 3).ToString().PadLeft(15) +
                           Math.Round(satellite.Dion, 4).ToString().PadLeft(20) +
                           "\n";
            }

            richTextBox1.Text = reports;
            tabControl1.SelectedTab = tabPage2;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
