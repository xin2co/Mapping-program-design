using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace 无人机刷题系统3_0
{
    public static class FileHelper
    {
        public static List<Question> ReadQuestions(string filePath)
        {
            List<Question> questions = new List<Question>();
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                string pattern = @"(\d+\.)(.*?)(答案:)(.)\.";
                MatchCollection matches = Regex.Matches(fileContent, pattern, RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    if (match.Groups.Count == 5)
                    {
                        string number = match.Groups[1].Value.Trim('.');
                        string title = match.Groups[2].Value.Trim();
                        string answer = match.Groups[4].Value.Trim().ToUpper();
                        questions.Add(new Question(title, answer));
                    }
                }
            }
            return questions;
        }
    }
}
