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
        public const string path = @"D:\Felix\Documents\Programmieren\C#\Advent of Code\Day";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            while (true)
            {
                Console.Clear();
                if (message != "") Console.WriteLine(message);
                message = "";
                Console.Title = "Advent Of Code - Day Select.";
                int dayNr = ConsoleAssist.GetUserInput(
                    "Advent Of Code\r\n" +
                    "Enter the number of the Day to use\r\n" +
                    "Use Ctrl+C or enter 0 to quit.");

                if (dayNr <= 0) return;

                Type DayType = Type.GetType("AdventOfCode.Days.Day" + dayNr);
                if (DayType == null)
                {
                    message = "Day doesn't exist";
                    continue;
                }

                Console.Clear();
                Console.Title = "Advent Of Code - Day " + dayNr;
                Console.WriteLine("You Chose: Day " + dayNr);

                Console.WriteLine("Select a Mode by pressing the Key in ():");
                Console.WriteLine("Part (1)");
                Console.WriteLine("Part (2)");
                Console.WriteLine("(T)est Part 1");
                Console.WriteLine("Test (P)art 2");
                Console.WriteLine("Any other Key: cancel");
                byte useSecond = 0;
                bool custIn = false;
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.NumPad1:
                        useSecond = 1;
                        break;
                    case ConsoleKey.D1:
                        useSecond = 1;
                        break;
                    case ConsoleKey.NumPad2:
                        useSecond = 2;
                        break;
                    case ConsoleKey.T:
                        custIn = true;
                        useSecond = 1;
                        break;
                    case ConsoleKey.P:
                        custIn = true;
                        useSecond = 2;
                        break;

                }

                if (useSecond == 0)
                    continue;

                    Console.Clear();
                if (useSecond == 2)
                    Console.Title += " Part 2";
                else
                    Console.Title += " Part 1";

                string fileextension = ".txt";
                if (useSecond == 1 && File.Exists(path + dayNr + "_2.txt"))
                    fileextension = "_2.txt";
                Console.WriteLine(((IDay)Activator.CreateInstance(DayType)).Solve(LoadInput(path + dayNr + fileextension, custIn), useSecond == 2));
                Console.WriteLine();
                Console.WriteLine("Done! Press any Key to return to start.");
                Console.ReadKey();
            }
        }

        static string LoadInput(string fileName, bool userInput)
        {
            if (userInput)
            {
                Console.WriteLine("Please enter the Testinput to use.");
                Console.WriteLine("Be aware, that the input is not checked. Proceed with caution");
                Console.WriteLine("Use \\n for line breaks");
                var input = Console.ReadLine().Replace("\\n", "\r\n");
                Console.Clear();
                return input;
            }
            else
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
}
