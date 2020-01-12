using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    static class ConsoleAssist
    {
        public static int GetUserInput(string prompt)
        {
            string message = "";
            while (true)
            {
                Console.Clear();
                if (message != "") Console.WriteLine(message);
                message = "";
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int inputNr))
                {
                    message = "You didn't enter a number";
                    continue;
                }
                else
                    return inputNr;
            }
        }
    }
}
