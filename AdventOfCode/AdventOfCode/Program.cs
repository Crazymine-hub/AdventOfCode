using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Tools;

namespace AdventOfCode
{
    class Program
    {
        static string message = "";
        const string path = @"D:\Felix\Documents\Programmieren\C#\Advent of Code\Day";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                if (message != "") Console.WriteLine(message);
                message = "";
                int dayNr = ConsoleAssist.GetUserInput(
                    "Advent Of Code\r\n" +
                    "Enter the number of the Day to use\r\n" +
                    "Use Ctrl+C to quit.");

                Type DayType = Type.GetType("AdventOfCode.Days.Day" + dayNr);
                if(DayType == null)
                {
                    message = "Day doesn't exist";
                    continue;
                }

                Console.Clear();
                Console.WriteLine("You Chose: Day " + dayNr);

                Console.WriteLine("do you want to use part 2? (y/n)");
                byte useSecond = 0;
                do
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.Y:
                            useSecond = 1;
                            break;
                        case ConsoleKey.N:
                            useSecond = 2;
                            break;
                        default:
                            Console.SetCursorPosition(0, 3);
                            Console.WriteLine("Invalid Input. Try again");
                            break;

                    }
                } while (useSecond == 0);

                string fileextension = ".txt";
                if (useSecond == 1 && File.Exists(path + dayNr + "_2.txt"))
                    fileextension = "_2.txt";
                Console.Clear();
                Console.WriteLine(((IDay)Activator.CreateInstance(DayType)).Solve(LoadInput(path+dayNr+fileextension), useSecond == 1));
                Console.WriteLine();
                Console.WriteLine("Done! Press any Key to return to start.");
                Console.ReadKey();
            }
        }

        static string LoadInput(string fileName)
        {
            FileStream input = File.OpenRead(fileName);
            byte[] content = new byte[input.Length];
            if (input.Length != input.Read(content, 0, (int)input.Length))
                throw new Exception("nicht alles gelesen");
            input.Close();
            char[] text = new char[content.Length];
            content.CopyTo(text, 0);
            return string.Join("", text);
        }
    }
}
