using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace 空间数据分析7._22
{
    public partial class 空间数据分析 : Form
    {

        private List<my_points> Points = new List<my_points>();
        private Matrixs Power_Matrix = new Matrixs();

        public 空间数据分析()
        {
            InitializeComponent();

            this.Text = "空间数据分析";
            this.tabPage1.Text = "数据";
            this.tabPage2.Text = "莫兰指数";
            this.tabPage3.Text = "图像";
            this.tabPage4.Text = "计算报告";
            this.tabPage5.Text = "Moran 全局&局部";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage1;

            Load_File();
        }

        private void Load_File()
        {
            using (OpenFileDialog file = new OpenFileDialog())
            {
                file.Filter = "文本文件(*.txt)|*.txt";
                file.Title = "文件";

                var res = file.ShowDialog();
                if(res == DialogResult.OK)
                {
                    Init_();

                    Get_data(file.FileName);

                    Show_data();

                    statues.Text = "数据文件导入成功......";
                }
            }
        }


        private void Init_()
        {
            Points.Clear();

            da.Columns.Clear();
            da.Rows.Clear();

            chart1.Series.Clear();

            re.Clear();
            m_re.Clear();
        }


        private void Get_data(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var r = sr.ReadToEnd().Trim().Split('\n');

                for(int i = 1; i<r.Count(); i++)
                {
                    my_points p = new my_points();
                    var s = r[i].Split(',');
                    p.ID = s[0];
                    p.x = double.Parse(s[1]);
                    p.y = double.Parse(s[2]);
                    p.value = double.Parse(s[3]);
                    Points.Add(p);
                }
            }
        }


        private void Show_data()
        {
            da.Columns.Clear();
            da.Rows.Clear();

            da.Columns.Add("ID", "ID");
            da.Columns.Add("X", "X");
            da.Columns.Add("Y", "Y");
            da.Columns.Add("Value", "Value");

            foreach(var p in Points)
            {
                da.Rows.Add(p.ID, p.x, p.y, p.value);
            }
        }

        private void 导入数据文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage1;

            Load_File();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage2;

            Cal_Matrix_Distance();

            statues.Text = "空间权重矩阵计算完成（已进行行标准化）......";
        }


        private void Cal_Matrix_Distance()
        {
            //初始化矩阵
            Power_Matrix.Init(Points.Count(), Points.Count());

            for(int i = 0; i<Points.Count(); i++)
            {
                for(int j = 0; j<Points.Count(); j++)
                {
                    if (i == j) continue;
                    double cur = 0;

                    cur = 1.0 / Cal_dis(Points[i], Points[j]);
                    Power_Matrix.Matrix[i][j] = cur;
                }
            }

            //行标准化
            //for(int i = 0; i<Points.Count(); i++)
            //{
            //    double ave_att = 0;
            //    for (int j = 0; j < Points.Count(); j++) ave_att += Power_Matrix.Matrix[i][j];

            //    for (int j = 0; j < Points.Count(); j++) Power_Matrix.Matrix[i][j] /= ave_att;
            //}

            //Print_Matrix(m_re, Power_Matrix);
        }

        private double Cal_dis(my_points a, my_points b)
        {
            double res = 0;
            double dx = b.x - a.x;  double dy = b.y - a.y;
            res = Math.Sqrt(dx * dx + dy * dy);
            return res;
        }


        private void Print_Matrix(RichTextBox tar, Matrixs a)
        {
            int n = a.Row;  int m = a.Col;

            for(int i = 0; i<n; i++)
            {
                for (int j = 0; j < m; j++)
                    tar.AppendText($"{a.Matrix[i][j].ToString("0.000"),-10}");
                tar.AppendText("\n");
            }

            tar.AppendText("\n");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var res = Cal_M();

            m_re.AppendText("全局莫兰指数为：" + res.ToString("0.0000000") + "\n");

            Cal_Area_M();
        }


        private double Cal_M()
        {
            double S0 = 0;
            double n = Points.Count();

            double l = 0, r = 0;
            double ave = 0;
            foreach (var p in Points) ave += p.value;
            ave /= Points.Count();

            //计算分子
            for(int i = 0; i<Points.Count(); i++)
                for(int j = 0; j<Points.Count(); j++)
                {
                    S0 += Power_Matrix.Matrix[i][j];
                    if (i == j) continue;
                    l += Power_Matrix.Matrix[i][j] * (Points[i].value - ave) * (Points[j].value - ave);
                }

            //计算分母
            for(int i = 0; i<Points.Count(); i++)
            {
                r += Math.Pow((Points[i].value - ave), 2);
            }

            double res = 0;
            res = (n / S0) * (l / r);

            return res;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage3;

            Cal_Eli();
        }


        private void Cal_Eli()
        {
            chart1.Series.Clear();

            chart1.Series.Add("Lines");
            chart1.Series.Add("Points");

            chart1.ChartAreas[0].AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.SharpTriangle;
            chart1.ChartAreas[0].AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.SharpTriangle;

            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;

            chart1.Series[0].Color = Color.Blue;
            chart1.Series[1].Color = Color.Red;

            foreach (var p in Points) chart1.Series[1].Points.AddXY(p.x, p.y);

            //计算椭圆
            double angle = 0;
            double ave_x = 0, ave_y = 0;
            double div_1 = 0, div_2 = 0, div_3 = 0;
            for (int i = 0; i < Points.Count(); i++)
            {
                ave_x += Points[i].x;
                ave_y += Points[i].y;
            }

            ave_x /= Points.Count();
            ave_y /= Points.Count();

            for(int i = 0; i<Points.Count(); i++)
            {
                div_1 += Math.Pow(Points[i].x - ave_x, 2);
                div_2 += Math.Pow(Points[i].y - ave_y, 2);
                div_3 += (Points[i].x - ave_x) * (Points[i].y - ave_y);
            }

            angle = Math.Atan2(((div_1 - div_2) + Math.Pow((div_1 - div_2)*(div_1 - div_2) + 4 * (div_3 * div_3), 0.5)), (2*div_3));

            double a = 0, b = 0;//长半轴，短半轴

            double cur_1 = 0, cur_2 = 0;
            foreach(var p in Points)
            {
                cur_1 += Math.Pow(((p.x - ave_x) * Math.Cos(angle) + (p.y - ave_y) * Math.Sin(angle)), 2);
                cur_2 += Math.Pow(((p.x - ave_x) * Math.Sin(angle) - (p.y - ave_y) * Math.Cos(angle)), 2);
            }

            a = Math.Sqrt(2 * cur_1 / Points.Count());
            b = Math.Sqrt(2 * cur_2 / Points.Count());

            Draw_Ellipse(ave_x, ave_y, a, b, angle);

            re.AppendText("椭球长半轴 a = " + a.ToString() + "\n");
            re.AppendText("椭球短半轴 b = " + b.ToString() + "\n");
            re.AppendText("平均中心点X avr_x = " + ave_x.ToString() + "\n");
            re.AppendText("平均中心点Y avr_y = " + ave_y.ToString() + "\n");
            re.AppendText("旋转角度：Angle = " + (angle).ToString() + "\n");
        }

        private void Draw_Ellipse(double x, double y, double E, double F, double max_angle)
        {
            Series ellipseSeries = new Series();
            //计算椭圆的参数
            double centerX = x;
            double centerY = y;
            //参数t的范围从0到2PI，步长可以根据需要调整
            double step = 0.01;
            for (double angle = 0; angle < 2 * Math.PI; angle += step)
            {
                my_points p = new my_points();
                p.x = x + E * Math.Cos(angle) * Math.Cos(max_angle) - F * Math.Sin(angle) * Math.Cos(max_angle);
                p.y = y + E * Math.Cos(angle) * Math.Sin(max_angle) + F * Math.Sin(angle) * Math.Sin(max_angle);
                ellipseSeries.Points.AddXY(p.x, p.y);
            }
            ellipseSeries.ChartType = SeriesChartType.Line;
            ellipseSeries.Color = Color.Blue;

            chart1.Series.Add(ellipseSeries);

        }


        //private void Draw_Ellipse(double x, double y, double a, double b, double max_angle)
        //{
        //    Series ellipseSeries = new Series();
        //    //计算椭圆的参数
        //    double centerX = x;
        //    double centerY = y;
        //    //参数t的范围从0到2PI，步长可以根据需要调整

        //    for(int i = 0; i<1000; i++)
        //    {
        //        double an = 2 * Math.PI / 1000;
        //        double x_1 = a * Math.Cos(an);
        //        double y_1 = b * Math.Sin(an);

        //        double x_r = x_1 * Math.Cos(max_angle) - y * Math.Sin(max_angle);
        //        double y_r = y_1 * Math.Sin(max_angle) + y_1 * Math.Sin(max_angle);

        //        double x_e = x_r + x;
        //        double y_e = y_r + y;

        //        ellipseSeries.Points.AddXY(x_e, y_e);
        //    }

        //    ellipseSeries.ChartType = SeriesChartType.Line;
        //    ellipseSeries.Color = Color.Blue;

        //    chart1.Series.Add(ellipseSeries);
        //}


        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void Cal_Area_M()
        {
            int n = Points.Count();

            chart2.Series.Clear();

            chart2.Series.Add("Global");
            chart2.Series.Add("All");

            chart2.Series[0].ChartType = SeriesChartType.Spline;
            chart2.Series[1].ChartType = SeriesChartType.Spline;

            chart2.ChartAreas[0].AxisX.ArrowStyle = AxisArrowStyle.SharpTriangle;
            chart2.ChartAreas[0].AxisY.ArrowStyle = AxisArrowStyle.SharpTriangle;

            chart2.Series[0].Color = Color.Blue;
            chart2.Series[1].Color = Color.Red;

            for (int i = 0; i < n; i++)
            {
                double res = 0;
                double l = 0;
                double ave = 0;
                double rell = Cal_M();

                for (int j = 0; j < Points.Count(); j++) ave += Points[j].value;
                ave /= n;

                double Si_2 = 0;
                for(int j = 0; j<n; j++)
                {
                    Si_2 += Math.Pow(Points[j].value - ave, 2);
                }
                Si_2 /= (n - 1);

                for(int j = 0; j<n; j++)
                {
                    l += Power_Matrix.Matrix[i][j] * (Points[j].value - ave);
                }

                l = l * (Points[i].value - ave);
                res = l / Si_2;

                m_re.AppendText("局部莫兰指数" + i.ToString() + ": \t" + res.ToString("0.000") + "\n");

                chart2.Series[0].Points.Add(res, i);
                chart2.Series[1].Points.Add(rell, i);
                if (i % 10 == 0)
                {
                    chart2.Series[0].Points[i].Label = res.ToString("0.000000");
                    chart2.Series[1].Points[i].Label = rell.ToString("0.000000");
                }
            }
        }

        private void 工具栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool st = toolStrip1.Visible;

            toolStrip1.Visible = st ? false : true;
        }

        private void 状态栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool st = statusStrip1.Visible;

            statusStrip1.Visible = st ? false : true;
        }

        private void 基础数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage1;
        }

        private void 标准误差椭圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage3;
        }

        private void 莫兰指数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage2;
        }

        private void 莫兰指数图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage5;
        }

        private void 计算报告哦啊ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage4;
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请查看帮助文档......");
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            tab.SelectedTab = tabPage4;

            re.AppendText(m_re.Text);

            statues.Text = "计算报告生成完成......";
        }
    }
}
