using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventOfCode.Tools;

namespace AdventOfCode
{
    class Program
    {
        static string message = "";
        static string dayPath;
        static string year;
        static Assembly lib;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Advent of Code - Initializing";
            if (args.Length <= 0 || !TryGetLibrary(args[0]))
            {
                Console.Title = "Advent of Code - Set Year";
                while (true)
                {
                    int userYear = ConsoleAssist.GetUserInput("No year library specified. Enter a year number:");
                    if (TryGetLibrary(Directory.GetCurrentDirectory() + "\\" + userYear)) break;
                }
            }


            while (true)
            {
                Console.Clear();
                if (message != "") Console.WriteLine(message);
                Console.Title = $"Advent Of Code {year} - Day Select.";
                int dayNr = ConsoleAssist.GetUserInput(
                    $"Advent Of Code {year}\r\n" +
                    "Enter the number of the Day to use\r\n" +
                    "Use Ctrl+C or enter 0 to quit.", false);
                message = "";

                if (dayNr <= 0) return;

                if (!RunDay(dayNr, out message)) continue;

                Console.WriteLine("Done! Press enter to return to start.");
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { /*that is not the key i want*/}
            }
        }

        private static bool TryGetLibrary(string directory)
        {
            if (!Directory.Exists(directory)) return false;
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                var dllMatch = Regex.Match(file, @"AoC(\d{4})\.dll$");
                if (!dllMatch.Success) continue;
                var asm = Assembly.LoadFrom(file);
                foreach (var dllType in asm.GetExportedTypes())
                    if (!dllType.FullName.StartsWith("AdventOfCode")) return false;
                lib = asm;
                year = dllMatch.Groups[1].Value;
                dayPath = directory + "\\Inputs\\Day";
                return true;
            }
            return false;
        }

        private static bool RunDay(int dayNr, out string message)
        {
            message = "";
            Type DayType = lib.GetType("AdventOfCode.Days.Day" + dayNr);
            if (DayType == null)
            {
                message = "=================\r\n";
                message += "DAY DOESN'T EXIST\r\n";
                message += "=================\r\n";
                return false;
            }
            var day = (DayBase)Activator.CreateInstance(DayType);

            Console.Clear();
            Console.Title = $"Advent Of Code - {year} Day " + dayNr;
            Console.Write("You Chose: Day " + dayNr);
            if (!string.IsNullOrWhiteSpace(day.Title))
                Console.WriteLine(": " + day.Title);
            else Console.WriteLine();

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

            if (useSecond == 0) return false;

            Console.Clear();

            string fileExtension = ".txt";
            if (useSecond == 1)
            {
                if (!File.Exists(dayPath + dayNr + fileExtension) && !custIn)
                {
                    message = "===================\r\n";
                    message += "NO INPUT FILE FOUND\r\n";
                    message += "===================\r\n";
                    return false;
                }
            }
            else if (File.Exists(dayPath + dayNr + "_2.txt"))
                fileExtension = "_2.txt";

            if (!string.IsNullOrWhiteSpace(day.Title))
                Console.Title += " ---" + day.Title + "---";
            if (useSecond == 2)
                Console.Title += " Part 2";
            else
                Console.Title += " Part 1";

            if (day.UsesAdditionalContent && File.Exists(dayPath + dayNr + "_addition" + fileExtension))
                day.AdditionalContent = File.ReadAllText(dayPath + dayNr + "_addition" + fileExtension);

            Console.WriteLine(day.Solve(LoadInput(dayPath + dayNr + fileExtension, custIn), useSecond == 2));

            if (day.UsesAdditionalContent && day.AdditionalContent != null)
                File.WriteAllText(dayPath + dayNr + "_addition" + fileExtension, day.AdditionalContent);
            Console.WriteLine();
            Console.WriteLine();
            return true;
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
                return File.ReadAllText(fileName);
            }
        }
    }
}
