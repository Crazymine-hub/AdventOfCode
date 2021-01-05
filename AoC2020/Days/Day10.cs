using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day10 : DayBase
    {
        public override string Title => "Adapter Array";


        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part2 is unavailable";
            List<int> adapters = GetLines(input).Select(x => int.Parse(x)).ToList();
            adapters.Add(0);
            adapters.Add(adapters.Max() + 3);
            adapters.Sort();
            int[] differences = new int[3];
            for(int i = 1; i < adapters.Count; i++)
            {
                int diff = adapters[i] - adapters[i - 1];
                differences[diff - 1]++;
            }
            Console.WriteLine("Adapter Differences:");
            Console.WriteLine("1: " + differences[0]);
            Console.WriteLine("2: " + differences[1]);
            Console.WriteLine("3: " + differences[2]);
            return "Result: " + (differences[0] * differences[2]);
        }
    }
}
