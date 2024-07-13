using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 水准网平差及精度评定程序
{
    public partial class Form1 : Form
    {
        #region 全局变量
        string[] all_lines;
        Dictionary<string, string> knownPointsDir = new Dictionary<string, string>();
        double[] L = new double[7];
        List<double> yuanheight = new List<double>();
        Matrix<double> Qxx;
        Matrix<double> QLLgu;
        Vector<double> x;
        double[] newv = new double[7];
        #endregion

        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000; // 1秒更新一次
            timer1.Tick += Timer_Tick;
            timer1.Start();
            richTextBox3.Text += "历史记录" + '\n';
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
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
                //切割已知点
                string firstLine = all_lines[0];
                string[] firstLineParts = firstLine.Split(',');
                for (int i = 0; i < firstLineParts.Length - 1; i += 2)
                {
                    if (i + 1 < firstLineParts.Length)
                    {
                        knownPointsDir.Add(firstLineParts[i], firstLineParts[i + 1]);
                    }
                }
                //切割路径数据
                for (int i = 2; i < all_lines.Length; i++)
                {
                    string line = all_lines[i];
                    string[] parts = line.Split(',');

                    if (parts.Length == 5)
                    {

                        dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4]);
                    }
                }
                //显示已知点
                string knownPoints = null;
                foreach (var pair in knownPointsDir)
                {
                    string temp = pair.Key + ':' + pair.Value + ' ';
                    knownPoints += temp;
                }
                toolStripStatusLabel1.Text = knownPoints;
            }
            catch
            {
                MessageBox.Show("文件格式错误!");
                toolStripStatusLabel1.Text = "文件格式错误!";
                return;
            }
            tabControl1.SelectedTab = tabPage1;
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "打开文件" + '\n';
        }
        private void 路径信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.shuizhun;
            tabControl1.SelectedTab = tabPage3;
        }
        private void 题目ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string text = "如图所示水准网，有2个已知点，3个未知点，7个测段。各已知数据及观测值见下表。\n\n" +
              "已知点高程：H1=5.016m, H2=6.016m\n\n" +
              "求解：\n" +
              "(1) 求各待定点的高程；\n" +
              "(2) 3-4点的高差中误差；\n" +
              "(3) 3号点、4号点的高程中误差。\n";

            richTextBox1.Text = text;
            tabControl1.SelectedTab = tabPage2;
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "查看路径信息" + '\n';
        }
        private void 报告ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string newText = "n=7, t=3, u=3, r=4.方程数为7。\n" +
                "待定点5不是三条及三条以上水准路线交点\n" +
                "把h6和h7合并为一条水准路线\n" +
                 "设3, 4点高程依次为x1, x2" + '\n';

            richTextBox1.AppendText(newText);
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "解题报告" + '\n';
        }
        private void 图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.func;
            tabControl1.SelectedTab = tabPage3;
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "查看图片" + '\n';
        }
        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (knownPointsDir.Count == 0 || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("请先加载文件数据！");
                return;
            }

            Dictionary<string, double> unknownPointsHeights = new Dictionary<string, double>();

            double HA = double.Parse(knownPointsDir.Values.ElementAt(0));
            double HB = double.Parse(knownPointsDir.Values.ElementAt(1));
            List<double> measurements = new List<double>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                double h = double.Parse(row.Cells[2].Value.ToString());
                measurements.Add(h);
            }

            // 初始值
            double x1 = HA + measurements[0];
            double x2 = HA + measurements[1];

            unknownPointsHeights["x1"] = x1;
            unknownPointsHeights["x2"] = x2;
            List<double> length = new List<double>();
            //记录长度
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                double value = double.Parse(row.Cells[3].Value.ToString());
                length.Add(value);
            }

            List<double> height = new List<double>();
            //记录高差
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                double h = double.Parse(row.Cells[2].Value.ToString());
                height.Add(Math.Round(h, 3));
            }
            yuanheight.AddRange(height);
            double[] l = new double[] { 0, 0, -4, -3, -7, -2 };

            // 计算矩阵P
            double[,] P = new double[6, 6];
            double l67 = length[5] + length[6];
            List<double> length67 = new List<double>(length.GetRange(0, 5));
            length67.Add(l67);//加入67总距离
            for (int i = 0; i < 6; i++)
            {
                P[i, i] = Math.Round(1 / length67[i], 3);
            }

            // 将B和P转换为Matrix，将l转换为Vector
            var B = DenseMatrix.OfArray(new double[,]
            {
                { 1, 0},
                { 0, 1},
                { 1, 0},
                { 0, 1},
                { -1, 1},
                { -1, 0},
            });
            Matrix<double> PMatrix = Matrix<double>.Build.DenseOfArray(P);
            Vector<double> lVector = Vector<double>.Build.Dense(l);

            Matrix<double> NBB = B.Transpose().Multiply(PMatrix).Multiply(B);

            Vector<double> w = B.Transpose().Multiply(PMatrix).Multiply(lVector);
            x = NBB.Inverse().Multiply(w);
            double newX1 = x1 + x[0] * 0.001;
            double newX2 = x2 + x[1] * 0.001;
            Vector<double> v = B.Multiply(x) - lVector;

            double v6 = length[5] / (length[5] + length[6]) * v[5];
            double v7 = length[6] / (length[5] + length[6]) * v[5];

            for (int i = 0; i < 5; i++)
            {
                newv[i] = v[i];
            }
            newv[5] = v6;
            newv[6] = v7;

            for (int i = 0; i < L.Length; i++)
            {
                L[i] = height[i] + newv[i] * 0.001;
            }
            double newX3 = HB - L[6];

            richTextBox1.Text += ("Matrix B:\n");
            richTextBox1.Text += (MatrixToString(B) + "\n");
            richTextBox1.Text += ("Matrix P:\n");
            richTextBox1.Text += (MatrixToString(PMatrix) + "\n");
            richTextBox1.Text += ("Matrix NBB:\n");
            richTextBox1.Text += (MatrixToString(NBB) + "\n");
            richTextBox1.Text += ("Vector w:\n");
            richTextBox1.Text += (VectorToString(w) + "\n");
            richTextBox1.Text += ("Vector x:\n");
            richTextBox1.Text += (VectorToString(x) + "\n");
            richTextBox1.Text += "\n改正数: " + string.Join(", ", newv.Select(num => Math.Round(num, 3))) + '\n';
            richTextBox1.Text += "改正后高程: " + string.Join(", ", L.Select(element => Math.Round(element, 3)));
            richTextBox1.Text += '\n'+ "**********************************" + '\n';
            richTextBox1.Text += ("问题一:\n");
            richTextBox1.Text += "3号点高程:" + newX1.ToString("F4") + 'm' + '\n' + "4号点高程:" + newX2.ToString("F4") + 'm' + '\n' + "5号点高程:" + newX3.ToString("F4") + 'm' + '\n';
            Matrix<double> vmatrix = Matrix<double>.Build.Dense(6, 1);

            for (int i = 0; i < v.Count; i++)
            {
                vmatrix[i, 0] = v[i];
            }
            double result = (vmatrix.Transpose().Multiply(PMatrix).Multiply(vmatrix))[0, 0];
            double seigema = +Math.Round(Math.Sqrt(result) / 2, 1);
            Qxx = NBB.Inverse();
            QLLgu = B.Multiply(NBB.Inverse()).Multiply(B.Transpose());

            double segemac = seigema * Math.Sqrt(Qxx[0, 0]);
            double segemad = seigema * Math.Sqrt(Qxx[1, 1]);
            richTextBox1.Text += "**********************************" + '\n';
            richTextBox1.Text += "问题二:\n";
            richTextBox1.Text += "seigema:" + seigema + "mm" + '\n';

            richTextBox1.Text += "CD点高程中误差:\n";
            richTextBox1.Text += "C:" + Math.Round(segemac, 1).ToString() + "mm" + '\n';
            richTextBox1.Text += "D:" + Math.Round(segemad, 1).ToString() + "mm" + '\n';
            double segemah5 = seigema * Math.Sqrt(QLLgu[4, 4]);
            richTextBox1.Text += "**********************************" + '\n'; ;
            richTextBox1.Text += "问题三:\n";
            richTextBox1.Text += "h5中误差:" + Math.Round(segemah5, 1).ToString() + "mm";
            tabControl1.SelectedTab = tabPage2;
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "计算" + '\n';
        }
        private void 保存当前报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string reportContent = richTextBox1.Text;
                File.WriteAllText(saveFileDialog.FileName, reportContent);
            }
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "保存当前报告" + '\n';
        }
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpText = "水准网平差及精度评定程序使用说明:\n" +
                              "1. 打开数据文件。-下方会显示已知点信息\n" +
                              "2. 查看已知点和路径信息。\n" +
                              "3. 阅读题目描述。\n" +
                              "4. 生成平差过程的矩阵。\n" +
                              "5. 可查看水准网示意图。\n" +
                              "6. 进行水准网平差计算。\n" +
                              "7. 查看计算结果。\n" +
                              "8. 保存报告。\n" +
                              "9. 退出程序。\n" +
                              "按顺序点击按钮即可!!\n" +
                              "\n请注意，数据文件格式必须正确，否则程序将提示错误。";

            MessageBox.Show(helpText, "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void 清除数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.ExecutablePath);
            Application.Exit();
        }
        private void 对比图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (yuanheight == null || L == null)
            {
                MessageBox.Show("请先进行平差计算!");
                return;
            }

            // 清除旧的数据点
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            // 创建新的ChartArea
            ChartArea chartArea1 = new ChartArea("ChartArea1");
            chart1.ChartAreas.Add(chartArea1);

            // 创建两个Series
            Series series1 = new Series("Series1");
            series1.ChartArea = "ChartArea1";
            series1.ChartType = SeriesChartType.Line;

            Series series2 = new Series("Series2");
            series2.ChartArea = "ChartArea1";
            series2.ChartType = SeriesChartType.Line;
            // 设置Series的颜色
            series1.Color = Color.Red;
            series2.Color = Color.Blue;
            // 添加Series到Chart
            chart1.Series.Add(series1);
            chart1.Series.Add(series2);

            // 添加数据点
            for (int i = 0; i < 6; i++)
            {
                series1.Points.AddXY(i + 1, yuanheight[i]);
                series2.Points.AddXY(i + 1, L[i]);
            }

            // 设置Y轴的刻度和标题
            chartArea1.AxisY.Title = "整体趋势";
            chartArea1.AxisY2.Title = "微小差异";
            chartArea1.AxisY.TitleFont = new Font("微软雅黑", 12, FontStyle.Bold); 
            chartArea1.AxisY2.TitleFont = new Font("微软雅黑", 12, FontStyle.Bold);
            // 使用LINQ获取最小值和最大值
            double minY = yuanheight.Min();
            double maxY = yuanheight.Max();

            // 设置Y轴的显示范围
            chartArea1.AxisY.Minimum = minY - 1;
            chartArea1.AxisY.Maximum = maxY + 1;
            chartArea1.AxisY2.Minimum = minY - 0.01;
            chartArea1.AxisY2.Maximum = maxY + 0.01;

            // 将第二个系列的Y轴设置为Y2轴
            series2.YAxisType = AxisType.Secondary;

            // 更新图表
            chart1.Invalidate();
            tabControl1.SelectedTab = tabPage4;
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "查看对比图像" + '\n';
        }
        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double width = chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum;
            double height = chart1.ChartAreas[0].AxisY.Maximum - chart1.ChartAreas[0].AxisY.Minimum;
            double x_cen = chart1.ChartAreas[0].AxisX.Maximum - width / 2;
            double y_cen = chart1.ChartAreas[0].AxisY.Maximum - height / 2;
            chart1.ChartAreas[0].AxisX.Maximum = x_cen + width / 2 * 1.2;
            chart1.ChartAreas[0].AxisX.Minimum = x_cen - width / 2 * 1.2;
            chart1.ChartAreas[0].AxisY.Maximum = y_cen + height / 2 * 1.2;
            chart1.ChartAreas[0].AxisY.Minimum = y_cen - height / 2 * 1.2;
        }
        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double width = chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum;
            double height = chart1.ChartAreas[0].AxisY.Maximum - chart1.ChartAreas[0].AxisY.Minimum;
            double x_cen = chart1.ChartAreas[0].AxisX.Maximum - width / 2;
            double y_cen = chart1.ChartAreas[0].AxisY.Maximum - height / 2;
            chart1.ChartAreas[0].AxisX.Maximum = x_cen + width / 2 * 0.8;
            chart1.ChartAreas[0].AxisX.Minimum = x_cen - width / 2 * 0.8;
            chart1.ChartAreas[0].AxisY.Maximum = y_cen + height / 2 * 0.8;
            chart1.ChartAreas[0].AxisY.Minimum = y_cen - height / 2 * 0.8;
        }
        private void 保存对比图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 创建一个FileDialog对象来选择保存位置
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Bitmap Image (*.bmp)|*.bmp|JPEG Image (*.jpg)|*.jpg|PNG Image (*.png)|*.png";
            saveDialog.Title = "保存对比图像";
            saveDialog.ShowDialog();

            if (saveDialog.FileName != "")
            {
                // 将图表转换为Bitmap对象
                Bitmap bitmap = new Bitmap(chart1.Width, chart1.Height);
                Graphics g = Graphics.FromImage(bitmap);
                chart1.DrawToBitmap(bitmap, new Rectangle(0, 0, chart1.Width, chart1.Height));
                g.Dispose();

                // 创建ImageFormat对象
                ImageFormat format = ImageFormat.Jpeg;

                // 保存Bitmap对象到指定位置
                bitmap.Save(saveDialog.FileName, format);
            }
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "保存对比图像" + '\n';
        }
        private static string MatrixToString(Matrix<double> matrix)
        {
            string result = "";
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    result += Math.Round(matrix[i, j], 2).ToString() + " ";
                }
                result += Environment.NewLine;
            }
            return result;
        }
        private static string VectorToString(Vector<double> vector)
        {
            string result = "";
            for (int i = 0; i < vector.Count; i++)
            {
                result += Math.Round(vector[i], 2).ToString("F3") + " ";
            }
            return result;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 更新当前时间
            label6.Text = "北京时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (Qxx == null)
            {
                MessageBox.Show("您还未进行平差计算");
                return;
            }
            textBox1.Text = "6";
            textBox2.Text = "2";
            textBox3.Text = "4";
            textBox4.Text = "2";
            richTextBox2.Text += (MatrixToString(Qxx) + "\n");
            richTextBox2.Text += (MatrixToString(QLLgu) + "\n");
            if (yuanheight == null || L == null)
            {
                MessageBox.Show("请先进行平差计算!");
                return;
            }

            // 清除旧的数据点
            chart2.Series.Clear();
            chart2.ChartAreas.Clear();

            // 创建新的ChartArea
            ChartArea chartArea1 = new ChartArea("ChartArea1");
            chart2.ChartAreas.Add(chartArea1);

            // 创建两个Series
            Series series1 = new Series("Series1");
            series1.ChartArea = "ChartArea1";
            series1.ChartType = SeriesChartType.Line;

            Series series2 = new Series("Series2");
            series2.ChartArea = "ChartArea1";
            series2.ChartType = SeriesChartType.Line;
            // 设置Series的颜色
            series1.Color = Color.Red;
            series2.Color = Color.Blue;
            // 添加Series到Chart
            chart2.Series.Add(series1);
            chart2.Series.Add(series2);

            // 添加数据点
            for (int i = 0; i < 6; i++)
            {
                series1.Points.AddXY(i + 1, yuanheight[i]);
                series2.Points.AddXY(i + 1, L[i]);
            }

            // 设置Y轴的刻度和标题
            chartArea1.AxisY.Title = "整体趋势";
            chartArea1.AxisY2.Title = "微小差异";
            chartArea1.AxisY.TitleFont = new Font("微软雅黑", 12, FontStyle.Bold);
            chartArea1.AxisY2.TitleFont = new Font("微软雅黑", 12, FontStyle.Bold);
            // 使用LINQ获取最小值和最大值
            double minY = yuanheight.Min();
            double maxY = yuanheight.Max();

            // 设置Y轴的显示范围
            chartArea1.AxisY.Minimum = minY - 1;
            chartArea1.AxisY.Maximum = maxY + 1;
            chartArea1.AxisY2.Minimum = minY - 0.01;
            chartArea1.AxisY2.Maximum = maxY + 0.01;

            // 将第二个系列的Y轴设置为Y2轴
            series2.YAxisType = AxisType.Secondary;

            // 更新图表
            chart2.Invalidate();

            dataGridView3.Rows.Add('3', Math.Round(x[0],3));
            dataGridView3.Rows.Add('4', Math.Round(x[1], 3));
            int j = 1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                double h = double.Parse(row.Cells[2].Value.ToString());
                dataGridView2.Rows.Add($"h{j}", Math.Round(h,3),Math.Round(newv[j-1],3),Math.Round(L[j-1],3));
                j++;

            }

        }
        private void 查看题目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = "如图所示水准网，有2个已知点，3个未知点，7个测段。各已知数据及观测值见下表。\n\n" +
              "已知点高程：H1=5.016m, H2=6.016m\n\n" +
              "求解：\n" +
              "(1) 求各待定点的高程；\n" +
              "(2) 3-4点的高差中误差；\n" +
              "(3) 3号点、4号点的高程中误差。\n";

            MessageBox.Show(text, "水准网平差及精度评定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            richTextBox3.Text += currentTime + '\n';
            richTextBox3.Text += "查看题目" + '\n';
        }
    }
}
