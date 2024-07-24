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

namespace 电离层改正练习721
{

    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> allPoints = new List<Point>();
        Time time = new Time();
        double pi = Math.PI;
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
                string[] firstline = all_lines[0].Split().Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                time.year = Convert.ToInt32(firstline[1]);
                time.month = Convert.ToInt32(firstline[2]);
                time.day = Convert.ToInt32(firstline[3]);
                time.hour = Convert.ToInt32(firstline[4]);
                time.min = Convert.ToInt32(firstline[5]);
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
            double XP = -2225669.7744;
            double YP = 4998936.1598;
            double ZP = 3265908.9678;
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
                {cosBp*cosLp,cosBp*sinLp,sinBp },
};
            double dx, dy, dz, X, Y, Z;
            foreach (Point s in allPoints)
            {
                dx = s.x - XP;
                dy = s.y - YP;
                dz = s.z - ZP;
                X = H[0, 0] * dx + H[0, 1] * dy + H[0, 2] * dz;
                Y = H[1, 0] * dx + H[1, 1] * dy + H[1, 2] * dz;
                Z = H[2, 0] * dx + H[2, 1] * dy + H[2, 2] * dz;
                s.A = Math.Atan2(Y, X);
                if (s.A < 0)
                {
                    s.A += 2 * pi;
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
                s.phiIP = Bp / pi + phi * Math.Cos(s.A);
                if (s.phiIP > 0.416) s.phiIP = 0.416;
                if (s.phiIP < -0.416) s.phiIP = -0.416;
                s.lamudaIP = Lp / pi + phi * Math.Sin(s.A ) / Math.Cos(s.phiIP * pi);
                s.phim = s.phiIP + 0.064 * Math.Cos((s.lamudaIP - 1.617) * pi);
                double c = 299792458;
                double t = 43200 * s.lamudaIP + time.hour * 3600 + time.min * 60 + time.sec;
                double[] alpha = { 0.1397e-7, -0.7451e-8, -0.5960e-7, 0.1192e-6 };
                double[] beta = { 0.127e6, -0.1966e6, 0.6554e5, 0.2621e6 };
                double A2 = alpha[0] + alpha[1] * s.phim + alpha[2] * s.phim * s.phim + alpha[3] * s.phim * s.phim * s.phim;
                double A4 = beta[0] + beta[1] * s.phim + beta[2] * s.phim * s.phim + beta[3] * s.phim * s.phim * s.phim;
                double F = 1 + 16 * (0.53 - Ecp) * (0.53 - Ecp) * (0.53 - Ecp);
                double panduan = Math.Abs(2 * pi * (t - 50400) / A4);
                if (panduan < 1.57)
                {
                    s.Tion = F * (5e-9 + A2 * Math.Cos(2 * pi * (t - 50400) / A4));
                }
                else
                {
                    s.Tion = F * 5e-9;
                }
                s.Dion = s.Tion * c;
            }
            toolStripLabel1.Text = "计算成功";

        }
        private double Rad2Deg(double d)
        {
            return d * 180 / pi;
        }
        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string reports = null;
            reports += "卫星名称".PadRight(8) + "高度角 (度)".PadRight(10) + "方位角 (度)".PadRight(15) + "电离层延迟 (秒)".PadRight(20) + "\n";
            foreach (Point satellite in allPoints)
            {
                reports += satellite.name.PadRight(5) +
                           Math.Round(Rad2Deg(satellite.E), 8).ToString().PadLeft(15) +
                           Math.Round(Rad2Deg(satellite.A), 8).ToString().PadLeft(15) +
                           Math.Round(satellite.Dion, 8).ToString().PadLeft(20) +
                           "\n";
            }
            richTextBox1.Text = reports;
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
    }
}

