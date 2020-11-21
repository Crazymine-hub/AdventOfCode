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
                    message = "==========================\r\n";
                    message += "YOU DIDN'T ENTER A NUMBER!\r\n";
                    message += "==========================\r\n";
                    continue;
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
    }
}
