using Af.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace studentsManagerSystem
{ 
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // 初始化窗体组件

            // 向性别下拉列表框中添加性别选项
            sexField.Items.Add("女");
            sexField.Items.Add("男");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //保存按钮 
        private void saveButton_Click(object sender, EventArgs e)
        {
            //创建一个新的Student对象
            Student stu = new Student();
            //从学号文件框获取并设置学生学号
            stu.Id = Convert.ToInt32(idField.Text.Trim());
            //从姓名文件框获取并设置学生姓名
            stu.Name = nameField.Text.Trim();
            //性别下拉框选择性别
            stu.Sex = (sexField.SelectedIndex == 1);
            //从手机号文件框获取并设置学生手机号
            stu.Phone = phoneField.Text.Trim();

            //将Student对象转JSON字符串
            string jsonStr = JsonConvert.SerializeObject(stu, Formatting.Indented);
            //保存
            AfTextFile.Write("student.txt", jsonStr, AfTextFile.UTF8);
            MessageBox.Show("操作成功");
        }
    }
}
