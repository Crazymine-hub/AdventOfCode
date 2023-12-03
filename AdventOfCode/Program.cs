using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AdventOfCode.Tools;

namespace AdventOfCode
{
    class Program
    {
        private const string AdditionFileSuffix = "_addition";
        private const string InputFileExtension = ".txt";
        private const string Part2FileSuffix = "_2";
        static string message = "";
        static string inputPath;
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
            bool part2 = false;
            bool testMode = false;
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.NumPad1 or ConsoleKey.D1:
                    part2 = false;
                    break;
                case ConsoleKey.NumPad2 or ConsoleKey.D2:
                    part2 = true;
                    break;
                case ConsoleKey.T:
                    testMode = true;
                    part2 = false;
                    break;
                case ConsoleKey.P:
                    testMode = true;
                    part2 = true;
                    break;

                default:
                    return false;
            }

            Console.Clear();

            if (!string.IsNullOrWhiteSpace(day.Title))
                Console.Title += " ---" + day.Title + "---";
            if (part2)
                Console.Title += " Part 2";
            else
                Console.Title += " Part 1";

            (FileInfo inputFile, FileInfo additionalContentFile) = GetInputFiles(dayNr, part2, testMode);

            if (!testMode && !inputFile.Exists)
            {
                message = "===================\r\n";
                message += "NO INPUT FILE FOUND\r\n";
                message += "===================\r\n";
                return false;
            }

            Stopwatch stopwatch = new Stopwatch();
            using (tokenSource = new CancellationTokenSource())
            {
#if !DEBUG
                try
                {
#endif
                if (day.UsesAdditionalContent && File.Exists(additionalContentFile.FullName))
                    day.AdditionalContent = File.ReadAllText(additionalContentFile.FullName);

                day.CancellationToken = tokenSource.Token;
                string input = LoadInput(inputFile, testMode, dayNr);
                Console.Clear();
                stopwatch.Start();
                Console.WriteLine(day.Solve(input, part2));
                stopwatch.Stop();

                if (day.UsesAdditionalContent && day.AdditionalContent != null)
                    File.WriteAllText(additionalContentFile.FullName, day.AdditionalContent);
#if !DEBUG
                }
                catch (OperationCanceledException ex)
                {
                    if (ex.CancellationToken != tokenSource.Token) throw;
                    tokenSource.Cancel();
                    Console.WriteLine("Operation canceled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An Exception occured while running the Day.");
                    Console.WriteLine(ex.ToString());
                }
#endif
            }
            tokenSource = null;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Completed in " + stopwatch.Elapsed.ToString());
            //if (VisualFormHandler.ValidInstanceCount > 0)
            //{
            //    Console.WriteLine("Waiting for Visualization to be closed...");
            //    Console.WriteLine("Press enter to force close the visualizer.");
            //    while (VisualFormHandler.ValidInstanceCount > 0)
            //    {
            //        if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Enter)
            //        {
            //            VisualFormHandler.ClearAll();
            //            return false;
            //        }
            //    }
            //}
            //VisualFormHandler.ClearAll();
            GC.Collect();
            return true;
        }


        static (FileInfo inputFile, FileInfo additionalContentFile) GetInputFiles(int day, bool part2, bool testMode)
        {
            var baseFile = inputPath;
            if (testMode)
                baseFile += "Test\\";

            baseFile += $"Day{day}";

            if (part2 && File.Exists(baseFile + Part2FileSuffix + InputFileExtension))
                baseFile += Part2FileSuffix;
            FileInfo additionalContentFile = new FileInfo(baseFile + AdditionFileSuffix + InputFileExtension);
            FileInfo inputFile = new FileInfo(baseFile + InputFileExtension);

            return (inputFile, additionalContentFile);
        }


        static string LoadInput(FileInfo inputFile, bool testMode, int dayNr)
        {
            if (testMode &&
                (!File.Exists(inputFile.FullName) || !ConsoleAssist.GetUserYesNo("A File with Testinput has been found. Do you want to use it?", true)))
            {
                Console.WriteLine("Please enter the testinput in the editor.");
                Console.WriteLine("Be aware, that the input is not checked. Proceed with caution");
                Console.WriteLine("Save and close the editor once finished");

                string? tempFile = null;
                try
                {
                    tempFile = Path.GetTempPath() + $"AoC_{year}_{dayNr}_" + Path.GetRandomFileName() + ".txt";
                    File.Create(tempFile).Dispose();
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo(tempFile)
                        {
                            UseShellExecute = true
                        }
                    };

                    process.Start();
                    process.WaitForExit();
                    var input = File.ReadAllText(tempFile);
                    Console.Clear();
                    return input;
                }
                finally
                {
                    if (tempFile is not null)
                        File.Delete(tempFile);
                }
            }
            return File.ReadAllText(inputFile.FullName);
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
                inputPath = directory + "\\Inputs\\";
                return true;
            }
            return false;
        }
    }
}
