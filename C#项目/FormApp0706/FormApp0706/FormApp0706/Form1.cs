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

namespace FormApp0706
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

        // 保存按钮点击事件处理方法
        private void saveButton_Click(object sender, EventArgs e)
        {
            // 创建一个新的Student对象
            Student stu = new Student();
            // 从学号文本框中获取并设置学生的学号，去除前后空格
            stu.Id = Convert.ToInt32(idField.Text.Trim());
            // 从姓名文本框中获取并设置学生的姓名，去除前后空格
            stu.Name = nameField.Text.Trim();
            // 根据性别下拉列表框的选中索引判断性别，true为男性，false为女性
            stu.Sex = (sexField.SelectedIndex == 1);
            // 从手机号文本框中获取并设置学生的手机号，去除前后空格
            stu.Phone = phoneField.Text.Trim();

            // 将Student对象序列化为JSON字符串，格式化输出
            string jsonStr = JsonConvert.SerializeObject(stu, Formatting.Indented);
            // 将JSON字符串写入到名为"student.txt"的文件中，使用UTF-8编码
            AfTextFile.Write("student.txt", jsonStr, AfTextFile.UTF8);
            // 弹出消息框显示操作成功的提示
            MessageBox.Show("操作成功");
        }

        
    }
}
