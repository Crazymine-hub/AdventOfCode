using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day6 : DayBase
    {
        public override string Title => "Lanternfish";

        private ulong[] fishies = new ulong[9];

        public override string Solve(string input, bool part2)
        {

            foreach (string fishAge in input.Split(','))
                ++fishies[int.Parse(fishAge)];

            ConsoleAssist consoleAssist = new ConsoleAssist();

            int stepCount = (part2 ? 256 : 80);
            ulong reproducing = 0;
            for (int i = 0; i <  stepCount; ++i)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(consoleAssist.GetNextProgressChar());
                Console.WriteLine("".PadLeft(Convert.ToInt32(i / (float)stepCount * Console.BufferWidth), '█'));
                Console.WriteLine($"Fishies: {fishies.Aggregate((ulong current, ulong next) => current + next)}");
                ulong carry = 0;
                for(int f = fishies.Length - 1; f >= 0; --f)
                {
                    ulong swapBuffer = fishies[f];
                    fishies[f] = carry;
                    carry = swapBuffer;
                }
                fishies[6] += carry;
                fishies[8] += carry;
            }

            return $"Fishies: {fishies.Aggregate((ulong current, ulong next) => current + next)}";
        }
    }
}
