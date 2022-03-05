using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Visualization;

namespace AdventOfCode
{
    class Program
    {
        private const string AdditionFileSuffix = "_addition";
        static string message = "";
        static string dayPath;
        static string year;
        static Assembly lib;
        static CancellationTokenSource tokenSource;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Initialize(args);

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
                DayBase day = GetDay(dayNr, out message);
                if (day != null && RunDay(day, dayNr, out message))
                {
                    Console.WriteLine("Done! Press enter to return to start.");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { /*that is not the key i want*/}
                }
                (day as IDisposable)?.Dispose();
            }
        }

        private static DayBase GetDay(int dayNr, out string message)
        {
            message = "";
            Type DayType = lib.GetType("AdventOfCode.Days.Day" + dayNr);
            if (DayType == null)
            {
                message = "=================\r\n";
                message += "DAY DOESN'T EXIST\r\n";
                message += "=================\r\n";
                return null;
            }

            var day = (DayBase)Activator.CreateInstance(DayType);

            Console.Clear();
            Console.Title = $"Advent Of Code - {year} Day " + dayNr;
            Console.Write("You Chose: Day " + dayNr);
            if (!string.IsNullOrWhiteSpace(day.Title))
                Console.WriteLine(": " + day.Title);
            else Console.WriteLine();
            return day;
        }

        private static bool RunDay(DayBase day, int dayNr, out string message)
        {
            message = "";
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
                case ConsoleKey.D2:
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

                default:
                    return false;
            }

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

            Stopwatch stopwatch = new Stopwatch();
            using (tokenSource = new CancellationTokenSource())
            {
                try
                {
                    if (day.UsesAdditionalContent && File.Exists(dayPath + dayNr + AdditionFileSuffix + fileExtension))
                        day.AdditionalContent = File.ReadAllText(dayPath + dayNr + AdditionFileSuffix + fileExtension);

                    day.CancellationToken = tokenSource.Token;
                    stopwatch.Start();
                    Console.WriteLine(day.Solve(LoadInput(dayPath + dayNr + fileExtension, custIn), useSecond == 2));
                    stopwatch.Stop();

                    if (day.UsesAdditionalContent && day.AdditionalContent != null)
                        File.WriteAllText(dayPath + dayNr + AdditionFileSuffix + fileExtension, day.AdditionalContent);
                }
                catch (OperationCanceledException ex)
                {
                    if (ex.CancellationToken != tokenSource.Token) throw;
                    Console.WriteLine("Operation canceled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An Exception occured while running the Day.");
                    Console.WriteLine(ex.ToString());
                }
            }
            tokenSource = null;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Completed in " + stopwatch.Elapsed.ToString());
            if (VisualFormHandler.ValidInstanceCount > 0)
            {
                Console.WriteLine("Waiting for Visualization to be closed...");
                Console.WriteLine("Press enter to force close the visualizer.");
                while (VisualFormHandler.ValidInstanceCount > 0)
                {
                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Enter)
                    {
                        VisualFormHandler.ClearAll();
                        return false;
                    }
                }
            }
            VisualFormHandler.ClearAll();
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



        private static void Initialize(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
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
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (tokenSource == null) return;
            tokenSource.Cancel();
            e.Cancel = true;
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
    }
}
