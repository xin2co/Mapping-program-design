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
using System.Text.RegularExpressions;

namespace 无人机刷题系统3_0
{
    public partial class Form1 : Form
    {
        private List<Question> questions;
        private Mark practiceMark;
        private Mark examMark;
        private int practiceCurrentQuestionIndex = 0;
        private int examCurrentQuestionIndex = 0;
        private bool isPracticeMode = true; // 刷题模式默认为true
        private string stateFilePath = "state.txt"; // 状态文件路径

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 读取题目文件并初始化列表
            string filePath = "itemBank.txt"; // 确保文件路径正确
            questions = FileHelper.ReadQuestions(filePath);

            // 恢复状态
            RestoreState();

            // 初始化toolStripStatusLabel控件
            toolStripStatusLabel1.Text = isPracticeMode ? "刷题模式" : "考试模式";
            toolStripStatusLabel2.Text = "请作答";
            UpdateQuestionStatus();
            // 显示当前题目的题目
            DisplayQuestion();
        }
    



        private void DisplayQuestion()
        {
            if (isPracticeMode)
            {
                Question question = practiceMark.Questions[practiceCurrentQuestionIndex];
                richTextBox1.Clear(); // 清空富文本框
                richTextBox1.Text = question.Title; // 显示新题目
                UpdateQuestionStatus();
            }
            else
            {
                Question question = examMark.Questions[examCurrentQuestionIndex];
                richTextBox1.Clear(); // 清空富文本框
                richTextBox1.Text = question.Title; // 显示新题目
                UpdateQuestionStatus();
            }
        }


        private void UpdateQuestionStatus()
        {
            toolStripStatusLabel3.Text = $"当前题号: {GetCurrentQuestionIndex() + 1}";
        }

        private int GetCurrentQuestionIndex()
        {
            return isPracticeMode ? practiceCurrentQuestionIndex : examCurrentQuestionIndex;
        }

        private void ModelChangeButton_Click(object sender, EventArgs e)
        {
            isPracticeMode = !isPracticeMode; // 切换模式
            if (isPracticeMode)
            {
                // 切换到刷题模式
                toolStripStatusLabel1.Text = "刷题模式";
                DisplayQuestion();
            }
            else
            {
                // 切换到考试模式
                toolStripStatusLabel1.Text = "考试模式";
                RandomizeExamQuestions(); // 随机化考试题目顺序
                examCurrentQuestionIndex = 0; // 设置考试模式的当前题目索引为0
                DisplayQuestion();
            }

            // 保存状态
            SaveState();
        }

        private void RandomizeExamQuestions()
        {
            Random random = new Random();
            List<Question> examQuestions = questions.OrderBy(q => random.Next()).ToList().Take(80).ToList(); // 随机化并取80题
            examMark = new Mark(80); // 创建一个新的Mark对象，确保大小为80
            examCurrentQuestionIndex = 0; // 设置考试模式的当前题目索引为0
            examMark.Questions = examQuestions; // 设置考试模式的题目列表
            questions = examQuestions; // 更新题目列表为考试题目
        }


        private void SaveState()
        {
            using (StreamWriter sw = new StreamWriter(stateFilePath))
            {
                sw.WriteLine(isPracticeMode ? "Practice" : "Exam");
                sw.WriteLine(practiceCurrentQuestionIndex);
                sw.WriteLine(practiceMark.CorrectCount);
                sw.WriteLine(practiceMark.WrongCount);
                sw.WriteLine(examCurrentQuestionIndex);
                sw.WriteLine(examMark.CorrectCount);
                sw.WriteLine(examMark.WrongCount);
            }
        }

        private void RestoreState()
        {
            if (File.Exists(stateFilePath))
            {
                string[] lines = File.ReadAllLines(stateFilePath);
                if (lines.Length >= 7)
                {
                    isPracticeMode = lines[0] == "Practice";
                    practiceCurrentQuestionIndex = int.Parse(lines[1]);
                    practiceMark = new Mark(questions.Count);
                    practiceMark.CorrectCount = int.Parse(lines[2]);
                    practiceMark.WrongCount = int.Parse(lines[3]);
                    examCurrentQuestionIndex = int.Parse(lines[4]);
                    examMark = new Mark(80);
                    examMark.CorrectCount = int.Parse(lines[5]);
                    examMark.WrongCount = int.Parse(lines[6]);
                }
                else
                {
                    practiceMark = new Mark(questions.Count);
                    examMark = new Mark(80);
                }
            }
            else
            {
                practiceMark = new Mark(questions.Count);
                examMark = new Mark(80);
            }
        }
    }
}
