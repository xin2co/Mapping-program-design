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

namespace 大地线长计算
{
    public partial class Form1 : Form
    {
        string[] all_lines = null;
        Input input = new Input();
        Output output = new Output();
        Myfunctions mf = new Myfunctions();
        Calculate ca = new Calculate();
        public static double Geodesy;
        public static string report;
        public bool call = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.RowCount = 2;
            dataGridView2.RowCount = 1;
            dataGridView1[0, 0].Value = "P" + "1";
            dataGridView1[0, 1].Value = "P" + "2";
            input.e2 = 0.006693421622966;
            input.c = 6399698.9017827110;
            Geodesy = 1;
            克拉索夫斯基椭球ToolStripMenuItem1.Checked = true;
            iUGG1975ToolStripMenuItem1.Checked = false;
            cGCS2000ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "克拉索夫斯基椭球";

        }
        public string reportstr()
        {
            string report = null;
            report = "                    计算报告                    \r\n";
            if (克拉索夫斯基椭球ToolStripMenuItem1.Checked == true)
            {
                report += "克拉索夫斯基椭球\n\r";
            }
            if (iUGG1975ToolStripMenuItem1.Checked == true)
            {
                report += "iUGG1975椭球\n\r";
            }
            if (cGCS2000ToolStripMenuItem.Checked == true)
            {
                report += "CGCS 2000椭球\n\r";
            }
            report += "P1点坐标（dd.mmsss）：" + dataGridView1[1, 0].Value.ToString() + "   " + dataGridView1[2, 0].Value.ToString() + "       B/L" + "\r\n";
            report += "P2点坐标（dd.mmss）：" + dataGridView1[1, 1].Value.ToString() + "   " + dataGridView1[2, 1].Value.ToString() + "       B/L" + "\r\n";
            report += "----------------结果--------------------\r\n";
            report += "大地线长（m）：" + dataGridView2[0, 0].Value.ToString() + "\r\n";
            return report;
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
                string[] temp = null; // 定义一个字符串数组temp，用于存储分割后的字符串
                all_lines[0] += (" " + all_lines[1]); // 将all_lines数组中的第一行和第二行数据合并为一行，中间用空格分隔
                temp = all_lines[0].Split(new char[] { ' ' }); // 使用空格分割合并后的字符串，结果存储在temp数组中
                int m = 0; // 定义一个计数器m，用于跟踪当前处理的坐标值（纬度或经度）
                for (int i = 0; i < temp.Length; i++) // 遍历temp数组中的每个元素
                {
                    if (m == 0 && temp[i] != "") // 如果m为0且当前元素不是空字符串，则认为它是第一个纬度值
                    {
                        dataGridView1[1, 0].Value = temp[i].Trim(); // 将纬度值显示在dataGridView1的第一行第二列
                        input.B1 = mf.ddmmssTorad(double.Parse(temp[i].Trim())); // 将纬度值转换为弧度，并更新input对象的B1属性
                        m++; // 计数器m加1
                        continue; // 继续下一次循环
                    }
                    if (m == 1 && temp[i] != "") // 如果m为1且当前元素不是空字符串，则认为它是第一个经度值
                    {
                        dataGridView1[2, 0].Value = temp[i].Trim(); // 将经度值显示在dataGridView1的第一行第三列
                        input.L1 = mf.ddmmssTorad(double.Parse(temp[i].Trim())); // 将经度值转换为弧度，并更新input对象的L1属性
                        m++; // 计数器m加1
                        continue; // 继续下一次循环
                    }
                    if (m == 2 && temp[i] != "") // 如果m为2且当前元素不是空字符串，则认为它是第二个纬度值
                    {
                        dataGridView1[1, 1].Value = temp[i].Trim(); // 将纬度值显示在dataGridView1的第二行第二列
                        input.B2 = mf.ddmmssTorad(double.Parse(temp[i].Trim())); // 将纬度值转换为弧度，并更新input对象的B2属性
                        m++; // 计数器m加1
                        continue; // 继续下一次循环
                    }
                    if (m == 3 && temp[i] != "") // 如果m为3且当前元素不是空字符串，则认为它是第二个经度值
                    {
                        dataGridView1[2, 1].Value = temp[i].Trim(); // 将经度值显示在dataGridView1的第二行第三列
                        input.L2 = mf.ddmmssTorad(double.Parse(temp[i].Trim())); // 将经度值转换为弧度，并更新input对象的L2属性
                        continue; // 继续下一次循环
                    }
                }
            }
            catch
            {
                MessageBox.Show("文件格式错误！");
                toolStripStatusLabel1.Text = "文件格式错误！";
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
                    toolStripStatusLabel1.Text = "文件保存成功";
                }
                catch
                {
                    MessageBox.Show("保存数据有误!请检查");
                    toolStripStatusLabel1.Text = "保存数据有误!";
                    return;
                }
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void 克拉索夫斯基椭球ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            input.e2 = 0.006693421622966;
            input.c = 6399698.9017827110;
            Geodesy = 1;
            克拉索夫斯基椭球ToolStripMenuItem1.Checked = true;
            iUGG1975ToolStripMenuItem1.Checked = false;
            cGCS2000ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "克拉索夫斯基椭球";
        }

        private void iUGG1975ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            input.e2 = 0.006694384999588;
            input.c = 6399596.6519880105;
            Geodesy = 2;
            克拉索夫斯基椭球ToolStripMenuItem1.Checked = false;
            iUGG1975ToolStripMenuItem1.Checked = true;
            cGCS2000ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "IUGG 1975椭球";
        }

        private void cGCS2000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            input.e2 = 0.00669438002290;
            input.c = 6399593.62586;
            Geodesy = 3;
            克拉索夫斯基椭球ToolStripMenuItem1.Checked = false;
            iUGG1975ToolStripMenuItem1.Checked = false;
            cGCS2000ToolStripMenuItem.Checked = true;
            toolStripStatusLabel2.Text = "CGCS2000 椭球";
        }

        private void 清空数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView1.RowCount = 2;
            dataGridView2.RowCount = 1;
            all_lines = null;
            input = new Input();
            output = new Output();
            克拉索夫斯基椭球ToolStripMenuItem1.Checked = true;
            iUGG1975ToolStripMenuItem1.Checked = false;
            cGCS2000ToolStripMenuItem.Checked = false;
            toolStripStatusLabel2.Text = "克拉索夫斯基椭球";
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            dataGridView1.CurrentCell = null;
            dataGridView2.CurrentCell = null;
            try
            {
                input.B1 = mf.ddmmssTorad(double.Parse(dataGridView1[1, 0].Value.ToString().Trim()));
                input.L1 = mf.ddmmssTorad(double.Parse(dataGridView1[2, 0].Value.ToString().Trim()));
                input.B2 = mf.ddmmssTorad(double.Parse(dataGridView1[1, 1].Value.ToString().Trim()));
                input.L2 = mf.ddmmssTorad(double.Parse(dataGridView1[2, 1].Value.ToString().Trim()));
                output = ca.Bessal_P(input, Geodesy);
                dataGridView2[0, 0].Value = output.S.ToString("f4");
                call = true;
                toolStripStatusLabel1.Text = "计算成功！";
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
