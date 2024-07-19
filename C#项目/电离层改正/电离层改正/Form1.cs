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

namespace 电离层改正
{
    public partial class Form1 : Form
    {
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
                        x = double.Parse(parts[1])*1000,
                        y = double.Parse(parts[2])*1000,
                        z = double.Parse(parts[3])*1000,
                    };
                    allPoints.Add(s);
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

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double c = 299792458;

            double[] alpha = { 0.1397e-7, -0.7451e-8, -0.5960e-7, 0.1192e-6 };
            double[] beta = { 0.1270e6, -0.1966e6, 0.6554e5, 0.2621e6 };

            // 测站P的地心坐标
            double x_P = -2225669.7744, y_P = 4998936.1598, z_P = 3265908.9678;

            // 测站P的大地坐标
            double B_P = 30 * Math.PI / 180; // 转换为弧度
            double L_P = 114 * Math.PI / 180; // 转换为弧度

            // 计算坐标转换矩阵H
            double[,] H = new double[,]
            {
        {-Math.Sin(B_P) * Math.Cos(L_P), -Math.Sin(B_P) * Math.Sin(L_P), Math.Cos(B_P)},
        {-Math.Sin(L_P), Math.Cos(L_P), 0},
        {Math.Cos(B_P) * Math.Cos(L_P), Math.Cos(B_P) * Math.Sin(L_P), Math.Sin(B_P)}
            };

            // 对每个卫星点进行坐标转换和角度计算
            foreach (Point s in allPoints)
            {
                double deltaX = s.x - x_P;
                double deltaY = s.y - y_P;
                double deltaZ = s.z - z_P;

                // 坐标转换
                double X = H[0, 0] * deltaX + H[0, 1] * deltaY + H[0, 2] * deltaZ;
                double Y = H[1, 0] * deltaX + H[1, 1] * deltaY + H[1, 2] * deltaZ;
                double Z = H[2, 0] * deltaX + H[2, 1] * deltaY + H[2, 2] * deltaZ;

                // 计算方位角A和高度角E
                s.A = Math.Atan2(Y, X);
                if (s.A < 0)
                {
                    s.A += 2 * Math.PI;
                }
                double re = Z / (Math.Sqrt(X * X + Y * Y));
        
                 s.E = Math.Atan(re);
                if (s.E < 0)
                {
                    s.D_ion = 0;
                    continue;
                }
                double sve = s.E / Math.PI;
                // 计算参数 psi
                double psi = 0.0137 / (sve + 0.11) - 0.022;

                // 计算穿刺点 IP 的大地坐标
                s.Phi_IP = B_P/Math.PI + psi * Math.Cos(s.A);
                if (s.Phi_IP > 0.416) s.Phi_IP = 0.416;
                if (s.Phi_IP < -0.416) s.Phi_IP = -0.416;
                s.Lambda_IP = L_P/Math.PI + psi * Math.Sin(s.A) / Math.Cos(s.Phi_IP * Math.PI);

                // 计算地磁纬度 φm
                s.Phi_M = s.Phi_IP + 0.064 * Math.Cos((s.Lambda_IP - 1.617)*Math.PI);

                // 计算 A2 和 A4
                double A2 = alpha[0] + alpha[1] * s.Phi_M + alpha[2] * s.Phi_M * s.Phi_M + alpha[3] * s.Phi_M * s.Phi_M * s.Phi_M;
                double A4 = beta[0] + beta[1] * s.Phi_M + beta[2] * s.Phi_M * s.Phi_M + beta[3] * s.Phi_M * s.Phi_M * s.Phi_M;

                // 观测时刻 t（秒）

                double t = 43200 * s.Lambda_IP + time.hour * 3600 + time.min * 60 + time.sec;
                if (t < 0) t += 86400;
                if (t > 86400) t -= 86400;
                // 计算 F
                double F = 1 + 16 * Math.Pow(0.53 - sve, 3);

                // 计算 T_ion
                double T_ion;
                if (Math.Abs(2 * Math.PI * (t - 50400) / A4) < 1.57)
                {
                    T_ion = F * (5e-9 + A2 * Math.Cos(2 * Math.PI * (t - 50400) / A4));
                }
                else
                {
                    T_ion = F * 5e-9;
                }

                // 计算距离延迟 D_ion
                double D_ion = T_ion * c;

                s.T_ion = T_ion;
                s.D_ion = D_ion;
            }

            MessageBox.Show("计算完成\n可查看报告");

        }

        private double Rad2Deg(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string reports=null;
            reports += "卫星名称".PadRight(8) + "高度角 (度)".PadRight(10) + "方位角 (度)".PadRight(15) + "电离层延迟 (秒)".PadRight(20) + "\n";
            foreach (Point satellite in allPoints)
            {
                reports += satellite.name.PadRight(5) +
                           Math.Round(Rad2Deg(satellite.E), 3).ToString().PadLeft(15) +
                           Math.Round(Rad2Deg(satellite.A), 3).ToString().PadLeft(15) +
                           Math.Round(satellite.D_ion, 4).ToString().PadLeft(20) +
                           "\n";
            }

            richTextBox1.Text = reports;
            tabControl1.SelectedTab = tabPage2;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
