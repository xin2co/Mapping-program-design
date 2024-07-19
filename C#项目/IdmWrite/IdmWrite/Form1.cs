﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IdmWrite
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
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Data = FileHelper.Read(openFileDialog1.FileName);
                richTextBox1.Text = Data.ToString();
            }
        }

        private void toolCal_Click(object sender, EventArgs e)
        {
            string res = "点名  X(m)     Y(m)       H(m)     参与插值的点列表\r\n";
            Algo go = new Algo(Data, 5);
            var Q1 = new Point("Q1", 4310, 3600);
            var Q2 = new Point("Q2", 4330, 3600);
            var Q3 = new Point("Q3", 4310, 3620);
            var Q4 = new Point("Q4", 4330, 3620);
            res += go.Idw(Q1) + "\r\n";
            res += go.Idw(Q2) + "\r\n";
            res += go.Idw(Q3) + "\r\n";
            res += go.Idw(Q4) + "\r\n";
            result = res;
            richTextBox1.Text = res;
        }

        private void toolHelp_Click(object sender, EventArgs e)
        {
            string copyright = "《测绘程序设计试题集（试题9 反距离加权插值）》配套程序\n作者：李英冰\n";
            copyright += "河南理工大学测绘学院\r\n卢文豪EMAIL: 2969029950@qq.com\r\n2024.4.26";
            // 显示版权信息在richTextBox1中
            richTextBox1.Text = copyright;
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileHelper.Write(result, saveFileDialog1.FileName);
            }
        }
    }
}
