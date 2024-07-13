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

namespace 五点拟合_5_20
{
    public partial class Form1 : Form
    {
        public List<MyPoint> myPoints = new List<MyPoint>();
        public List<MyCurve> myCurves = new List<MyCurve>();
        public Form1()
        {
            InitializeComponent();
        }

        private void 打开点txt文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string file_path = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file_path = openFileDialog.FileName;
            }

            try
            {
                myPoints = readPointFile(file_path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件读取错误: " + ex.Message);
                return;
            }
            updateTable(myPoints);
            updateChart(myPoints);
        }

        private void updateChart(List<MyPoint> myPoints, List<MyCurve> myCurves = null)
        {
            chart1.Titles.Clear();

            // 设置图表区域
            chart1.ChartAreas[0].AxisX.Title = "曲线拟合";
            chart1.ChartAreas[0].AxisX.ArrowStyle = AxisArrowStyle.Triangle;
            chart1.ChartAreas[0].AxisY.ArrowStyle = AxisArrowStyle.Triangle;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Transparent;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Transparent;

            // 清理现有系列
            chart1.Series.Clear();

            // 添加点系列
            foreach (var po in myPoints)
            {
                var series = chart1.Series.Add(po.ID);
                series.ChartType = SeriesChartType.Point;
                series.Points.AddXY(po.x, po.y);
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 7;
                series.Color = Color.Red;
            }

            if (myCurves != null)
            {
                var curveSeries = chart1.Series.Add("line");
                curveSeries.ChartType = SeriesChartType.Line;
                curveSeries.MarkerStyle = MarkerStyle.None;
                curveSeries.IsVisibleInLegend = false;
                curveSeries.Color = Color.Blue;

                foreach (var mycurve in myCurves)
                {
                    for (double z = 0; z < 1; z += 0.01)
                    {
                        double x = mycurve.E0 + mycurve.E1 * z + mycurve.E2 * z * z + mycurve.E3 * z * z * z;
                        double y = mycurve.F0 + mycurve.F1 * z + mycurve.F2 * z * z + mycurve.F3 * z * z * z;
                        // 直接添加X和Y值
                        curveSeries.Points.AddXY(x, y);
                    }
                }
            }
        }

        private void updateTable(List<MyPoint> myPoints)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("ID", "ID");
            dataGridView1.Columns.Add("x", "x");
            dataGridView1.Columns.Add("y", "y");

            foreach (MyPoint po in myPoints)
            {
                int rowIndex = dataGridView1.Rows.Add(); // 添加一个新行
                dataGridView1.Rows[rowIndex].Cells["ID"].Value = po.ID;
                dataGridView1.Rows[rowIndex].Cells["x"].Value = po.x;
                dataGridView1.Rows[rowIndex].Cells["y"].Value = po.y;
            }
        }

        private List<MyPoint> readPointFile(string file_path)
        {
            List<MyPoint> mypoint_list = new List<MyPoint>();
            StreamReader sr = new StreamReader(file_path);
            while (!sr.EndOfStream)
            {
                string text = sr.ReadLine();
                string[] str_split = text.Split(',');
                mypoint_list.Add(new MyPoint(str_split[0], double.Parse(str_split[1]), double.Parse(str_split[2])));
            }
            return mypoint_list;
        }

        private void 闭合toolStripButton2_Click(object sender, EventArgs e)
        {
            if (myPoints == null || myPoints.Count == 0)
            {
                MessageBox.Show("没有足够的点来构建曲线!");
                return;
            }
            myCurves = PointToCurve.builtCurve(myPoints, true);

            updateChart(myPoints, myCurves);
            updateReport(myPoints, myCurves, true);
            tabControl1.SelectedIndex = 1;
        }

        private void 不闭合toolStripButton3_Click(object sender, EventArgs e)
        {
            if (myPoints == null || myPoints.Count == 0)
            {
                MessageBox.Show("没有足够的点来构建曲线!");
                return;
            }
            myCurves = PointToCurve.builtCurve(myPoints, false);

            updateChart(myPoints, myCurves);
            updateReport(myPoints, myCurves, false);
            tabControl1.SelectedIndex = 1;
        }

        private void updateReport(List<MyPoint> myPoints, List<MyCurve> myCurves, bool is_close)
        {
            double x_min = 0, x_max = 0, y_min = 0, y_max = 0; // 初始化边界变量
            getBorder(myPoints, ref x_min, ref x_max, ref y_min, ref y_max); // 获取边界值
            textBox1.Text = ""; // 清空textBox
            textBox1.Text += "\t\t结果报告\r\n"; // 添加报告标题
            textBox1.Text += "------------基本信息------------\r\n"; // 添加基本信息标题
            textBox1.Text += "总点数:" + myPoints.Count.ToString() + "\r\n"; // 添加点数信息
            textBox1.Text += "x边界:" + x_min.ToString() + "至" + x_max.ToString() + "\r\n"; // 添加x边界信息
            textBox1.Text += "y边界:" + y_min.ToString() + "至" + y_max.ToString() + "\r\n"; // 添加y边界信息
            if (is_close)
            {
                textBox1.Text += "是否闭合:是\r\n\r\n"; // 添加闭合信息
            }
            else
            {
                textBox1.Text += "是否闭合:否\r\n\r\n"; // 添加不闭合信息
            }
            textBox1.Text += "------------计算结果------------\r\n"; // 添加计算结果标题
            textBox1.Text += "说明:两点之间的曲线方程为:\r\n"; // 添加曲线方程说明
            textBox1.Text += "x=E0+E1*z+E2*z*z+E3*z*z*z\r\ny=F0+F1*z+F2*z*z+F3*z*z*z\r\n其中z为两点之间的弦长变量[0,1]\r\n" + "\r\n";
            textBox1.Text += "起点ID\t起点x\t起点y\t终点ID\t终点x\t终点y\tE0\tE1\tE2\tE3\tF0\tF1\tF2\tF3\r\n";
            foreach (MyCurve mycurve in myCurves) // 遍历曲线集合
            {
                textBox1.Text += mycurve.mypoint_start.ID + "\t"; // 添加起点ID
                textBox1.Text += mycurve.mypoint_start.x.ToString("0.000") + "\t"; // 添加起点x
                textBox1.Text += mycurve.mypoint_start.y.ToString("0.000") + "\t"; // 添加起点y
                textBox1.Text += mycurve.mypoint_end.ID + "\t"; // 添加终点ID
                textBox1.Text += mycurve.mypoint_end.x.ToString("0.000") + "\t"; // 添加终点x
                textBox1.Text += mycurve.mypoint_end.y.ToString("0.000") + "\t"; // 添加终点y
                textBox1.Text += mycurve.E0.ToString("0.000") + "\t"; // 添加E0参数
                textBox1.Text += mycurve.E1.ToString("0.000") + "\t"; // 添加E1参数
                textBox1.Text += mycurve.E2.ToString("0.000") + "\t"; // 添加E2参数
                textBox1.Text += mycurve.E3.ToString("0.000") + "\t"; // 添加E3参数
                textBox1.Text += mycurve.F0.ToString("0.000") + "\t"; // 添加F0参数
                textBox1.Text += mycurve.F1.ToString("0.000") + "\t"; // 添加F1参数
                textBox1.Text += mycurve.F2.ToString("0.000") + "\t"; // 添加F2参数
                textBox1.Text += mycurve.F3.ToString("0.000") + "\r\n\r\n"; // 添加F3参数
            }
            textBox1.Text += "保留三位小数"; // 添加保留小数位数说明
        }

        public void getBorder(List<MyPoint> mypoint_list, ref double x_min, ref double x_max,
       ref double y_min, ref double y_max)
        {
            x_max = double.MinValue; // 初始化x_max为最小值
            x_min = double.MaxValue; // 初始化x_min为最大值
            y_max = double.MinValue; // 初始化y_max为最小值
            y_min = double.MaxValue; // 初始化y_min为最大值
            foreach (MyPoint point in mypoint_list) // 遍历点集合
            {
                if (point.x < x_min) x_min = point.x; // 更新x_min
                if (point.y < y_min) y_min = point.y; // 更新y_min
                if (point.y > y_max) y_max = point.y; // 更新y_max
                if (point.x > x_max) x_max = point.x; // 更新x_max
            }
        }

        private void 保存图形toolStripButton6_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DXF 文件 (*.dxf)|*.dxf";
            saveFileDialog.Title = "保存 DXF 文件";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file_path = saveFileDialog.FileName;

                if (myCurves.Count == 0)
                {
                    MessageBox.Show("不存在可保存的曲线！");
                    return;
                }

                try
                {
                    using (StreamWriter sw = File.CreateText(file_path))
                    {
                        sw.Write(generateDxf(myPoints, myCurves));
                    }
                    MessageBox.Show("DXF文件保存成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存文件时出错：" + ex.Message);
                }
            }





        }

        public string generateDxf(List<MyPoint> mypoint_list, List<MyCurve> mycurve_list)
        {
            string str_dxf = "";
            str_dxf += "  0\r\n";
            str_dxf += "SECTION\r\n";
            str_dxf += "  2\r\n";
            str_dxf += "ENTITIES\r\n";
            // 点
            for (int i = 0; i < mypoint_list.Count; i++)
            {
                str_dxf += "  0\r\n";
                str_dxf += "POINT\r\n";
                str_dxf += "  8\r\n";
                str_dxf += "0\r\n";
                str_dxf += " 10\r\n";
                str_dxf += mypoint_list[i].x.ToString() + "\r\n";
                str_dxf += " 20\r\n";
                str_dxf += mypoint_list[i].y.ToString() + "\r\n";
            }
            // 线
            foreach (MyCurve mycurve in mycurve_list)
            {
                double z = 0;
                double z_next = 0.01;
                while (z < 1)
                {
                    double x = mycurve.E0 + mycurve.E1 * z + mycurve.E2 * z * z + mycurve.E3 * z * z * z;
                    double y = mycurve.F0 + mycurve.F1 * z + mycurve.F2 * z * z + mycurve.F3 * z * z * z;
                    double x_next = mycurve.E0 + mycurve.E1 * z_next + mycurve.E2 * z_next * z_next + mycurve.E3 * z_next * z_next * z_next;
                    double y_next = mycurve.F0 + mycurve.F1 * z_next + mycurve.F2 * z_next * z_next + mycurve.F3 * z_next * z_next * z_next;
                    str_dxf += "  0\r\n";
                    str_dxf += "LINE\r\n";
                    str_dxf += "  8\r\n";
                    str_dxf += "0\r\n";
                    str_dxf += " 10\r\n";
                    str_dxf += x.ToString() + "\r\n";
                    str_dxf += " 20\r\n";
                    str_dxf += y.ToString() + "\r\n";
                    str_dxf += " 11\r\n";
                    str_dxf += x_next.ToString() + "\r\n";
                    str_dxf += " 21\r\n";
                    str_dxf += y_next.ToString() + "\r\n";
                    z_next += 0.01;
                    z += 0.01;
                }
            }
            str_dxf += "  0\r\n";
            str_dxf += "ENDSEC\r\n";
            str_dxf += "  0\r\n";
            str_dxf += "EOF\r\n";
            return str_dxf; // 返回DXF字符串
        }

        private void 保存报告toolStripButton7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("报告内容为空，无法保存!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                DefaultExt = "txt",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file_path = saveFileDialog.FileName;
                File.WriteAllText(file_path, textBox1.Text);
            }
        }

        private void 缩小toolStripButton7_Click(object sender, EventArgs e)
        {
            AdjustChartScale(chart1, 0.8);
        }

        private void 放大toolStripButton8_Click(object sender, EventArgs e)
        {
            AdjustChartScale(chart1, 1.2);
        }

        private void AdjustChartScale(Chart chart, double scale)
        {
            double width = chart.ChartAreas[0].AxisX.Maximum - chart.ChartAreas[0].AxisX.Minimum;
            double height = chart.ChartAreas[0].AxisY.Maximum - chart.ChartAreas[0].AxisY.Minimum;
            double x_cen = chart.ChartAreas[0].AxisX.Maximum - width / 2;
            double y_cen = chart.ChartAreas[0].AxisY.Maximum - height / 2;

            chart.ChartAreas[0].AxisX.Maximum = x_cen + width / 2 * scale;
            chart.ChartAreas[0].AxisX.Minimum = x_cen - width / 2 * scale;
            chart.ChartAreas[0].AxisY.Maximum = y_cen + height / 2 * scale;
            chart.ChartAreas[0].AxisY.Minimum = y_cen - height / 2 * scale;
        }

        private void 原始点数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void 图形界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void 计算报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0); // 退出程序
        }

    }
}
