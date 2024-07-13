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
using static 大地线长省.Algo;

namespace 大地线长省
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        Input input = new Input();
        Algo algo = new Algo();
        MyFunctions my = new MyFunctions();
        List<Input> list = new List<Input>();
        public Form1()
        {
            InitializeComponent();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op =new OpenFileDialog();
            if(op.ShowDialog()==DialogResult.OK)
            {
                all_lines=File.ReadAllLines(op.FileName,Encoding.Default);
            }
            else
            {
                return;
            }
            try {
                string firstLine=all_lines[0];
                string[] firstLineParts=firstLine.Split(',');
                input.a=int.Parse(firstLineParts[0]);
                input.fdao=double.Parse(firstLineParts[1]);
                for(int i=2;i<all_lines.Length;i++){
                    string line =all_lines[i];
                    string []parts=line.Split(',');
                    if(parts.Length==6)
                }
            }
            catch{
            }
            tabControl1.SelectedTab = tabPage1;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Input item = list[i];
                double S = algo.Bessal_P(item);
                toolStripStatusLabel1.Text = "计算成功";
                dataGridView2.Rows.Add(i + 1, S.ToString("F3"));
            }
            tabControl1.SelectedTab = tabPage2;

        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("序号,说明,计算结果");
            foreach (var entry in GlobalVariables.Variables)
            {
                report.AppendLine(entry.Key + ','+entry.Value.ToString());
            }

            int currentLine = 25;

            for (int i = 0; i < dataGridView2.Rows.Count-1; i++)
            {
                string coefficientS = dataGridView2.Rows[i].Cells[1].Value?.ToString() ?? "数据无效";
                currentLine++;
                report.AppendFormat("{0}，第{1}条大地线系数S  , {2}\n", currentLine++, i + 1, coefficientS);
            }

            // 更新富文本框的内容
            richTextBox1.Text = report.ToString();
            tabControl1.SelectedTab = tabPage3;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();
            richTextBox1.Clear();
            list.Clear();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

