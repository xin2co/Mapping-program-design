using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

// 定义命名空间CurveFit
namespace fitceshi
{
    // 定义Form1类，继承自Form类
    public partial class Form1 : Form
    {
        // 定义全局变量
        public List<MyPoint> global_mypoint_list = new List<MyPoint>(); // 全局点集合
        public List<MyCurve> global_mycurve_list = new List<MyCurve>(); // 全局曲线集合

        // Form1的构造函数
        public Form1()
        {
            InitializeComponent(); // 初始化组件
        }

        // 文件打开按钮的点击事件处理函数
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string file_path = openFileDialog(); // 调用openFileDialog函数获取文件路径
            try
            {
                global_mypoint_list = readPointFile(file_path); // 读取文件内容到全局点集合
            }
            catch
            {
                MessageBox.Show("文件有误!"); // 如果读取异常，显示错误信息
                return;
            }
            updateTable(global_mypoint_list); // 更新表格显示
            updateChart(global_mypoint_list); // 更新图表显示
        }

        // 更新表格显示的函数
        public void updateTable(List<MyPoint> mypoint_list)
        {
            DataTable table = new DataTable(); // 创建DataTable对象
            table.Columns.Add("ID", Type.GetType("System.String")); // 添加ID列
            table.Columns.Add("x", Type.GetType("System.String")); // 添加x列
            table.Columns.Add("y", Type.GetType("System.String")); // 添加y列
            foreach (MyPoint po in mypoint_list) // 遍历点集合
            {
                DataRow row = table.NewRow(); // 创建新行
                row["ID"] = po.ID; // 设置ID列
                row["x"] = po.x.ToString(); // 设置x列
                row["y"] = po.y.ToString(); // 设置y列
                table.Rows.Add(row); // 添加行到表格
            }
            dataGridView1.DataSource = table; // 设置dataGridView的DataSource
        }

        // 更新图表显示的函数
        public void updateChart(List<MyPoint> mypoint_list, List<MyCurve> mycurve_list = null)
        {
            chart1.Titles.Clear(); // 清除标题
            chart1.ChartAreas[0].AxisX.Title = "曲线拟合"; // 设置X轴标题
            chart1.ChartAreas[0].AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle; // 设置X轴箭头样式
            chart1.ChartAreas[0].AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle; // 设置Y轴箭头样式
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent; // 设置X轴网格颜色透明
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Transparent; // 设置Y轴网格颜色透明
            int count = chart1.Series.Count; // 获取当前图表的Series数量
            for (int j = 0; j < count; j++) // 遍历并移除所有Series
            {
                chart1.Series.RemoveAt(0);
            }
            int i = 0;
            foreach (MyPoint po in mypoint_list) // 遍历点集合
            {
                chart1.Series.Add(po.ID); // 添加Series
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point; // 设置为点类型
                chart1.Series[i].Points.Clear(); // 清除Points
                chart1.Series[i].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle; // 设置标记样式
                chart1.Series[i].Points.AddXY(po.x, po.y); // 添加点
                chart1.Series[i].MarkerSize = 7; // 设置标记大小
                chart1.Series[i].Label = i.ToString(); // 设置标签
                chart1.Series[i].IsVisibleInLegend = false; // 设置图例不可见
                chart1.Series[i].Color = Color.Red; // 设置颜色
                i++;
            }
            if (mycurve_list != null)
            {
                chart1.Series.Add("line");
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[i].Points.Clear();
                chart1.Series[i].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                chart1.Series[i].IsVisibleInLegend = false;
                chart1.Series[i].Color = Color.Blue;
                foreach (MyCurve mycurve in mycurve_list) // 遍历曲线集合
                {
                    double z = 0; // 初始化z变量
                    while (z < 1) // 当z小于1时循环
                    {
                        double x = mycurve.p0 + mycurve.p1 * z + mycurve.p2 * z * z + mycurve.p3 * z * z * z; // 根据z计算x值
                        double y = mycurve.q0 + mycurve.q1 * z + mycurve.q2 * z * z + mycurve.q3 * z * z * z; // 根据z计算y值
                        chart1.Series[i].Points.AddXY(x, y); // 添加点到曲线Series
                        z += 0.01; // z增加0.01
                    }
                }
            }
        }

        // 文件选择对话框函数
        public string openFileDialog()
        {
            string file_path = ""; // 初始化文件路径为空
            OpenFileDialog op = new OpenFileDialog(); // 创建OpenFileDialog对象
            op.Filter = "文件(*.txt)|*.txt"; // 设置文件过滤
            op.ShowDialog(); // 显示对话框
            file_path = op.FileName; // 获取文件路径
            return file_path; // 返回文件路径
        }

        // 读取点文件的函数
        public List<MyPoint> readPointFile(string file_path)
        {
            List<MyPoint> mypoint_list = new List<MyPoint>(); // 创建点集合
            StreamReader sr = new StreamReader(file_path); // 创建StreamReader对象
            while (!sr.EndOfStream) // 当未到文件末尾时循环
            {
                string text = sr.ReadLine(); // 读取一行文本
                string[] str_split = text.Split(','); // 以逗号分割文本
                mypoint_list.Add(new MyPoint(str_split[0], double.Parse(str_split[1]), double.Parse(str_split[2]))); // 添加点对象到集合
            }
            return mypoint_list; // 返回点集合
        }

        // 闭合拟合按钮的点击事件处理函数
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // global_mycurve_list = PointToCurve.builtCurve(global_mypoint_list, false);
            global_mycurve_list = PointToCurve.builtCurve(global_mypoint_list, true); // 调用builtCurve函数进行闭合拟合
            updateChart(global_mypoint_list, global_mycurve_list); // 更新图表显示
            updateReport(global_mypoint_list, global_mycurve_list, true); // 更新报告显示
        }

        // 更新报告显示的函数
        public void updateReport(List<MyPoint> mypoint_list, List<MyCurve> mycurve_list, bool is_close)
        {
            double x_min = 0, x_max = 0, y_min = 0, y_max = 0; // 初始化边界变量
            getBorder(mypoint_list, ref x_min, ref x_max, ref y_min, ref y_max); // 获取边界值
            textBox1.Text = ""; // 清空textBox
            textBox1.Text += "\t\t结果报告\r\n"; // 添加报告标题
            textBox1.Text += "------------基本信息------------\r\n"; // 添加基本信息标题
            textBox1.Text += "总点数:" + mypoint_list.Count.ToString() + "\r\n"; // 添加点数信息
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
            textBox1.Text += "x=p0+p1*z+p2*z*z+p3*z*z*z\r\ny=q0+q1*z+q2*z*z+q3*z*z*z\r\n其中z为两点之间的弦长变量[0,1]\r\n" + "\r\n";
            textBox1.Text += "起点ID\t起点x\t起点y\t终点ID\t终点x\t终点y\tp0\tp1\tp2\tp3\tq0\tq1\tq2\tq3\r\n";
            foreach (MyCurve mycurve in mycurve_list) // 遍历曲线集合
            {
                textBox1.Text += mycurve.mypoint_start.ID + "\t"; // 添加起点ID
                textBox1.Text += mycurve.mypoint_start.x.ToString("0.000") + "\t"; // 添加起点x
                textBox1.Text += mycurve.mypoint_start.y.ToString("0.000") + "\t"; // 添加起点y
                textBox1.Text += mycurve.mypoint_end.ID + "\t"; // 添加终点ID
                textBox1.Text += mycurve.mypoint_end.x.ToString("0.000") + "\t"; // 添加终点x
                textBox1.Text += mycurve.mypoint_end.y.ToString("0.000") + "\t"; // 添加终点y
                textBox1.Text += mycurve.p0.ToString("0.000") + "\t"; // 添加p0参数
                textBox1.Text += mycurve.p1.ToString("0.000") + "\t"; // 添加p1参数
                textBox1.Text += mycurve.p2.ToString("0.000") + "\t"; // 添加p2参数
                textBox1.Text += mycurve.p3.ToString("0.000") + "\t"; // 添加p3参数
                textBox1.Text += mycurve.q0.ToString("0.000") + "\t"; // 添加q0参数
                textBox1.Text += mycurve.q1.ToString("0.000") + "\t"; // 添加q1参数
                textBox1.Text += mycurve.q2.ToString("0.000") + "\t"; // 添加q2参数
                textBox1.Text += mycurve.q3.ToString("0.000") + "\r\n\r\n"; // 添加q3参数
            }
            textBox1.Text += "保留三位小数"; // 添加保留小数位数说明
        }

        // 获取边界值的函数
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

        // 退出按钮的点击事件处理函数
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0); // 退出程序
        }

        // 不闭合拟合按钮的点击事件处理函数
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            global_mycurve_list = PointToCurve.builtCurve(global_mypoint_list, false); // 调用builtCurve函数进行不闭合拟合
            updateChart(global_mypoint_list, global_mycurve_list); // 更新图表显示
            updateReport(global_mypoint_list, global_mycurve_list, false); // 更新报告显示
        }

        // 保存图形按钮的点击事件处理函数
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string file_path = saveFileDialog("dxf"); // 调用saveFileDialog获取文件路径
            if (file_path == "") return; // 如果文件路径为空，则返回
            if (global_mycurve_list.Count == 0) // 如果曲线数量为0
            {
                MessageBox.Show("不存在可保存的曲线!"); // 显示提示信息
                return;
            }
            StreamWriter sw = new StreamWriter(file_path, false); // 创建StreamWriter对象
            sw.Write(generateDxf(global_mypoint_list, global_mycurve_list)); // 写入DXF文件内容
            sw.Close(); // 关闭StreamWriter对象
        }
        // 生成DXF文件内容的函数
        public string generateDxf(List<MyPoint> mypoint_list, List<MyCurve> mycurve_list)
        {
            string str_dxf = ""; // 初始化DXF字符串为空
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
                    double x = mycurve.p0 + mycurve.p1 * z + mycurve.p2 * z * z + mycurve.p3 * z * z * z;
                    double y = mycurve.q0 + mycurve.q1 * z + mycurve.q2 * z * z + mycurve.q3 * z * z * z;
                    double x_next = mycurve.p0 + mycurve.p1 * z_next + mycurve.p2 * z_next * z_next + mycurve.p3 * z_next * z_next * z_next;
                    double y_next = mycurve.q0 + mycurve.q1 * z_next + mycurve.q2 * z_next * z_next + mycurve.q3 * z_next * z_next * z_next;
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

        // 保存文件对话框函数
        public string saveFileDialog(string type)
        {
            string localFilePath = ""; // 初始化文件路径为空
            SaveFileDialog sfd = new SaveFileDialog(); // 创建SaveFileDialog对象
            // 设置文件类型 
            sfd.Filter = "文件（*." + type + " ）|*." + type;
            // 设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;
            // 保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;
            // 点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                localFilePath = sfd.FileName.ToString(); // 获取文件路径
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); // 获取文件名，不带路径
            }
            return localFilePath; // 返回文件路径
        }

        // 保存报告按钮的点击事件处理函数
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string file_path = saveFileDialog("txt"); // 调用saveFileDialog获取文件路径
            if (file_path == "") return; // 如果文件路径为空，则返回
            if (textBox1.Text == "") // 如果报告内容为空
            {
                MessageBox.Show("不存在可保存的报告!"); // 显示提示信息
                return;
            }
            StreamWriter sw = new StreamWriter(file_path, false); // 创建StreamWriter对象
            sw.Write(textBox1.Text); // 写入报告内容
            sw.Close(); // 关闭StreamWriter对象
        }
        // 打开点文件菜单项的点击事件处理函数
        private void 打开点txt文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file_path = openFileDialog(); // 调用openFileDialog函数获取文件路径
            try
            {
                global_mypoint_list = readPointFile(file_path); // 读取文件内容到全局点集合
            }
            catch
            {
                MessageBox.Show("文件有误!"); // 如果读取异常，显示错误信息
                return;
            }
            updateTable(global_mypoint_list); // 更新表格显示
            updateChart(global_mypoint_list); // 更新图表显示
        }

        // 保存报告文件菜单项的点击事件处理函数
        private void 保存报告txt文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file_path = saveFileDialog("txt"); // 调用saveFileDialog获取文件路径
            if (file_path == "") return; // 如果文件路径为空，则返回
            if (textBox1.Text == "") // 如果报告内容为空
            {
                MessageBox.Show("不存在可保存的报告!"); // 显示提示信息
                return;
            }
            StreamWriter sw = new StreamWriter(file_path, false); // 创建StreamWriter对象
            sw.Write(textBox1.Text); // 写入报告内容
            sw.Close(); // 关闭StreamWriter对象
        }

        // 保存图形文件菜单项的点击事件处理函数
        private void 保存图形dxf文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file_path = saveFileDialog("dxf"); // 调用saveFileDialog获取文件路径
            if (file_path == "") return; // 如果文件路径为空，则返回
            if (global_mycurve_list.Count == 0) // 如果曲线数量为0
            {
                MessageBox.Show("不存在可保存的曲线!"); // 显示提示信息
                return;
            }
            StreamWriter sw = new StreamWriter(file_path, false); // 创建StreamWriter对象
            sw.Write(generateDxf(global_mypoint_list, global_mycurve_list)); // 写入DXF文件内容
            sw.Close(); // 关闭StreamWriter对象
        }

        // 闭合拟合菜单项的点击事件处理函数
        private void 闭合拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            global_mycurve_list = PointToCurve.builtCurve(global_mypoint_list, true); // 调用builtCurve函数进行闭合拟合
            updateChart(global_mypoint_list, global_mycurve_list); // 更新图表显示
            updateReport(global_mypoint_list, global_mycurve_list, true); // 更新报告显示
        }

        // 不闭合拟合菜单项的点击事件处理函数
        private void 不闭合拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            global_mycurve_list = PointToCurve.builtCurve(global_mypoint_list, false); // 调用builtCurve函数进行不闭合拟合
            updateChart(global_mypoint_list, global_mycurve_list); // 更新图表显示
            updateReport(global_mypoint_list, global_mycurve_list, false); // 更新报告显示
        }

        // 原始点数据菜单项的点击事件处理函数
        private void 原始点数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0; // 设置tabControl选择原始点数据页
        }

        // 图形界面菜单项的点击事件处理函数
        private void 图形界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1; // 设置tabControl选择图形界面页
        }

        // 计算报告菜单项的点击事件处理函数
        private void 计算报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2; // 设置tabControl选择计算报告页
        }

        // 退出按钮的点击事件处理函数
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0); // 退出程序
        }

        // 缩小按钮的点击事件处理函数
        private void toolStripButton7_Click(object sender, EventArgs e)
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

        // 放大按钮的点击事件处理函数
        private void toolStripButton8_Click(object sender, EventArgs e)
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
        private void tabPage1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab("TabPage1");
        }

        private void tabPage2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab("TabPage2");
        }

        private void tabPage3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab("TabPage3");
        }

    }
}