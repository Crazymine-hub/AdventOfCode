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

        public void WaitForAllTasks(IEnumerable<Task> tasks, int delay = 1000, System.Threading.CancellationToken cancellationToken = default)
        {
            int previousActive = -1;
            int activeCount = tasks.Count(x => !x.IsCompleted);
            while (activeCount > 0)
            {
                if(previousActive != activeCount)
                {
                    previousActive = activeCount;
                    --Console.CursorLeft;
                    Console.Write(" ");
                    Console.CursorLeft = 0;
                    Console.Write($"Waiting for {activeCount}/{tasks.Count()} tasks to complete...  ");
                }

                --Console.CursorLeft;
                Console.Write(GetNextProgressChar());
                activeCount = tasks.Count(x => !x.IsCompleted);
                Task.Delay(delay).Wait();
                cancellationToken.ThrowIfCancellationRequested();
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
