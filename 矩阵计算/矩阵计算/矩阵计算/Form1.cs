using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 矩阵计算
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void 矩阵转置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = Get_Matrix(richTextBox1.Text);
            var b = Get_Matrix(richTextBox2.Text);

            var res_1 = Matrix_Operator.MatrixsTransPose(a);
            Matrix_Operator.Print_Matrix(res_1, richTextBox3);
        }

        /// <summary>
        /// 从特定形式的字符串中获取矩阵
        /// </summary>
        /// <param name="s1">字符串</param>
        /// <returns>矩阵</returns>
        private Matrixs Get_Matrix(string s1)
        {
            var s = s1.Trim().Split('\n');
            Matrixs a = new Matrixs(s.Count(), s[0].Split(',').Count());
            var res = a.detail;

            for(int i = 0; i<s.Count(); i++)
            {
                var cur = s[i].Split(',');
                a.getCol = cur.Count();
                for(int j = 0; j<cur.Count(); j++)
                {
                    res[i, j] = double.Parse(cur[j].ToString());
                }
            }

            a.Name = "Inverse_of_" + a.Name;

            return a;
        }

        private void 加法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = Get_Matrix(richTextBox1.Text);
            var b = Get_Matrix(richTextBox2.Text);

            var res = Matrix_Operator.MatrixsAdd(a, b);
            Matrix_Operator.Print_Matrix(res, richTextBox3);
        }

        private void 减法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = Get_Matrix(richTextBox1.Text);
            var b = Get_Matrix(richTextBox2.Text);

            var res = Matrix_Operator.MatrixsSub(a, b);
            Matrix_Operator.Print_Matrix(res, richTextBox3);
        }

        private void 乘法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = Get_Matrix(richTextBox1.Text);
            var b = Get_Matrix(richTextBox2.Text);

            var res = Matrix_Operator.MatrixMulti(a, b);
            Matrix_Operator.Print_Matrix(res, richTextBox3);
        }

        private void 求逆矩阵ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = Get_Matrix(richTextBox1.Text);
            var b = Get_Matrix(richTextBox2.Text);

            var res = Matrix_Operator.MatrixsInverse(a);
            Matrix_Operator.Print_Matrix(res, richTextBox3);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 工具栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var st = toolStrip1.Visible;

            if (st) st = false;
            else st = true;

            toolStrip1.Visible = st;

            var s = "工具栏";
            工具栏ToolStripMenuItem.Text = s;
            if (st) 工具栏ToolStripMenuItem.Text = 工具栏ToolStripMenuItem.Text +  "（可见）";
            else 工具栏ToolStripMenuItem.Text = 工具栏ToolStripMenuItem.Text + "（不可见）";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox3.Clear();
        }
    }
}
