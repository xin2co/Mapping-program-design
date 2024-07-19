using System;
using System.Collections.Generic;
using System.IO;

namespace StudentManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Student> students = new List<Student>();

            // 欢迎界面
            Console.WriteLine("欢迎使用学生管理系统");

            while (true)
            {
                Console.WriteLine("\n请选择一个操作：");
                Console.WriteLine("1. 添加学生");
                Console.WriteLine("2. 显示所有学生");
                Console.WriteLine("3. 保存到文件");
                Console.WriteLine("4. 从文件加载");
                Console.WriteLine("5. 修改学生信息");
                Console.WriteLine("6. 删除学生信息");
                Console.WriteLine("7. 退出");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent(students);
                        break;
                    case "2":
                        DisplayStudents(students);
                        break;
                    case "3":
                        SaveStudentsToFile(students);
                        break;
                    case "4":
                        LoadStudentsFromFile(students);
                        break;
                    case "5":
                        EditStudentInfo(students);
                        break;
                    case "6":
                        DeleteStudent(students);
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("无效的选择，请重新输入。");
                        break;
                }
            }
        }

        static void AddStudent(List<Student> students)
        {
            Console.WriteLine("请输入学生信息:");
            Console.Write("学号:");
            string id = Console.ReadLine();
            Console.Write("姓名:");
            string name = Console.ReadLine();
            Console.Write("性别（输入'男'或'女'）:");
            string gender = Console.ReadLine();
            Console.Write("手机号:");
            string phoneNumber = Console.ReadLine();

            Student newStudent = new Student { Id = id, Name = name, Gender = gender, PhoneNumber = phoneNumber };
            students.Add(newStudent);
            Console.WriteLine("学生信息已添加");
        }

        static void DisplayStudents(List<Student> students)
        {
            Console.WriteLine("\n学生列表：");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"学号：{students[i].Id}, 姓名：{students[i].Name}, 性别：{students[i].Gender}, 手机号：{students[i].PhoneNumber}");
            }
        }

        static void SaveStudentsToFile(List<Student> students)
        {
            using (StreamWriter sw = new StreamWriter("students.txt"))
            {
                foreach (Student student in students)
                {
                    sw.WriteLine($"{student.Id},{student.Name},{student.Gender},{student.PhoneNumber}");
                }
            }
            Console.WriteLine("学生信息已保存到文件。");
        }

        static void LoadStudentsFromFile(List<Student> students)
        {
            students.Clear();
            if (File.Exists("students.txt"))
            {
                using (StreamReader sr = new StreamReader("students.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        Student student = new Student
                        {
                            Id = parts[0],
                            Name = parts[1],
                            Gender = parts[2],
                            PhoneNumber = parts[3]
                        };
                        students.Add(student);
                    }
                }
                Console.WriteLine("学生信息已从文件加载。");
            }
            else
            {
                Console.WriteLine("文件不存在，无法加载学生信息。");
            }
        }

        static void EditStudentInfo(List<Student> students)
        {
            Console.Write("请输入要修改的学生学号:");
            string id = Console.ReadLine();

            Student studentToEdit = students.Find(s => s.Id == id);

            if (studentToEdit != null)
            {
                Console.WriteLine("请输入新的学生信息:");
                Console.Write("姓名（留空保持不变）：");
                string name = Console.ReadLine();
                Console.Write("性别（留空保持不变）：");
                string gender = Console.ReadLine();
                Console.Write("手机号（留空保持不变）：");
                string phoneNumber = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    studentToEdit.Name = name;
                }
                if (!string.IsNullOrWhiteSpace(gender))
                {
                    studentToEdit.Gender = gender;
                }
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    studentToEdit.PhoneNumber = phoneNumber;
                }

                Console.WriteLine("学生信息已更新。");
            }
            else
            {
                Console.WriteLine("未找到该学号的学生。");
            }
        }

        static void DeleteStudent(List<Student> students)
        {
            Console.Write("请输入要删除的学生学号：");
            string id = Console.ReadLine();

            Student studentToDelete = students.Find(s => s.Id == id);

            if (studentToDelete != null)
            {
                students.Remove(studentToDelete);
                Console.WriteLine("学生信息已删除。");
            }
            else
            {
                Console.WriteLine("未找到该学号的学生。");
            }
        }
    }

    class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
    }
}