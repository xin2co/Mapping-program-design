using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 无人机刷题系统3_0
{
    public class Question
    {
        public string Title { get; set; } // 题目
        public string Answer { get; set; } // 答案
        public bool IsAnswered { get; set; } // 是否被回答

        public Question(string title, string answer)
        {
            Title = title;
            Answer = answer;
            IsAnswered = false;
        }
    }
}
