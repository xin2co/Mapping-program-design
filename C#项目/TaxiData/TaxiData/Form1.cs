using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaxiData
{
    public partial class Form1 : Form
    {
        private SessionList Data;
        public Form1()
        {
            InitializeComponent();
        }

        // 当用户点击工具栏上的“Open”按钮时调用此方法
        private void toolOpen_Click(object sender, EventArgs e)
        {
            // 显示一个打开文件对话框，让用户选择要打开的文件
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // 调用FileHelper类的Read方法，读取用户选择的文件
                // openFileDialog1.FileName是用户选择的文件的完整路径
                var epochs = FileHelper.Read("T2", openFileDialog1.FileName);

                // 使用读取的数据创建一个新的SessionList对象，并将其赋值给Data变量
                Data = new SessionList(epochs);

                // 在richTextBox1控件中显示一条消息，提示用户数据读取完成
                richTextBox1.Text = "数据读取完成！";
            }
        }

        // 当用户点击工具栏上的“Calculate”按钮时调用此方法
        private void toolCal_Click(object sender, EventArgs e)
        {
            // 将Data对象转换为字符串，并在richTextBox1控件中显示
            // 假设SessionList类重写了ToString方法，以提供数据的字符串表示
            richTextBox1.Text = Data.ToString();
        }

        // 当用户点击工具栏上的“Save”按钮时调用此方法
        private void toolSave_Click(object sender, EventArgs e)
        {
            // 显示一个保存文件对话框，让用户选择要保存的文件位置和名称
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // 调用FileHelper类的Write方法，将Data对象的数据写入到用户指定的文件
                FileHelper.Write(Data, saveFileDialog1.FileName);
            }
        }

        // 当用户点击工具栏上的“Help”按钮时调用此方法
        private void toolHelp_Click(object sender, EventArgs e)
        {
            // 创建一个包含版权信息的字符串
            string copyright = "《测绘程序设计试题集（试题1 出租车数据计算）》配套程序 作者：李英冰\n";
            copyright += "河南理工大学测绘学院\r\n卢文豪EMAIL: 2969029950@qq.com\r\n2024.4.25";

            richTextBox1.Text = copyright;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
