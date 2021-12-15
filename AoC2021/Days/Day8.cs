using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    internal class Day8 : DayBase
    {
        public override string Title => "Seven Segment Search";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            int uniqueDisplays = 0;
            int[] uniqueDigitSegmentCount = new int[] { 2, 4, 3, 7};
            foreach(string display in GetLines(input))
            {
                var displayData = display.Split('|');
                var samples = displayData[0].Split(' ');
                var values = displayData[1].Split(' ');
                uniqueDisplays += values.Select(x => x.Length).Count(x => uniqueDigitSegmentCount.Contains(x));
            }
            return $"Output digits with unique segment count: {uniqueDisplays}";
        }
    }
}
