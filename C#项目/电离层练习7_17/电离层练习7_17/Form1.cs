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

namespace 电离层练习7_17
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        Time time = new Time();
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
                string[] firstline = all_lines[0].Split().Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                time.years = Convert.ToInt32(firstline[1]);
                time.mou = Convert.ToInt32(firstline[2]);
                time.day = Convert.ToInt32(firstline[3]);
                time.hour = Convert.ToInt32(firstline[4]);
                time.min = Convert.ToInt32(firstline[5]);
                time.sec = double.Parse(firstline[6]);

                for(int i = 1; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split().Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    Point p = new Point()
                    {
                        name =parts[0],
                        x = double.Parse(parts[1])*1000,
                        y = double.Parse(parts[2])*1000,
                        z = double.Parse(parts[3])*1000
                    };
                    allpoints.Add(p);
                    dataGridView1.Rows.Add(p.name, p.x, p.y, p.z);
                    toolStripLabel1.Text = "已成功导入";
                }
            }
            catch
            {
                MessageBox.Show("导入失败");
                return;
            }
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double c = 299792458;

            double[] alpha = { 0.1397e-7, -0.7451e-8, -0.5960e-7, 0.1192e-6 };
            double[] beta = { 0.1270e6, -0.1966e6, 0.6554e5, 0.2621e6 };
            double xP = -2225669.7744, yP = 4998936.1598, zP = 3265908.9678;
            double BP = 30 * Math.PI / 180;
            double Bphu = BP / Math.PI;
            double LP = 114 * Math.PI / 180;
            double Lphu = LP / Math.PI;

            double[,] H = new double[,]
            {
                {-Math.Sin(BP)*Math.Cos(LP),-Math.Sin(BP)*Math.Sin(LP),Math.Cos(BP) },
                {-Math.Sin(LP),Math.Cos(LP),0 },
                {Math.Cos(BP)*Math.Cos(LP),Math.Cos(BP)*Math.Sin(LP),Math.Sin(BP) }
            };
            foreach(Point s in allpoints)
            {
                double dX = s.x - xP;
                double dY = s.y - yP;
                double dZ = s.z - zP;
                double X = H[0, 0] * dX + H[0, 1] * dY + H[0, 2] * dZ;
                double Y = H[1, 0] * dX + H[1, 1] * dY + H[1, 2] * dZ;
                double Z = H[2, 0] * dX + H[2, 1] * dY + H[2, 2] * dZ;

                s.A = Math.Atan2(Y, X);
                if (s.A < 0)
                {
                    s.A += 2 * Math.PI;
                }
                double fenshu = Z / Math.Sqrt(X * X + Y * Y);
                s.E = Math.Atan(fenshu);
                if (s.E < 0)
                {
                    s.Dion = 0;
                    continue;
                }
                double sve = s.E / Math.PI;
                double psi = 0.0137 / (sve + 0.11) - 0.022;
                s.faiIp = Bphu + psi * Math.Cos(s.A);
                if (s.faiIp > 0.416) s.faiIp = 0.416;
                if (s.faiIp < -0.416) s.faiIp = -0.416;
                s.laIP = Lphu + psi * Math.Sin(s.A) / Math.Cos(s.faiIp*Math.PI);
                s.faim = s.faiIp + 0.064 * Math.Cos((s.laIP - 1.617)*Math.PI);
                double A2 = alpha[0] + alpha[1] * s.faim + alpha[2] * s.faim * s.faim + alpha[3] * s.faim * s.faim * s.faim;
                double A4 = beta[0] + beta[1] * s.faim + beta[2] * s.faim * s.faim + beta[3] * s.faim * s.faim * s.faim;
                double t = 43200 * s.laIP + time.hour * 3600 + time.min * 60 + time.sec;
                if (t < 0) t += 86400;
                if (t > 86400) t -= 86400;
                double F = 1 + 16 * Math.Pow(0.53 - sve, 3);
                double Tion;
                double panduan = 2 * Math.PI * (t - 50400) / A4;
                if (Math.Abs(panduan) < 1.57)
                {
                    Tion = F * (5e-9 + A2 * Math.Cos(2 * Math.PI * (t - 50400) / A4));
                }
                else
                {
                    Tion = F * 5e-9;
                }
                double Dion = Tion * c;

                s.Tion = Tion;
                s.Dion = Dion;

            }
            toolStripLabel1.Text = "计算完成";

        }
        private double Rad2Deg(double radians)
        {
            return radians * 180 / Math.PI;
        }
        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            string reports = null;
            reports += "卫星名称".PadRight(8) + "高度角 (度)".PadRight(10) + "方位角 (度)".PadRight(15) + "电离层延迟 (秒)".PadRight(20) + "\n";
            foreach (Point satellite in allpoints)
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

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sa = new SaveFileDialog();
            sa.Filter = "Text Files (*.txt)|*.txt";
            //Text File(*.txt)|(*.txt)|
            sa.FileName = "result";
            if (sa.ShowDialog() == DialogResult.OK)
            {
                string filepath = sa.FileName;
                using (StreamWriter sw=new StreamWriter(filepath))
                {
                    sw.Write(richTextBox1.Text);
                }
            }
        }
    }
}
