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

namespace 无人机刷题系统3_0
{
    public class Mark
    {
        public int CurrentQuestionIndex { get; set; } // 当前题号
        public int CorrectCount { get; set; } // 答题正确数
        public int WrongCount { get; set; } // 答题错误数
        public int TotalQuestions { get; set; } // 总题数
        public List<Question> Questions { get; set; } // 题目列表

        public Mark(int totalQuestions)
        {
            CurrentQuestionIndex = 0;
            CorrectCount = 0;
            WrongCount = 0;
            TotalQuestions = totalQuestions;
            Questions = new List<Question>();
        }

        // 用于更新标记信息的方法
        public void UpdateMark(bool isCorrect)
        {
            CurrentQuestionIndex++;
            if (isCorrect)
            {
                CorrectCount++;
            }
            else
            {
                WrongCount++;
            }
        }
    }
}
