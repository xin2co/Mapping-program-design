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
using System.Windows.Forms.DataVisualization.Charting;

namespace 空间数据分析722
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> allPoints = new List<Point>();
        double[,] globalmatrix = new double[1000,1000];
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
                    Point s = new Point()
                    {
                        id = parts[0],
                        x = double.Parse(parts[1]),
                        y = double.Parse(parts[2]),
                        value = double.Parse(parts[3])
                    };
                    allPoints.Add(s);

                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3]);
                }
                toolStripLabel1.Text = "文件导入成功";
            }
            catch
            {
                MessageBox.Show("文件导入失败");
                return;
            }
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void 误差椭圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            double xba = 0;
            double yba = 0;
            double n = allPoints.Count;
            foreach (Point p in allPoints)
            {
                xba += p.x / n;
                yba += p.y / n;
            }

            double sumA2 = 0;
            double sumB2 = 0;
            double sumAB = 0;

            foreach (Point p in allPoints)
            {
                double ai = p.x - xba;
                double bi = p.y - yba;
                sumA2 += ai * ai;
                sumB2 += bi * bi;
                sumAB += ai * bi;
            }

            double theta = Math.Atan2((sumA2 - sumB2) + Math.Sqrt(Math.Pow((sumA2 - sumB2), 2) + 4 * Math.Pow(sumAB, 2)), 2 * sumAB);

            double SDE_x = Math.Sqrt(2) * Math.Sqrt(Math.Abs(sumA2 * Math.Cos(theta) * Math.Cos(theta) + 2 * sumAB * Math.Cos(theta) * Math.Sin(theta) + sumB2 * Math.Sin(theta) * Math.Sin(theta)) / n);
            double SDE_y = Math.Sqrt(2) * Math.Sqrt(Math.Abs(sumA2 * Math.Sin(theta) * Math.Sin(theta) - 2 * sumAB * Math.Cos(theta) * Math.Sin(theta) + sumB2 * Math.Cos(theta) * Math.Cos(theta)) / n);
            richTextBox1.Clear();
            toolStripLabel1.Text = $"当前计算:误差椭圆,长轴角度: {theta}°, 长半轴: {SDE_x}, 短半轴: {SDE_y}";
            richTextBox1.AppendText("详细报告\n");
            richTextBox1.AppendText("----------\n\n");
            richTextBox1.AppendText("平均坐标: (" + xba.ToString("F2") + ", " + yba.ToString("F2") + ")\n\n");
            richTextBox1.AppendText("误差椭圆参数:\n");
            richTextBox1.AppendText("长轴角度: " + (theta * (180 / Math.PI)).ToString("F2") + "°\n");
            richTextBox1.AppendText("长半轴长度: " + SDE_x.ToString("F2") + "\n");
            richTextBox1.AppendText("短半轴长度: " + SDE_y.ToString("F2") + "\n\n");
            DrawEllipse(xba, yba, SDE_x, SDE_y, theta);


        }

        private void DrawEllipse(double xba, double yba, double SDE_x, double SDE_y, double theta)
        {
            Series ellipseSeries = new Series("Ellipse")
            {
                ChartType = SeriesChartType.Line,
                IsVisibleInLegend = false,
                Color = Color.Red,
            };

            int pointsCount = 100;
            for (int i = 0; i <= pointsCount; i++)
            {
                double angle = (2 * Math.PI / pointsCount) * i;
                double x = SDE_x * Math.Cos(angle) * Math.Cos(theta) - SDE_y * Math.Sin(angle) * Math.Sin(theta) + xba;
                double y = SDE_x * Math.Cos(angle) * Math.Sin(theta) + SDE_y * Math.Sin(angle) * Math.Cos(theta) + yba;

                ellipseSeries.Points.AddXY(x, y);
            }

            chart1.Series.Clear();
            chart1.Series.Add(ellipseSeries);
        }

        private void 莫兰ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double S0 = 0;
            int n = allPoints.Count();

            double fenzi = 0, fenmu = 0;
            double ave = 0;
            foreach (var p in allPoints) ave += p.value;
            ave /= allPoints.Count();

            for (int i = 0; i < allPoints.Count(); i++)
                for (int j = 0; j < allPoints.Count(); j++)
                {
                    S0 += globalmatrix[i,j];
                    if (i == j) continue;
                    fenzi += globalmatrix[i,j] * (allPoints[i].value - ave) * (allPoints[j].value - ave);
                }

            for (int i = 0; i < allPoints.Count(); i++)
            {
                fenmu += Math.Pow((allPoints[i].value - ave), 2);
            }

            double I = 0;
            I = (n / S0) * (fenzi / fenmu);

            double[] I_local = new double[n];
            for (int i = 0; i < n; i++)
            {
                double Si2 = 0;
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i == j) continue;
                    Si2 += Math.Pow((allPoints[j].value - ave), 2);
                    sum += globalmatrix[i, j] * (allPoints[j].value - ave);
                }
                Si2 /= (n - 1);
                I_local[i] = (allPoints[i].value - ave) / Si2 * sum;
            }
            richTextBox1.Text+="全局莫兰指数 I: " + I.ToString("F8") + "\n\n";

            for (int i = 0; i < n; i++)
            {
                richTextBox1.Text += "点 " + i + " 的局部莫兰指数 Ii: " + I_local[i].ToString("F8") + "\n";
            }
            double[] zScores = new double[n];
            double variance = 0;

            for (int i = 0; i < n; i++)
            {
                variance += Math.Pow((allPoints[i].value - ave), 2);
            }
            variance /= n; 
            double standardDeviation = Math.Sqrt(variance); 

            for (int i = 0; i < n; i++)
            {
                zScores[i] = (allPoints[i].value - ave) / standardDeviation;
            }

            richTextBox1.Text += "\nZ得分:\n";
            for (int i = 0; i < n; i++)
            {
                richTextBox1.Text += "点 " + i + " 的Z得分: " + zScores[i].ToString("F4") + "\n";
            }
            tabControl1.SelectedTab = tabPage2;
        }
        private double Distance(Point p1, Point p2)
        {
            double result = (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
            result = Math.Sqrt(result);
            return result;
        }
        public double[,] fanjiaoquan(double[,] matrix)
        {
            for (int i = 0; i < allPoints.Count(); i++)
            {
                for (int j = 0; j < allPoints.Count(); j++)
                {
                    if (i == j) continue;
                    double re = 0;

                    re = 1.0 / Distance(allPoints[i], allPoints[j]);
                    matrix[i, j] = re;
                }
            }
            return matrix;
        }
        public double[,] Init(int Col, int Row)
        {
            double[,] matrix = new double[Col, Row];
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    matrix[i, j] = 0;
                }
            }

            return matrix;
        }

        private void 反距离加权法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] matrix = Init(allPoints.Count, allPoints.Count);
            matrix = fanjiaoquan(matrix);
            globalmatrix = matrix;
        }

        private void 半径权重距离矩阵ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form inputDialog = new Form();
            inputDialog.Text = "输入阈值";
            inputDialog.StartPosition = FormStartPosition.CenterScreen;
            inputDialog.Size = new System.Drawing.Size(220, 120);

            Label lblThreshold = new Label();
            lblThreshold.Text = "请输入阈值d：";
            lblThreshold.Location = new System.Drawing.Point(10, 20);
            lblThreshold.Size = new System.Drawing.Size(100, 20);
            inputDialog.Controls.Add(lblThreshold);

            TextBox txtThreshold = new TextBox();
            txtThreshold.Location = new System.Drawing.Point(110, 20);
            txtThreshold.Size = new System.Drawing.Size(70, 20);
            inputDialog.Controls.Add(txtThreshold);

            Button btnOK = new Button();
            btnOK.Text = "确定";
            btnOK.Location = new System.Drawing.Point(30, 50);
            btnOK.Click += (senderBtn, eBtn) =>
            {
                if (double.TryParse(txtThreshold.Text, out double d0))
                {
                    inputDialog.DialogResult = DialogResult.OK;
                    inputDialog.Close();
                }
                else
                {
                    MessageBox.Show("请输入有效的数字阈值。");
                }
            };
            inputDialog.Controls.Add(btnOK);

            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new System.Drawing.Point(110, 50);
            btnCancel.Click += (senderBtn, eBtn) =>
            {
                inputDialog.DialogResult = DialogResult.Cancel;
                inputDialog.Close();
            };
            inputDialog.Controls.Add(btnCancel);
            double[,] matrix = Init(allPoints.Count, allPoints.Count);
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                double d0 = double.Parse(txtThreshold.Text);
                

                for (int i = 0; i < allPoints.Count; i++)
                {
                    for (int j = 0; j < allPoints.Count; j++)
                    {
                        double dij = Distance(allPoints[i], allPoints[j]);

                        matrix[i, j] = dij <= d0 ? 1 : 0;
                    }
                }
            }
            globalmatrix = matrix;
        }

        private void k近邻权重矩阵ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form inputDialog = new Form();
            inputDialog.Text = "输入阈值";
            inputDialog.StartPosition = FormStartPosition.CenterScreen;
            inputDialog.Size = new System.Drawing.Size(220, 150);

            Label lblThreshold = new Label();
            lblThreshold.Text = "请输入距离阈值d0：";
            lblThreshold.Location = new System.Drawing.Point(10, 20);
            lblThreshold.Size = new System.Drawing.Size(100, 20);
            inputDialog.Controls.Add(lblThreshold);

            TextBox txtThreshold = new TextBox();
            txtThreshold.Location = new System.Drawing.Point(110, 20);
            txtThreshold.Size = new System.Drawing.Size(70, 20);
            inputDialog.Controls.Add(txtThreshold);

            Label lblK = new Label();
            lblK.Text = "请输入k值：";
            lblK.Location = new System.Drawing.Point(10, 50);
            lblK.Size = new System.Drawing.Size(100, 20);
            inputDialog.Controls.Add(lblK);

            TextBox txtK = new TextBox();
            txtK.Location = new System.Drawing.Point(110, 50);
            txtK.Size = new System.Drawing.Size(70, 20);
            inputDialog.Controls.Add(txtK);

            Button btnOK = new Button();
            btnOK.Text = "确定";
            btnOK.Location = new System.Drawing.Point(30, 80);
            btnOK.Click += (senderBtn, eBtn) =>
            {
                if (double.TryParse(txtThreshold.Text, out double d0) && int.TryParse(txtK.Text, out int k))
                {
                    inputDialog.DialogResult = DialogResult.OK;
                    inputDialog.Close();
                }
                else
                {
                    MessageBox.Show("请输入有效的数字阈值和k值。");
                }
            };
            inputDialog.Controls.Add(btnOK);

            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new System.Drawing.Point(110, 80);
            btnCancel.Click += (senderBtn, eBtn) =>
            {
                inputDialog.DialogResult = DialogResult.Cancel;
                inputDialog.Close();
            };
            inputDialog.Controls.Add(btnCancel);
            double[,] matrix = new double[allPoints.Count, allPoints.Count];
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                double d0 = double.Parse(txtThreshold.Text);
                int k = int.Parse(txtK.Text);
               

                for (int i = 0; i < allPoints.Count; i++)
                {
                    List<double> distances = new List<double>();
                    for (int j = 0; j < allPoints.Count; j++)
                    {
                        if (i != j)
                        {
                            double dij = Distance(allPoints[i], allPoints[j]);
                            distances.Add(dij);
                        }
                    }

                    distances.Sort();
                    for (int j = 0; j < allPoints.Count; j++)
                    {
                        if (i != j)
                        {
                            double dij = Distance(allPoints[i], allPoints[j]);
                            if (distances.IndexOf(dij) < k && dij <= d0)
                            {
                                matrix[i, j] = 1 / dij;
                            }
                            else
                            {
                                matrix[i, j] = 0;
                            }
                        }
                    }
                }
            }
            globalmatrix = matrix;
        }

        private void 图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
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
