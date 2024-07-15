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
                for (int i = 0; i < all_lines.Length - 1; i++)
                {
                    string[] parts = all_lines[i].Split(',');
                    Point p = new Point()
                    {
                        id = Convert.ToInt32(parts[0]),
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

                List<Curve> curves = CalCurves(bihepoints);

                DrawCurves(curves);
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
                DrawCurves(curves);
                tabControl1.SelectedTab = tabPage2;

            }
            else
            {
                MessageBox.Show("数据不足");
                return;
            }


        }


        public List<Curve> CalCurves(List<Point> points)
        {
            List<Curve> curves = new List<Curve>();

            for (int i = 0; i < points.Count - 1; i++)
            {

                Curve curve1 = new Curve(i, points);
                Curve curve2 = new Curve(i+1, points);
                curve1.E2= 3 * (curve1.pi1.x - curve1.p.x) - curve1.R * (curve2.CosTheta + 2 * curve1.CosTheta);
                curve1.E3 = -2 * (curve1.pi1.x - curve1.p.x) + curve1.R * (curve2.CosTheta + curve1.CosTheta);
                curve1.F2 = 3 * (curve1.pi1.y - curve1.p.y) - curve1.R* (curve2.SinTheta + 2 * curve1.SinTheta);
                curve1.F3 = -2 * (curve1.pi1.y - curve1.p.y) + curve1.R * (curve2.SinTheta + curve1.SinTheta);

                curves.Add(curve1);
            }

            return curves;
        }

        private void DrawCurves(List<Curve> curves)
        {
            chart1.Series.Clear();

            Series series = new Series
            {
                ChartType = SeriesChartType.Line,
                Color = System.Drawing.Color.Red,
                BorderWidth = 5
            };

            chart1.Series.Add(series);

            foreach (Curve curve in curves)
            {
                List<Point> curvePoints = curve.GetCurvePoints(0.01);

                foreach (Point point in curvePoints)
                {
                    series.Points.AddXY(point.x, point.y);
                }
            }

            chart1.Update();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void 报告ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
