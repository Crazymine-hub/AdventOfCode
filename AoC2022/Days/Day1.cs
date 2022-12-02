using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public override string Title => "Calorie Counting";

        public override string Solve(string input, bool part2)
        {
            var calories = GetGroupedLines(input).Select(group => GetLines(group).Select(line => int.Parse(line)).Sum());

            if(part2)
                return $"{calories.OrderByDescending(x => x).Take(3).Sum()} Calories are carried by the top 3 elves.";

            return $"{calories.Max()} Calories are the maximum carried by an elve.";
        }
    }
}
