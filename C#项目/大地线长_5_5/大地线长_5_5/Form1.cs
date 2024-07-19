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
using static 大地线长_5_5.Calculation;

namespace 大地线长_5_5
{
    public partial class Form1 : Form
    {
        string[] all_lines = null;
        Input input = new Input();
        MyFunctions mf = new MyFunctions();
        Calculation ca = new Calculation();
        public static string report;
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.RowCount = 2;
            dataGridView2.RowCount = 1;
            dataGridView1[0, 0].Value = "P1";
            dataGridView1[0, 1].Value = "P2";
            input.e2 = 0.00669342162297;
            input.a = 6378245;
            input.b = 6356863;
            input.e12 = 0.00673852541468;
            克拉索夫斯基椭球ToolStripMenuItem.Checked = true;
            iUGG1975椭球ToolStripMenuItem.Checked = false;
            cGCS2000椭球ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "克拉索夫斯基椭球";
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "txt|*.txt";
            if (op.ShowDialog() == DialogResult.OK)
            {
                StreamReader Reader = new StreamReader(op.FileName);
                all_lines = File.ReadAllLines(op.FileName, Encoding.Default);
                Reader.Close();
            }
            else
            {
                return;
            }
            try
            {
                string[] temp = null;
                all_lines[0] += (" " + all_lines[1]);
                temp = all_lines[0].Split(new char[] { ' ' });
                int m = 0;
                for (int i = 0; i < temp.Length; i++)
                {
                    if (m == 0 && temp[i] != " ")
                    {
                        dataGridView1[1, 0].Value = temp[i].Trim();
                        input.B1 = mf.ddmmssTorad(double.Parse(temp[i].Trim()));
                        m++;
                        continue;
                    }
                    if (m == 1 && temp[i] != "")
                    {
                        dataGridView1[2, 0].Value = temp[i].Trim();
                        input.L1 = mf.ddmmssTorad(double.Parse(temp[i].Trim()));
                        m++;
                        continue;
                    }
                    if (m == 2 && temp[i] != "")
                    {
                        dataGridView1[1, 1].Value = temp[i].Trim();
                        input.L1 = mf.ddmmssTorad(double.Parse(temp[i].Trim()));
                        m++;
                        continue;
                    }
                    if (m == 3 && temp[i] != "")
                    {
                        dataGridView1[2, 1].Value = temp[i].Trim();
                        input.L1 = mf.ddmmssTorad(double.Parse(temp[i].Trim()));
                        m++;
                        continue;
                    }
                }
            }
            catch
            {
                MessageBox.Show("文件格式错误!");
                toolStripStatusLabel1.Text = "文件格式错误!";
                return;
            }
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sa = new SaveFileDialog();
            sa.Filter = "txt|*.txt";
            if (sa.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sa.FileName);
                try
                {
                    sw.WriteLine(reportstr());
                    sw.Close();
                    toolStripStatusLabel1.Text = "文件保存成功!";
                }
                catch
                {
                    MessageBox.Show("保存数据有误!");
                    toolStripStatusLabel1.Text = "保存出错!";
                    return;

                }
            }
        }

        private string reportstr()
        {
            string report;
            report = "                    计算报告                    \r\n";
            if (克拉索夫斯基椭球ToolStripMenuItem.Checked == true)
            {
                report += "克拉索夫斯基椭球\n\r";
            }
            if (iUGG1975椭球ToolStripMenuItem.Checked == true)
            {
                report += "iUGG1975椭球\n\r";
            }
            if (cGCS2000椭球ToolStripMenuItem.Checked == true)
            {
                report += "CGCS 2000椭球\n\r";
            }
            report += "P1点坐标(dd.mmss):" + dataGridView1[1, 0].Value.ToString() + "  " + dataGridView1[2, 0].Value.ToString() + "B/L" + "\r\n";
            report += "P2点坐标(dd.mmss):" + dataGridView1[1, 1].Value.ToString() + "  " + dataGridView1[2, 1].Value.ToString() + "B/L" + "\r\n";
            report += "--------------------结果------------------------\r\n";
            report += "大地线长（m）：" + dataGridView2[0, 0].Value.ToString() + "\r\n";
            return report;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void 克拉索夫斯基椭球ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            input.e2 = 0.00669342162297;
            input.a = 6378245;
            input.b = 6356863;
            input.e12 = 0.00673852541468;
            克拉索夫斯基椭球ToolStripMenuItem.Checked = true;
            iUGG1975椭球ToolStripMenuItem.Checked = false;
            cGCS2000椭球ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "克拉索夫斯基椭球";
        }

        private void iUGG1975椭球ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            input.e2 = 0.00669438499959;
            input.a = 6378140;
            input.b = 6356755;
            input.e12 = 0.00673950181947;
            克拉索夫斯基椭球ToolStripMenuItem.Checked = false;
            iUGG1975椭球ToolStripMenuItem.Checked = true;
            cGCS2000椭球ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "IUGG 1975椭球";
        }

        private void cGCS2000椭球ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            input.e2 = 0.00669438002290;
            input.a = 6378137;
            input.b = 6356752;
            input.e12 = 0.00673949677548;
            克拉索夫斯基椭球ToolStripMenuItem.Checked = false;
            iUGG1975椭球ToolStripMenuItem.Checked = false;
            cGCS2000椭球ToolStripMenuItem.Checked = true;
            toolStripStatusLabel2.Text = "CGCS2000 椭球";
        }

        private void 清除数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView1.RowCount = 2;
            dataGridView2.RowCount = 1;
            all_lines = null;
            input = new Input();
            克拉索夫斯基椭球ToolStripMenuItem.Checked = true;
            iUGG1975椭球ToolStripMenuItem.Checked = false;
            cGCS2000椭球ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "克拉索夫斯基椭球";
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            dataGridView1.CurrentCell = null;
            dataGridView2.CurrentCell = null;
            double S = 0;
            try
            {

                input.B1 = mf.ddmmssTorad(double.Parse(dataGridView1[1, 0].Value.ToString().Trim()));
                input.L1 = mf.ddmmssTorad(double.Parse(dataGridView1[2, 0].Value.ToString().Trim()));
                input.B2 = mf.ddmmssTorad(double.Parse(dataGridView1[1, 1].Value.ToString().Trim()));
                input.L2 = mf.ddmmssTorad(double.Parse(dataGridView1[2, 1].Value.ToString().Trim()));
                S = ca.Bessal_P(input);
                dataGridView2[0, 0].Value = S.ToString("f4");
                toolStripStatusLabel1.Text = "计算成功";
            }
            catch
            {
                MessageBox.Show("输入数据有误！");
                toolStripStatusLabel1.Text = "输入数据有误！";
                return;
            }
            report = reportstr();

        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = Form1.report;
        }
    }
}
