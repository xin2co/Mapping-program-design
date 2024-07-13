using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_4_22
{
    //计算阶乘的方法
    class Factorial
    {
        public static long Calculate(int number)
        {
            if (number < 0)
            {
                throw new ArgumentException("数字必须为非负数");
                //在C#中，throw关键字用于抛出一个异常。当一个异常被抛出时，正常的程序执行流程会被中断，并且程序的控制权会转移到最近的异常处理代码（try-catch块）。
            }

            long result = 1;
            for(int i = 1; i <= number; i++)
            {
                result *= i;
            }

            return result;
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            //相当于cout
            Console.WriteLine("欢迎使用阶乘计算器:");

            while (true)
            {
                //相当于cin
                Console.WriteLine("请输入一个非负整数(输入-1退出):");

                string input = Console.ReadLine();
                /*int.TryParse 是 C# 中 int 类型的一个静态方法，它用于尝试将字符串解析为整数类型 int。
                 这个方法非常有用，因为它提供了非破坏性的解析方式，即在解析失败时不会抛出异常。
                int.TryParse 方法接受两个参数：
                        input：要解析的字符串。
                        out number：一个 out 参数，用于接收解析后的整数结果。*/
                if(int.TryParse(input,out int number))
                {
                    if (number == -1)
                    {
                        break;
                    }
                    else if (number < 0)
                    {
                        Console.WriteLine("错误:请输入非负整数。");
                    }
                    else
                    {
                        /*这段代码是一个try-catch块，它在C#中用于异常处理。
                         * 在这个特定的例子中，它用于调用Factorial类的Calculate方法，这个方法计算一个整数的阶乘。
                         * 如果Calculate方法在执行过程中抛出了一个ArgumentException异常，
                         * catch块将会捕获这个异常，并打印出异常的消息。*/
                        try
                        {
                            //从类中调用函数
                            long factorial = Factorial.Calculate(number);
                            Console.WriteLine($"{number}!={factorial}");
                        }
                        catch(ArgumentException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("错误,请输入有效整数");
                }
            }
            Console.WriteLine("感谢使用阶乘计算器！按任意键退出。");
            Console.ReadKey();
        }
    }
}
