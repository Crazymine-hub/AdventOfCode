using AdventOfCode.Tools.IntComputer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day17 : IDay
    {
        private IntComputer camera = new IntComputer(true);
        public string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            camera.ReadMemory(input);
            camera.OnOutput += Camera_OnOutput;
            camera.InputRequested += Camera_InputRequested;
            if (part2)
                camera.Memory[0] = 2;
            Console.Clear();
            camera.Run();
            return "";
        }

        private long Camera_InputRequested()
        {
            var input = Console.ReadKey();
            if (input.Key == ConsoleKey.Enter)
                return 10;
            return input.KeyChar;
        }

        private void Camera_OnOutput(long value)
        {
            string display = Convert.ToChar(value).ToString();
            if (display == "\n")
                display = "\r\n";
            Console.Write(display);
        }
    }
}
