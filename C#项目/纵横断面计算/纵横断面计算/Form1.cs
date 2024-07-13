using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 纵横断面计算
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> list = new List<Point>();
        double H0;
        Point A = new Point();
        Point B = new Point();
        List<Point> KPoints = new List<Point>();
        List<Point> allPoints = new List<Point>();
        List<Point> midPoints = new List<Point>();
        public Form1()
        {
            InitializeComponent();
        }

        private void 数据文件读入ToolStripMenuItem_Click(object sender, EventArgs e)
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
                string firstLine = all_lines[0];
                string[] firstLineParts = firstLine.Split(',');
                H0 = double.Parse(firstLineParts[1]);

                string[] aline = all_lines[2].Split(',');
                string[] bline = all_lines[3].Split(',');
                A = new Point { pointname = aline[0], x = double.Parse(aline[1]), y = double.Parse(aline[2]), z = 0 };
                B = new Point { pointname = bline[0], x = double.Parse(bline[1]), y = double.Parse(bline[2]), z = 0 };


                for (int i = 5; i < all_lines.Length; i++)
                {
                    string line = all_lines[i];
                    string[] parts = line.Split(',');
                    if (parts.Length == 4)
                    {
                        Point po = new Point
                        {
                            pointname = parts[0],
                            x = double.Parse(parts[1]),
                            y = double.Parse(parts[2]),
                            z = double.Parse(parts[3]),
                        };
                        list.Add(po);
                        dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3]);
                    }
                }
                toolStripStatusLabel1.Text = "当前H0:" + H0.ToString("f3") + 'm';
            }
            catch
            {
                MessageBox.Show("文件格式错误!");
                toolStripStatusLabel1.Text = "文件格式错误!";
                return;
            }
        }

        private void 坐标方位角计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Angle bearing = Angle.CalculateBearing(A, B);
            string output = $"*******坐标方位角计算结果*******\n\n" +
                 $"度（dd°）\t分（mm′）\t秒（ss.ssss″）\n" +
                 $"{bearing.Degrees}°\t \t{bearing.Minutes}′\t \t{bearing.Seconds:0.####}″\n";
            richTextBox1.AppendText(output);
            tabControl1.SelectTab(tabPage2);
        }

        private void 反距离加权法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point K1 = list.FirstOrDefault(point => point.pointname == "K1");

            if (K1 == null)
            {
                MessageBox.Show("未找到点号K1的点。");
                return;
            }

            var nearestPoints = list.Where(point => point != K1).OrderBy(point => Distance(K1, point)).Take(5).ToList();

            double interpolatedHeight = InverseDistanceWeighting(K1, nearestPoints);

            string output = "以K1为内插点，最近5个点的点号、坐标(X,Y,H)和距离如下:\n";
            foreach (var Qi in nearestPoints)
            {
                double distance = Distance(K1, Qi);
                output += $"{Qi.pointname}\t{Qi.x,10:0.000}\t{Qi.y,10:0.000}\t{Qi.z,7:0.000}\t{distance,7:0.000}\n";
            }
            output += $"内插点K1的内插高程为：{interpolatedHeight,7:0.000} 米";

            richTextBox1.Text = output;
            tabControl1.SelectTab(tabPage2);
        }

        private void 断面面积计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 查找K0和K1点
            Point K0 = list.FirstOrDefault(point => point.pointname == "K0");
            Point K1 = list.FirstOrDefault(point => point.pointname == "K1");

            // 检查K0和K1点是否都存在
            if (K0 == null || K1 == null)
            {
                MessageBox.Show("未找到点号K0或K1的点。");
                return;
            }

            // 假设H0是一个已经定义的变量
            double h0 = H0;

            // 调用封装的函数计算面积
            double area = CalculateTrapezoidArea(K0, K1, h0);

            // 输出结果
            string output = $"以K_0，K_1为梯形的两个端点的梯形面积为：{area.ToString("F3")} 平方米\n";
            richTextBox1.Text = output;
            tabControl1.SelectTab(tabPage2);
        }

        private void 纵断面长度计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            KPoints = list.Where(point => point.pointname.StartsWith("K")).ToList();


            if (KPoints.Count == 0)
            {
                MessageBox.Show("未找到任何以K开头的点。");
                return;
            }

            // 初始化纵断面总长度
            double totalLength = 0;

            // 初始化前一个点
            Point previousPoint = KPoints[0];

            // 遍历KPoints列表，计算纵断面总长度
            foreach (var point in KPoints.Skip(1))
            {

                double distance = Distance(previousPoint, point);

                totalLength += distance;

                previousPoint = point;
            }


            string output = $"纵断面的总长度为：{totalLength.ToString("F3")} 米\n";
            richTextBox1.Text = output;
            tabControl1.SelectTab(tabPage2);
        }

        private void 内插点平面坐标计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KPoints = list.Where(point => point.pointname.StartsWith("K")).ToList();
            double delta = 10; // 内插距离为10米
            allPoints = list.ToList(); // 复制列表以包含所有点
            List<Point> interpolatedPoints = new List<Point>();

            // 输出内插点坐标和高程到报告

            double currentDistance = 10;
            double licheng = 0;
            for (int i = 0; i < KPoints.Count - 1; i++)
            {
                Point K0 = KPoints[i];
                Point K1 = KPoints[i + 1];
                double distance = Distance(K0, K1);
                licheng += distance;
                // 如果是第一个关键点，设置其里程为0
                if (i == 0)
                {
                    K0.mileage = 0;
                }

                // 计算从K0到K1的插值点
                int vIndex = 1; // 内插点编号
                while (currentDistance < licheng)
                {
                    Point Pi = new Point();
                    Angle bearing = Angle.CalculateBearing(K0, K1);
                    double Li = currentDistance; // Li 是 Pi 和 K0 之间的距离
                    double D0 = K0.mileage; // D0 是 Kj 和 K0 之间的距离

                    Pi.x = K0.x + (Li - D0) * Math.Cos(bearing.ToRadians());
                    Pi.y = K0.y + (Li - D0) * Math.Sin(bearing.ToRadians());

                    // 设置里程
                    Pi.mileage = currentDistance;

                    // 设置点名，V-1, V-2, ...
                    Pi.pointname = $"V-{interpolatedPoints.Count + 1}";

                    interpolatedPoints.Add(Pi);

                    allPoints.Add(Pi); // 将插值点添加到所有点的列表中

                    currentDistance += delta;
                    vIndex++;
                }

                // 更新下一个关键点的里程
                K1.mileage = licheng;

            }
            foreach (var point in interpolatedPoints)
            {
                point.z = InverseDistanceWeighting(point, allPoints);

            }
            // 将关键点和内插点按里程排序
            var sortedPoints = KPoints.Concat(interpolatedPoints).OrderBy(p => p.mileage).ToList();
            double totalArea = 0;
            for (int i = 0; i < sortedPoints.Count - 1; i++)
            {

                Point K0 = sortedPoints[i];
                Point K1 = sortedPoints[i + 1];

                // 计算梯形面积并累加到总面积中
                totalArea += CalculateTrapezoidArea(K0, K1, H0);
            }

            // 生成报告
            string report = "**************************纵断面计算报告**************************" + '\n' + "纵断面的面积为:" + totalArea.ToString("F3") + '\n';
            report += "点名\t里程K(m)\tX坐标(m)\tY坐标(m)\tH坐标(m)\n";

            // 定义一个格式化字符串，用于格式化里程，确保至少有三位小数
            string mileageFormat = "{0,-5}\t{1,7:F3}\t{2,10:F3}\t{3,10:F3}\t{4,10:F3}\n";

            foreach (var point in sortedPoints)
            {
                // 使用格式化字符串来确保对齐
                report += string.Format(mileageFormat, point.pointname, point.mileage, point.x, point.y, point.z);
            }

            richTextBox1.Text = report;
            tabControl1.SelectTab(tabPage2);
        }

        private void 横断面中心店计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (KPoints.Count < 3)
            {
                MessageBox.Show("至少需要三个关键点来计算中点！");
                return;
            }

            // 计算并显示两个中点
            midPoints = CalculateMidPoints(KPoints);
            string report = "横断面中心点坐标信息：";
            report += "X坐标(m)     Y坐标(m)\n";

            for (int i = 0; i < midPoints.Count; i++)
            {
                report += $"第{i + 1}个中点坐标：     {midPoints[i].x:F3}      {midPoints[i].y:F3}\n";
            }

            // 显示报告
            richTextBox1.Text = report;
            tabControl1.SelectTab(tabPage2);
        }

        private void 横断面插值点的ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string report = string.Empty; // 初始化报告字符串

            for (int i = 0; i < midPoints.Count; i++)
            {
                var midPoint = midPoints[i];

                // 计算方位角
                Angle bearing = Angle.CalculateBearing(midPoint, KPoints[0]);

                // 计算横断面面积
                double area = CalculateTrapezoidArea(KPoints[0], KPoints[1], H0);


                report += $"以M{i}为中间点的横断面面积为：{area:F3}\n";

                report += "****************************************************************************\n";
                report += "点名            X坐标(m)        Y坐标(m)        H坐标(m)\n";

                // 计算内插点的坐标和高程
                for (int j = -5; j <= 5; j++)
                {
                    double x = midPoint.x + j * 5 * Math.Cos(bearing.ToRadians());
                    double y = midPoint.y + j * 5 * Math.Sin(bearing.ToRadians());
                    Point interpolatePoint = new Point { x = x, y = y };

                    // 计算内插点的高程
                    double z = InverseDistanceWeighting(interpolatePoint, allPoints);

                    // 输出结果
                    report += $" {j + 6}-M          {x:F3}      {y:F3}      {z:F3}\n";
                }

                report += "\n";
            }

            richTextBox1.Text = report;
            tabControl1.SelectTab(tabPage2);
        }

        private void 保存当前报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 创建一个保存文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";

            // 如果用户选择了文件并点击了保存按钮
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取文本框内容
                string reportContent = richTextBox1.Text;

                // 保存到文件
                File.WriteAllText(saveFileDialog.FileName, reportContent);
            }
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpFilePath = Path.Combine(Application.StartupPath, "help.pdf");

            if (File.Exists(helpFilePath))
            {
                Process.Start(helpFilePath);
            }
            else
            {
                MessageBox.Show("帮助文件 'help.pdf' 不存在！");
            }
        }

        private void 退出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private double Distance(Point p1, Point p2)
        {
            double dx = p1.x - p2.x;
            double dy = p1.y - p2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private double CalculateTrapezoidArea(Point K0, Point K1, double h0)
        {
            // 计算K0和K1之间的距离
            double deltaL = Distance(K0, K1);

            // 计算梯形面积
            double area = (K1.z + K0.z - 2 * h0) / 2 * deltaL;

            return area;
        }

        private double InverseDistanceWeighting(Point point, List<Point> points)
        {
            var nearestPoints = points.OrderBy(p => Distance(p, point)).Take(5).ToList();
            double sumWeightedHeights = 0;
            double sumWeights = 0;

            foreach (var Qi in nearestPoints)
            {
                double distance = Distance(Qi, point);
                if (distance == 0) continue; // 距离为0时跳过，避免除以0
                double weight = 1 / (distance * distance);
                sumWeightedHeights += Qi.z * weight;
                sumWeights += weight;
            }

            if (sumWeights == 0)
            {
                MessageBox.Show("无法计算点的高程，因为权重总和为零。");
            }

            return sumWeightedHeights / sumWeights;
        }

        private List<Point> CalculateMidPoints(List<Point> points)
        {
            List<Point> midPoints = new List<Point>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                Point midPoint = new Point
                {
                    x = (points[i].x + points[i + 1].x) / 2,
                    y = (points[i].y + points[i + 1].y) / 2
                };
                midPoints.Add(midPoint);
            }

            return midPoints;
        }

    }
}
