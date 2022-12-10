using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day6 : DayBase
    {
        public override string Title => "Tuning Trouble";

        public override string Solve(string input, bool part2)
        {
            int offset = -1;
            bool foundStart = false;
            int packageLength = part2? 14 : 4;
            while (!foundStart)
            {
                CancellationToken.ThrowIfCancellationRequested();
                ++offset;
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(input.Substring(offset, Math.Min(Console.BufferWidth, input.Length - offset)));
                Console.WriteLine("┘".PadLeft(packageLength, '─'));
                var signal = input.Substring(offset, packageLength);
                foundStart = signal.Distinct().Count() == packageLength;
                Task.Delay(1).Wait();
            }
            return $"Package starts after {offset + packageLength} characters";
        }
    }
}
