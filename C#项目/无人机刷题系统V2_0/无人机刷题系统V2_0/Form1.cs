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

namespace 无人机刷题系统V3_0
{
    public partial class Form1 : Form
    {
        private List<Input> questions; // 声明一个私有变量来存储问题列表
        private Mark mark;
        private int currentQuestionIndex = -1;
        private int questionModel = 0;
        private string defaultFilePath = "itemBank.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 默认的文件路径，假设 itemBank.txt 文件位于应用程序的当前目录
            string defaultFilePath = "itemBank.txt";

            // 检查文件是否存在
            if (File.Exists(defaultFilePath))
            {
                // 创建 fileHelper 实例
                FileHelper fileHelper = new FileHelper();

                // 读取文件并获取题目列表
                questions = fileHelper.ReadQuestionsFromFile(defaultFilePath);

                // 检查是否有问题存在
                if (questions.Any())
                {
                    // 获取题库中的第一行的数字，作为上次刷题的题号
                    string firstLine = File.ReadLines(defaultFilePath).First();
                    int lastQuestionNumber = int.Parse(firstLine);

                    // 根据题号获取对应的问题
                    currentQuestionIndex = lastQuestionNumber - 1; // 由于索引从0开始，所以减1
                    if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
                    {
                        // 显示对应的问题
                        richTextBox1.Text = questions[currentQuestionIndex].Question;
                        // 更新状态栏显示当前题的序号
                        toolStripStatusLabel3.Text = $"当前题号: {lastQuestionNumber}";
                    }
                    else
                    {
                        MessageBox.Show("题库中的题号超出范围。");
                    }
                }
                else
                {
                    // 如果没有问题，显示消息
                    MessageBox.Show("题库中没有问题。");
                }
            }
            else
            {
                // 文件不存在，可以显示错误消息或进行其他处理
                MessageBox.Show("默认题目文件不存在。");
            }
        }


        private void button_save_Click(object sender, EventArgs e)
        {
            // 创建 StreamWriter 实例来写入文件
            using (StreamWriter writer = new StreamWriter("收藏题目.txt"))
            {
                // 获取当前问题的题目和答案
                string question = richTextBox1.Text;
                string answer = toolStripStatusLabel2.Text;

                // 写入题目和答案，确保题目和答案之间有适当的分隔
                writer.WriteLine(question);
                writer.WriteLine("答案: " + answer);
                writer.WriteLine(Environment.NewLine); // 添加空行作为分隔
            }

            // 提示用户操作成功
            toolStripStatusLabel2.Text = "已收藏！";
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // 创建 OpenFileDialog 实例
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
            openFileDialog.Title = "选择题目文件";

            // 如果用户选择了文件并点击了“打开”
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件路径
                string filePath = openFileDialog.FileName;

                // 创建 fileHelper 实例
                FileHelper fileHelper = new FileHelper();

                // 读取文件并获取题目列表
                questions = fileHelper.ReadQuestionsFromFile(filePath);

                // 显示新文件中的第一个问题的题目
                if (questions.Any())
                {
                    richTextBox1.AppendText(questions.First().Question + Environment.NewLine);
                }
            }
        }

        private void radioButton_C_CheckedChanged(object sender, EventArgs e)
        {
            // 获取当前选中的 RadioButton
            RadioButton radioButton = sender as RadioButton;
            if (questionModel == 0)
            {
                if (radioButton.Checked)
                {
                    // 获取当前问题的答案
                    char answer = questions[currentQuestionIndex].Answer;

                    // 检查答案是否正确
                    if (answer == 'C')
                    {
                        toolStripStatusLabel2.Text = "答对了！";
                        toolStripStatusLabel2.ForeColor = System.Drawing.Color.Green;
                        // 更新标记
                        mark.UpdateMark(true);
                    }
                    else
                    {
                        toolStripStatusLabel2.Text = "正确答案为: " + answer;
                        toolStripStatusLabel2.ForeColor = System.Drawing.Color.Red;
                        // 更新标记
                        mark.UpdateMark(false);
                    }

                    // 重置其他 RadioButton 为未选中状态
                    radioButton_B.Checked = false;
                    radioButton_A.Checked = false;
                }
            }
            else
            {
                char answer = questions.First().Answer;
                if (answer == 'C')
                {
                    mark.CorrectCount++;
                }
                else
                {
                    // 创建 StreamWriter 实例来写入文件
                    string wrongAnswersFilePath = "examWrongAnswers.txt";
                    using (StreamWriter writer = new StreamWriter(wrongAnswersFilePath))
                    {
                        // 写入题目和答案，确保题目和答案之间有适当的分隔
                        writer.WriteLine(questions.First().Question);
                        writer.WriteLine("答案: " + questions.First().Answer);
                        writer.WriteLine(Environment.NewLine); // 添加空行作为分隔
                    }
                }
            }
        }

        private void radioButton_B_CheckedChanged(object sender, EventArgs e)
        {
            // 获取当前选中的 RadioButton
            RadioButton radioButton = sender as RadioButton;
            if (questionModel == 0)
            {
                if (radioButton.Checked)
                {
                    // 获取当前问题的答案
                    char answer = questions[currentQuestionIndex].Answer;

                    // 检查答案是否正确
                    if (answer == 'B')
                    {
                        toolStripStatusLabel2.Text = "答对了！";
                        toolStripStatusLabel2.ForeColor = System.Drawing.Color.Green;
                        // 更新标记
                        mark.UpdateMark(true);
                    }
                    else
                    {
                        toolStripStatusLabel2.Text = "正确答案为: " + answer;
                        toolStripStatusLabel2.ForeColor = System.Drawing.Color.Red;
                        // 更新标记
                        mark.UpdateMark(false);
                    }

                    // 重置其他 RadioButton 为未选中状态
                    radioButton_A.Checked = false;
                    radioButton_C.Checked = false;
                }
            }
            else
            {
                char answer = questions.First().Answer;
                if (answer == 'B')
                {
                    mark.CorrectCount++;
                }
                else
                {
                    // 创建 StreamWriter 实例来写入文件
                    string wrongAnswersFilePath = "examWrongAnswers.txt";
                    using (StreamWriter writer = new StreamWriter(wrongAnswersFilePath))
                    {
                        // 写入题目和答案，确保题目和答案之间有适当的分隔
                        writer.WriteLine(questions.First().Question);
                        writer.WriteLine("答案: " + questions.First().Answer);
                        writer.WriteLine(Environment.NewLine); // 添加空行作为分隔
                    }
                }
            }
        }

        private void radioButton_A_CheckedChanged(object sender, EventArgs e)
        {
            // 获取当前选中的 RadioButton
            RadioButton radioButton = sender as RadioButton;
            if (questionModel == 0)
            {
                if (radioButton.Checked)
                {
                    // 获取当前问题的答案
                    char answer = questions[currentQuestionIndex].Answer;

                    // 检查答案是否正确
                    if (answer == 'A')
                    {
                        toolStripStatusLabel2.Text = "答对了！";
                        toolStripStatusLabel2.ForeColor = System.Drawing.Color.Green;
                        // 更新标记
                        mark.UpdateMark(true);
                    }
                    else
                    {
                        toolStripStatusLabel2.Text = "正确答案为: " + answer;
                        toolStripStatusLabel2.ForeColor = System.Drawing.Color.Red;
                        // 更新标记
                        mark.UpdateMark(false);
                    }

                    // 重置其他 RadioButton 为未选中状态
                    radioButton_B.Checked = false;
                    radioButton_C.Checked = false;
                }
            }
            else
            {
                char answer = questions.First().Answer;
                if (answer == 'A')
                {
                    mark.CorrectCount++;
                }
                else
                {
                    // 创建 StreamWriter 实例来写入文件
                    string wrongAnswersFilePath = "examWrongAnswers.txt";
                    using (StreamWriter writer = new StreamWriter(wrongAnswersFilePath))
                    {
                        // 写入题目和答案，确保题目和答案之间有适当的分隔
                        writer.WriteLine(questions.First().Question);
                        writer.WriteLine("答案: " + questions.First().Answer);
                        writer.WriteLine(Environment.NewLine); // 添加空行作为分隔
                    }
                }
                }

        }


        private void button_next_Click(object sender, EventArgs e)
        {
            if (questionModel == 0) // 刷题模式
            {
                // 确保列表中有问题存在
                if (questions.Any())
                {
                    // 获取下一个问题的索引
                    currentQuestionIndex = (currentQuestionIndex + 1) % questions.Count;
                    // 显示下一个问题的题目
                    DisplayQuestion();
                    // 更新题号
                    UpdateQuestionNumber();
                }
                // 如果列表中没有问题，显示消息
                else
                {
                    MessageBox.Show("题都做完了,卷狗");
                }
            }
            else // 考试模式
            {
                // 如果是考试模式，检查是否到达最后一题
                if (currentQuestionIndex == questions.Count - 1)
                {
                    // 显示正确数
                    toolStripStatusLabel2.Text = $"答对了 {mark.CorrectCount} 题\n卷死我了";
                    toolStripStatusLabel2.ForeColor = System.Drawing.Color.Green;
                }
                // 如果是刷题模式，继续显示下一题
                else
                {
                    // 获取下一个问题的索引
                    currentQuestionIndex = (currentQuestionIndex + 1) % questions.Count;
                    // 显示下一个问题的题目
                    DisplayQuestion();
                }
                radioButton_A.Checked = false;
                radioButton_B.Checked = false;
                radioButton_C.Checked = false;
            }
        }

        private void UpdateQuestionNumber()
        {
            // 创建 StreamWriter 实例来写入文件
            using (StreamWriter writer = new StreamWriter(defaultFilePath, false))
            {
                // 直接将当前题号加1后写入文件第一行
                writer.WriteLine(currentQuestionIndex + 1);
            }
        }


        private void button_last_Click(object sender, EventArgs e)
        {
            // 确保列表中有问题存在
            if (questions.Any())
            {
                // 如果当前索引小于0，显示第一个问题
                if (currentQuestionIndex < 0)
                {
                    currentQuestionIndex = questions.Count - 1;
                }
                // 显示上一个问题的题目
                DisplayQuestion();
            }
            // 如果列表中没有问题，显示消息
            else
            {
                MessageBox.Show("卷死我了,你把题都做完了!");
            }
        }


        private void DisplayQuestion()
        {
            // 确保列表中有问题存在
            if (questions.Any())
            {
                // 显示当前问题的题目
                richTextBox1.Text = questions[currentQuestionIndex].Question;
                // 更新状态栏显示当前题的序号
                toolStripStatusLabel3.Text = $"当前题号: {currentQuestionIndex + 1}";
            }
            // 如果列表中没有问题，显示消息
            else
            {
                MessageBox.Show("卷死我了,你把题都做完了!");
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // 创建 FileHelper 实例
            FileHelper fileHelper = new FileHelper();

            // 调用 FileHelper 类中的 RandomlySelectQuestions 方法
            fileHelper.RandomlySelectQuestions(questions, 80);
            if (questionModel == 1)
            {
                questionModel = 0;
                richTextBox1.Text = "已进入刷题模式";
                toolStripStatusLabel1.Text = "刷题模式";

            }
            else
            {
                richTextBox1.Text = "已进入考试模式";
                questionModel = 1;
                toolStripStatusLabel2.Text = "卷死我了!";
                toolStripStatusLabel1.Text = "考试模式";
            }


        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("有问题或者bug加:\n2969029950");
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0); // 退出程序
        }
    }
}
