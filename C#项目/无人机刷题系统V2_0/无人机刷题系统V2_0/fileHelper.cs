using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 无人机刷题系统V3_0
{
    // 输入类，用于存储问题和答案
    class Input
    {

        public string Question { get; set; } // 问题题目
        public char Answer { get; set; }   // 问题答案

        // 构造函数
        public Input(string question, char answer)
        {
            Question = question;
            Answer = answer;
        }
    }

    // 标记类，用于存储答题的正确数、错误数、当前题数以及总题数
    class Mark
    {
        public int CorrectCount { get; set; } // 正确数
        public int WrongCount { get; set; }   // 错误数
        public int CurrentQuestion { get; set; } // 当前题数
        public int TotalQuestions { get; set; }   // 总题数

        // 构造函数
        public Mark(int totalQuestions)
        {
            TotalQuestions = totalQuestions;
            CorrectCount = 0;
            WrongCount = 0;
            CurrentQuestion = 0;
        }

        // 用于更新标记的方法
        public void UpdateMark(bool isCorrect)
        {
            CurrentQuestion++; // 当前题数增加
            if (isCorrect)
            {
                CorrectCount++; // 答对则正确数增加
            }
            else
            {
                WrongCount++; // 答错则错误数增加
            }
        }
    }

    class FileHelper
    {
        public List<Input> ReadQuestionsFromFile(string filePath)
        {
            List<Input> questions = new List<Input>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                string currentQuestion = "";
                char currentAnswer = '\0'; // 初始化为无效字符

                foreach (string line in lines)
                {
                    // 检查是否是答案行
                    if (line.Trim().StartsWith("答案:"))
                    {
                        // 提取答案字符
                        currentAnswer = line.Trim().Substring(3).Trim()[0];
                        // 创建 Input 实例并添加到列表中
                        questions.Add(new Input(currentQuestion, currentAnswer));
                        // 重置当前问题文本和答案字符
                        currentQuestion = "";
                        currentAnswer = '\0';
                    }
                    else
                    {
                        // 构建问题文本
                        currentQuestion += line + Environment.NewLine;
                    }
                }
            }
            return questions;
        }

        public void RandomlySelectQuestions(List<Input> questions, int numberOfQuestions)
        {
            // 确保有足够的问题可供抽取
            if (questions.Count >= numberOfQuestions)
            {
                // 创建一个新的 List<Input> 实例来存储随机抽取的题目
                List<Input> selectedQuestions = new List<Input>();

                // 从题库中随机抽取指定数量的问题
                while (selectedQuestions.Count < numberOfQuestions)
                {
                    int randomIndex = new Random().Next(0, questions.Count);
                    Input randomQuestion = questions[randomIndex];
                    if (!selectedQuestions.Contains(randomQuestion))
                    {
                        selectedQuestions.Add(randomQuestion);
                    }
                }

                // 将抽取的题目添加到原本存在的列表中
                questions.Clear();
                questions.AddRange(selectedQuestions);
            }
            else
            {
                // 如果没有足够的问题，显示消息
                MessageBox.Show("题库中没有足够80个问题供考试模式使用\n内卷吧你就");
            }
        }
    }
}
