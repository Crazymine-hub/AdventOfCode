using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    public class ConsoleAssist
    {
        readonly static char[] progressIndicator = new char[] { '|', '/', '─', '\\' };
        int progressPos = 0;

        public static int GetUserInput(string prompt, bool noInitialClear = true)
        {
            string message = "";
            while (true)
            {
                if (noInitialClear) Console.Clear();
                noInitialClear = true;
                if (message != "") Console.WriteLine(message);
                message = "";
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int inputNr))
                {
                    message = "==========================\r\n" +
                    "YOU DIDN'T ENTER A NUMBER!\r\n" +
                    "==========================\r\n";
                }
                else
                    return inputNr;
            }
        }

        public char GetNextProgressChar()
        {
            progressPos++;
            if (progressPos >= progressIndicator.Length)
                progressPos = 0;
            return progressIndicator[progressPos];
        }

        public void WaitForAllTasks(IEnumerable<Task> tasks, int delay = 1000, System.Threading.CancellationToken cancellationToken = default, Func<string> progressUpdateCallback = null)
        {
            int previousActive = -1;
            int activeCount = tasks.Count(x => !x.IsCompleted);
            try
            {
                Console.TreatControlCAsInput = true;
                int indicatorPosition = 0;
                var width = 0;
                while (activeCount > 0)
                {
                    if (previousActive != activeCount)
                    {
                        previousActive = activeCount;
                        width = Console.CursorLeft;
                        Console.CursorLeft = 0;
                        Console.Write(string.Empty.PadLeft(width));
                        Console.CursorLeft = 0;
                        Console.Write($"Waiting for {activeCount}/{tasks.Count()} tasks to complete...  ");
                        indicatorPosition = Console.CursorLeft;
                    }

                    width = Console.CursorLeft - indicatorPosition;
                    Console.CursorLeft = indicatorPosition;
                    Console.Write(GetNextProgressChar());
                    if (progressUpdateCallback != null)
                    {
                        Console.Write(string.Empty.PadLeft(width));
                        Console.CursorLeft -= width;
                        Console.Write(" ");
                        Console.Write(progressUpdateCallback());
                    }
                    activeCount = tasks.Count(x => !x.IsCompleted);
                    var start = DateTime.Now;
                    while ((DateTime.Now - start).TotalMilliseconds < delay)
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(false);
                            if (key.Modifiers == ConsoleModifiers.Control && (key.Key == ConsoleKey.C || key.Key == ConsoleKey.Pause))
                            {
                                Console.WriteLine();
                                Console.WriteLine($"Waiting canceled");
                                throw new OperationCanceledException(cancellationToken);
                            }
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            }
            finally
            {
                Console.TreatControlCAsInput = false;
            }

            Console.WriteLine();
            Console.WriteLine("All Complete");
        }

        public static string Center(string text, int width, char padChar = ' ')
        {
            int left = width - text.Length;
            if (left < 0) left = 0;
            return text.PadLeft(left / 2 + text.Length, padChar).PadRight(left + text.Length, padChar);
        }
    }
}
