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

namespace 第一次测试
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> pointslist = new List<Point>();
        int dx = 10;
        int dy = 10;
        int num;
        StringBuilder report = new StringBuilder();
        List<Pingmian> pms = new List<Pingmian>();


        public Form1()
        {
            InitializeComponent();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
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
                num = int.Parse(all_lines[0]);
                double maxx = 0;
                double maxy = 0;
                double maxz = 0;
                double minx = 0;
                double miny = 0;
                double minz = 0;
                double zsum = 0;
                int cnum = 0;
                double cmaxz = 0;
                double cminz = 0;
                for (int i = 1; i < all_lines.Length; i++)
                {
                    string line = all_lines[i];
                    string[] parts = line.Split(',');

                    if (parts.Length == 4)
                    {
                        Point point = new Point
                        {
                            pointname = parts[0],
                            x = double.Parse(parts[1]),
                            y = double.Parse(parts[2]),
                            z = double.Parse(parts[3]),

                        };

                        pointslist.Add(point);

                        maxx = Math.Max(pointslist[i - 1].x, maxx);
                        maxy = Math.Max(pointslist[i - 1].y, maxy);
                        maxz = Math.Max(pointslist[i - 1].z, maxz);
                        minx = Math.Min(pointslist[i - 1].x, minx);
                        miny = Math.Min(pointslist[i - 1].y, miny);
                        minz = Math.Min(pointslist[i - 1].z, minz);


                        pointslist[i - 1].i = (int)Math.Floor(pointslist[i - 1].y / dy);
                        pointslist[i - 1].j = (int)Math.Floor(pointslist[i - 1].x / dx);
                        if (pointslist[i - 1].i == 2 && pointslist[i - 1].j == 3)
                        {
                            cnum++;
                            zsum += pointslist[i - 1].z;
                            cmaxz = Math.Max(pointslist[i - 1].z, cmaxz);
                            cminz = Math.Min(pointslist[i - 1].z, cminz);

                        }
                        dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], pointslist[i - 1].i, pointslist[i - 1].j);
                    }
                }
                report.AppendLine("1,P5 的坐标分量:" + pointslist[4].x);
                report.AppendLine("2,P5 的坐标分量:" + pointslist[4].y);
                report.AppendLine("3,P5 的坐标分量:" + pointslist[4].z);
                report.AppendLine("4,坐标分量 x 的最小值:" + minx);
                report.AppendLine("5,坐标分量 y 的最小值:" + miny);
                report.AppendLine("6,坐标分量 z 的最小值:" + minz);
                report.AppendLine("7,坐标分量 x 的最大值:" + maxx);
                report.AppendLine("8,坐标分量 y 的最大值:" + maxy);
                report.AppendLine("9,坐标分量 z 的最大值:" + maxz);
                report.AppendLine("10,P5 点的所在栅格的行 i:" + pointslist[4].i);
                report.AppendLine("11,P5 点的所在栅格的行 j:" + pointslist[4].j);
                report.AppendLine("12,栅格 C 中的点的数量 :" + cnum);
                report.AppendLine("13,栅格 C 中的平均高度 :" + (zsum / num).ToString("F3"));
                report.AppendLine("14,栅格 C 中高度的最大值 :" + cmaxz);
                report.AppendLine("15,栅格 C 中的高度差 :" + (cmaxz - cminz));
                double sumfangcha = 0;
                for (int k = 0; k < num; k++)
                {
                    double temp = (pointslist[k].z - (zsum / num)) * (pointslist[k].z - (zsum / num));
                    sumfangcha += temp;
                }
                sumfangcha = sumfangcha / num;
                report.AppendLine("16,栅格 C 中的高度方差 :" + sumfangcha.ToString("F3"));
            }
            catch
            {
                MessageBox.Show("读取错误");
                toolStripLabel1.Text = "文件格式错误!";
                return;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pointslist == null)
            {
                MessageBox.Show("请导入数据");
                return;
            }
            if (pms.Count()!=0)
            {
                MessageBox.Show("计算完成");
                return;
            }
            int i = 0;
            for (int time = 0; time < 300; time++, i += 2)
            {
                bool gongxian = GetPingmian(pointslist[i], pointslist[i + 1], pointslist[i + 2]);
                if (gongxian)
                {
                    List<Point> points = new List<Point>();
                    for (int j = 0; j < pointslist.Count; j++)
                    {
                        if (j < i || j >= i + 3)
                        {
                            points.Add(pointslist[j]);
                        }
                    }
                    pms[time].pnum = calneibudian(pms[time], points);
                    if (time == 0)
                    {
                        report.AppendLine("17,P1-P2-P3 构成三角形的面积:" + pms[0].S.ToString("F6"));
                        report.AppendLine("18,拟合平面 S1 的参数 A:" + pms[0].A.ToString("F6"));
                        report.AppendLine("19,拟合平面 S1 的参数 B:" + pms[0].B.ToString("F6"));
                        report.AppendLine("20,拟合平面 S1 的参数 C:" + pms[0].C.ToString("F6"));
                        report.AppendLine("21,拟合平面 S1 的参数 D:" + pms[0].D.ToString("F6"));
                        double p10002S1 = diantopingmian(pms[0], pointslist[999]);
                        double p52S1 = diantopingmian(pms[0], pointslist[4]);
                        report.AppendLine("22,P1000 到拟合平面 S1 的距离 :" + p10002S1.ToString("F3"));
                        report.AppendLine("23,P5 到拟合平面 S1 的距离 :" + p52S1.ToString("F3"));

                        pms[time].pnum = calneibudian(pms[time], points);
                        report.AppendLine("24,拟合平面 S1 的内部点数量  :" + pms[time].pnum);
                        report.AppendLine("25,拟合平面 S1 的外部点数量  :" + (points.Count() - pms[time].pnum));
                    }

                }
                else
                {
                    time--;
                }
            }
            int temp = 0;

            for (int k = 0; k < 300; k++)
            {
                if (pms[k].pnum >= pms[temp].pnum)
                {
                    temp = k;
                }
            }
            Pingmian J1 = pms[temp];
            List<Point> j2points = new List<Point>();
            List<Point> j1points = new List<Point>();
            for (int j = 0; j < pointslist.Count; j++)
            {   
                if (j < temp || j >= temp + 3)
                {
                    j1points.Add(pointslist[j]);
                }
            }
            j2points = calquneibudian(J1, j1points);
            for (int r = 0; r < 80; r++)
            {
                bool j2gongxian = GetPingmian(pointslist[r], pointslist[r + 1], pointslist[r + 2]);
                if (j2gongxian)
                {
                    List<Point> j2pointstemp = new List<Point>();
                    for (int j = 0; j < j2points.Count; j++)
                    {
                        if (j < i || j >= i + 3)
                        {
                            j2pointstemp.Add(pointslist[j]);
                        }
                    }
                    pms[r].pnum = calneibudian(pms[r], j2pointstemp);
                }
            }
            int tempj2 = 0;
            for (int u = 0; u < 80; u++)
            {
                if (pms[u].pnum >= pms[tempj2].pnum)
                {
                    tempj2 = u;
                }
            }
            Pingmian J2 = pms[tempj2];

            Point p52J1 = caltouyingdianzuobiao(J1, pointslist[4]);
            Point p8002J2 = caltouyingdianzuobiao(J2, pointslist[799]);

            #region 输出
            report.AppendLine("26,最佳分割平面 J1 的参数 A  :" + J1.A.ToString("F6"));
            report.AppendLine("27,最佳分割平面 J1 的参数 B  :" + J1.B.ToString("F6"));
            report.AppendLine("28,最佳分割平面 J1 的参数 C  :" + J1.C.ToString("F6"));
            report.AppendLine("29,最佳分割平面 J1 的参数 D  :" + J1.D.ToString("F6"));
            report.AppendLine("30,最佳分割平面 J1 的内部点数量  :" + J1.pnum);
            report.AppendLine("31,最佳分割平面 J1 的外部点数量  :" + (997 - J1.pnum));
            report.AppendLine("32,最佳分割平面 J2 的参数 A  :" + J2.A.ToString("F6"));
            report.AppendLine("33,最佳分割平面 J2 的参数 B  :" + J2.B.ToString("F6"));
            report.AppendLine("34,最佳分割平面 J2 的参数 C  :" + J2.C.ToString("F6"));
            report.AppendLine("35,最佳分割平面 J2 的参数 D  :" + J2.D.ToString("F6"));
            report.AppendLine("36,最佳分割平面 J2 的内部点数量  :" + J2.pnum);
            report.AppendLine("37,最佳分割平面 J2 的外部点数量  :" + (997 - J1.pnum-3-J2.pnum));
            report.AppendLine("38,P5 点到最佳分割面（J1）的投影坐标 xt  :" + p52J1.x.ToString("F3"));
            report.AppendLine("39,P5 点到最佳分割面（J1）的投影坐标 yt  :" + p52J1.y.ToString("F3"));
            report.AppendLine("40,P5 点到最佳分割面（J1）的投影坐标 zt  :" + p52J1.z.ToString("F3"));
            report.AppendLine("41,P800 点到最佳分割面（J1）的投影坐标 xt  :" + p8002J2.x.ToString("F3"));
            report.AppendLine("42,P800 点到最佳分割面（J1）的投影坐标 yt  :" + p8002J2.y.ToString("F3"));
            report.AppendLine("43,P800 点到最佳分割面（J1）的投影坐标 zt  :" + p8002J2.z.ToString("F3"));

            richTextBox1.Text += report;
            tabControl1.SelectedTab = tabPage2;
            #endregion
        }

        private Point caltouyingdianzuobiao(Pingmian pm,Point p)
        {
            double xt = ((pm.B * pm.B + pm.C * pm.C) * p.x - pm.A * (pm.B * p.y + pm.C * p.z + pm.D)) / (pm.A * pm.A + pm.B * pm.B + pm.C * pm.C);
            double yt = ((pm.A * pm.A + pm.C * pm.C) * p.y - pm.B * (pm.A * p.x + pm.C * p.z + pm.D)) / (pm.A * pm.A + pm.B * pm.B + pm.C * pm.C);
            double zt = ((pm.A * pm.A + pm.B * pm.B) * p.z - pm.C * (pm.A * p.x + pm.B * p.y + pm.D)) / (pm.A * pm.A + pm.B * pm.B + pm.C * pm.C);
            Point po = new Point();
            po.x = xt;
            po.y = yt;
            po.z = zt;
            return po;
        }

        private int calneibudian(Pingmian pm, List<Point> points)
        {
            int num = 0;
            double s = 0;

            for (int i = 0; i < points.Count(); i++)
            {
                s = diantopingmian(pm, points[i]);
                if (s < 0.1)
                {
                    num++;
                }
            }

            return num;
        }

        private List<Point> calquneibudian(Pingmian pm, List<Point> points)
        {
            double s = 0;
            List<Point> j2points = new List<Point>();
            for (int i = 0; i < points.Count(); i++)
            {
                s = diantopingmian(pm, points[i]);
                if (s >= 0.1)
                {
                    j2points.Add(points[i]);
                }
            }
            return j2points;
        }

        private bool GetPingmian(Point p1, Point p2, Point p3)
        {
            double a = caljvli(p1, p2);
            double b = caljvli(p2, p3);
            double c = caljvli(p1, p3);
            double p = (a + b + c) / 2;
            double s = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            if (s > 0.1)
            {
                double daa = (p2.y - p1.y) * (p3.z - p1.z) - (p3.y - p1.y) * (p2.z - p1.z);
                double dab = (p2.z - p1.z) * (p3.x - p1.x) - (p3.z - p1.z) * (p2.x - p1.x);
                double dac = (p2.x - p1.x) * (p3.y - p1.y) - (p3.x - p1.x) * (p2.y - p1.y);
                double dad = -daa * p1.x - dab * p1.y - dac * p1.z;
                Pingmian pm = new Pingmian
                {
                    S = s,
                    A = daa,
                    B = dab,
                    C = dac,
                    D = dad,
                };
                pms.Add(pm);
                return true;
            }
            else
            {
                return false;
            }
        }

        private double caljvli(Point p1, Point p2)
        {
            double s = Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z));
            return s;
        }

        private double diantopingmian(Pingmian pm, Point p)
        {
            double fenzi = Math.Abs(pm.A * p.x + pm.B * p.y + pm.C * p.z + pm.D);
            double fenmu = Math.Sqrt(pm.A * pm.A + pm.B * pm.B + pm.C * pm.C);
            return fenzi / fenmu;
        }

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            richTextBox1.Clear();
            pointslist.Clear();
        }

        private void 保存报告ToolStripMenuItem_Click(object sender, EventArgs e)
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
