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

namespace 五点拟合_714
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        List<Point> allpoints = new List<Point>();
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
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
                for (int i = 0; i < all_lines.Length; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point p = new Point()
                    {
                        id = parts[0],
                        x = double.Parse(parts[1]),
                        y = double.Parse(parts[2])
                    };
                    allpoints.Add(p);
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2]);
                }
                toolStripLabel1.Text = "导入成功";
            }
            catch
            {
                MessageBox.Show("导入失败");
                return;
            }
        }
        private void UpdateChart(List<Point> Points, List<Curve> Curves = null)
        {

            chart1.Series.Clear();

            foreach (Point po in Points)
            {
                var series = chart1.Series.Add(po.id);
                series.ChartType = SeriesChartType.Point;
                series.Points.AddXY(po.x, po.y);
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 7;
                series.Color = Color.Red;
            }

            if (Curves != null)
            {
                var curveSeries = chart1.Series.Add("line");
                curveSeries.ChartType = SeriesChartType.Line;
                curveSeries.MarkerStyle = MarkerStyle.None;
                curveSeries.IsVisibleInLegend = false;
                curveSeries.Color = Color.Blue;

                foreach (var curve in Curves)
                {
                    for (double z = 0; z < 1; z += 0.01)
                    {
                        double x = curve.E0 + curve.E1 * z + curve.E2 * z * z + curve.E3 * z * z * z;
                        double y = curve.F0 + curve.F1 * z + curve.F2 * z * z + curve.F3 * z * z * z;
                        curveSeries.Points.AddXY(x, y);
                    }
                }
            }
        }


        private void 闭合拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Point> bihepoints = new List<Point>();
            if (allpoints.Count > 1)
            {
                bihepoints.Add(allpoints[allpoints.Count - 2]);

                bihepoints.Add(allpoints[allpoints.Count - 1]);

                bihepoints.AddRange(allpoints);

                bihepoints.Add(allpoints[0]);

                bihepoints.Add(allpoints[1]);

                bihepoints.Add(allpoints[2]);

                List<Curve> curves = CalCurves(bihepoints);

                UpdateChart(allpoints,curves);
                UpdateReport(allpoints, curves, true);
                tabControl1.SelectedTab = tabPage2;
            }
            else
            {
                MessageBox.Show("数据不足");
                return;
            }




        }

        private void 不闭合拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (allpoints.Count > 1)
            {
                List<Point> bubihepoints = new List<Point>();
                Point A = new Point();
                Point B = new Point();
                Point C = new Point();
                Point D = new Point();
                A.x = (allpoints[2].x - allpoints[1].x) - (allpoints[1].x - allpoints[0].x);
                A.y = (allpoints[2].x - allpoints[1].x) - (allpoints[1].x - allpoints[0].x);
                B.x = (allpoints[1].x - allpoints[0].x) - (allpoints[0].x - A.x);
                B.y = (allpoints[1].x - allpoints[0].x) - (allpoints[0].x - A.x);
                C.x = (allpoints[allpoints.Count - 3].x - allpoints[allpoints.Count - 2].x) - (allpoints[allpoints.Count - 2].x - allpoints[allpoints.Count - 1].x);
                C.y = (allpoints[allpoints.Count - 3].y - allpoints[allpoints.Count - 2].y) - (allpoints[allpoints.Count - 2].y - allpoints[allpoints.Count - 1].y);
                D.x = (allpoints[allpoints.Count - 2].x - allpoints[allpoints.Count - 1].x) - (allpoints[allpoints.Count - 1].x - C.x);
                D.y = (allpoints[allpoints.Count - 2].y - allpoints[allpoints.Count - 1].y) - (allpoints[allpoints.Count - 1].y - C.y);
                bubihepoints.Add(B);
                bubihepoints.Add(A);
                bubihepoints.AddRange(allpoints);
                bubihepoints.Add(C);
                bubihepoints.Add(D);
                List<Curve> curves = CalCurves(bubihepoints);
                UpdateChart(allpoints, curves);
                UpdateReport(allpoints, curves, false);
                tabControl1.SelectedTab = tabPage2;

            }
            else
            {
                MessageBox.Show("数据不足");
                return;
            }


        }


        private void UpdateReport(List<Point> myPoints, List<Curve> myCurves, bool is_close)
        {
            double x_max = double.MinValue;
            double x_min = double.MaxValue;
            double y_max = double.MinValue;
            double y_min = double.MaxValue;
            foreach (Point point in myPoints)
            {
                if (point.x < x_min) x_min = point.x;
                if (point.y < y_min) y_min = point.y;
                if (point.y > y_max) y_max = point.y;
                if (point.x > x_max) x_max = point.x;
            }

            richTextBox1.Text = ""; 
            richTextBox1.Text += "\t\t结果报告\r\n"; 
            richTextBox1.Text += "------------基本信息------------\r\n"; 
            richTextBox1.Text += "总点数:" + myPoints.Count.ToString() + "\r\n"; 
            richTextBox1.Text += "x边界:" + x_min.ToString() + "至" + x_max.ToString() + "\r\n"; 
            richTextBox1.Text += "y边界:" + y_min.ToString() + "至" + y_max.ToString() + "\r\n"; 
            if (is_close)
            {
                richTextBox1.Text += "是否闭合:是\r\n\r\n"; 
            }
            else
            {
                richTextBox1.Text += "是否闭合:否\r\n\r\n";
            }
            richTextBox1.Text += "------------计算结果------------\r\n"; 
            richTextBox1.Text += "说明:两点之间的曲线方程为:\r\n"; 
            richTextBox1.Text += "x=E0+E1*z+E2*z*z+E3*z*z*z\r\ny=F0+F1*z+F2*z*z+F3*z*z*z\r\n其中z为两点之间的弦长变量[0,1]\r\n" + "\r\n";
            richTextBox1.Text += "起点ID\t起点x\t起点y\t终点ID\t终点x\t终点y\tE0\tE1\tE2\tE3\tF0\tF1\tF2\tF3\r\n";
            foreach (Curve mycurve in myCurves) 
            {
                richTextBox1.Text += mycurve.start.id + "\t"; 
                richTextBox1.Text += mycurve.start.x.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.start.y.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.end.id + "\t";
                richTextBox1.Text += mycurve.end.x.ToString("0.000") + "\t";
                richTextBox1.Text += mycurve.end.y.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.E0.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.E1.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.E2.ToString("0.000") + "\t";
                richTextBox1.Text += mycurve.E3.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.F0.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.F1.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.F2.ToString("0.000") + "\t"; 
                richTextBox1.Text += mycurve.F3.ToString("0.000") + "\r\n\r\n"; 
            }
            richTextBox1.Text += "保留三位小数";
        }

        private double Distance(Point p1,Point p2)
        {
            double result = (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
            result = Math.Sqrt(result);
            return result;
        }

        public List<Curve> CalCurves(List<Point> points)
        {
            List<Curve> curves = new List<Curve>();
            
            for (int i = 0; i < points.Count - 5; i++)
            {
                double r = Distance(points[i + 2], points[i + 3]);
                double sin0 = Tidu(i, points)[0];
                double cos0 = Tidu(i, points)[1];
                double sin1 = Tidu(i+1, points)[0];
                double cos1 = Tidu(i+1, points)[1];
                Curve curve = new Curve
                {
                    E0 = points[i + 2].x,
                    E1 = r * cos0,
                    E2 = 3 * (points[i + 3].x - points[i + 2].x) - r * (cos1 + 2 * cos0),
                    E3 = -2 * (points[i + 3].x - points[i + 2].x) + r * (cos1 + cos0),

                    F0 = points[i + 2].y,
                    F1 = r * sin0,
                    F2 = 3 * (points[i + 3].y - points[i + 2].y) - r * (sin1 + 2 * sin0),
                    F3 = -2 * (points[i + 3].y - points[i + 2].y) + r * (sin1 + sin0),

                    start = points[i + 2],
                    end = points[i + 3]
                };
                curves.Add(curve);
            }

            return curves;
        }

        public double[]Tidu(int i, List<Point> points)
        {
            Point p1 = points[i];
            Point p2 = points[i+1];
            Point p3 = points[i+2];
            Point p4 = points[i+3];
            Point p5 = points[i+4];
            double a0, a1, a2, a3, a4, b0, b1, b2, b3, b4;
            double w2, w3;
            a1 = p2.x - p1.x;
            a2 = p3.x - p2.x;
            a3 = p4.x - p3.x;
            a4 = p5.x - p4.x;

            b1 = p2.y - p1.y;
            b2 = p3.y - p2.y;
            b3 = p4.y - p3.y;
            b4 = p5.y - p4.y;

            w2 = Math.Abs(a3 * b4 - a4 * b3);
            w3 = Math.Abs(a1 * b2 - a2 * b1);

            a0 = w2 * a2 + w3 * a3;
            b0 = w2 * b2 + w3 * b3;

            double cos = a0 / (Math.Sqrt(a0 * a0 + b0 * b0));
            double sin = b0 / (Math.Sqrt(a0 * a0 + b0 * b0));
            double[] re ={ sin,cos};
            return re;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }
        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
