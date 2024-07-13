using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDW
{
    public partial class Form1 : Form
    {
        // 公共成员，用于存储插值结果字符串
        public string result;
        // DataEntity实例，用于存储和管理数据点
        DataEntity Data = new DataEntity();

        public Form1()
        {
            InitializeComponent();
        }

        private void toolOpen_Click(object sender, EventArgs e)
        {
            // 显示文件打开对话框，如果用户选择了一个文件，则读取数据并显示在richTextBox1中
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Data = FileHelper.Read(openFileDialog1.FileName);
                richTextBox1.Text = Data.ToString();
            }
        }

        private void toolCal_Click(object sender, EventArgs e)
        {
            // 初始化结果字符串
            string res = "点名  X(m)     Y(m)       H(m)     参与插值的点列表\r\n";
            // 创建Algo实例，用于执行IDW算法
            Algo go = new Algo(Data, 5);
            // 创建四个待插值的点
            var Q1 = new Point("Q1", 4310, 3600);
            var Q2 = new Point("Q2", 4330, 3600);
            var Q3 = new Point("Q3", 4310, 3620);
            var Q4 = new Point("Q4", 4330, 3620);

            // 对每个点执行IDW算法，并将结果添加到结果字符串中
            res += go.Idw(Q1) + "\r\n";
            res += go.Idw(Q2) + "\r\n";
            res += go.Idw(Q3) + "\r\n";
            res += go.Idw(Q4) + "\r\n";

            // 更新公共成员result和richTextBox1的文本
            result = res;
            richTextBox1.Text = res;
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileHelper.Write(result, saveFileDialog1.FileName);
            }
        }

        private void toolHelp_Click(object sender, EventArgs e)
        {
            string copyright = "《测绘程序设计试题集（试题9 反距离加权插值）》配套程序\n作者：李英冰\n";
            copyright += "河南理工大学测绘学院\r\n卢文豪EMAIL: 2969029950@qq.com\r\n2024.4.26";
            richTextBox1.Text = copyright;
        }
    }
}
