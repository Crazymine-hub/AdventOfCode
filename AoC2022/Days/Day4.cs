using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day4 : DayBase
    {
        public override string Title => "Camp Cleanup";

        public override string Solve(string input, bool part2)
        {
            var reconsiderCount = (from line in GetLines(input)
                                   where NeedsReconsideration(line, !part2)
                                   select line).Count();
            return $"{reconsiderCount} pairs need reconsideration";
        }

        private bool NeedsReconsideration(string sectionPair, bool contained)
        {
            var match = Regex.Match(sectionPair, @"^(?<startA>\d+)-(?<endA>\d+),(?<startB>\d+)-(?<endB>\d+)$");
            var startA = int.Parse(match.Groups["startA"].Value);
            var endA = int.Parse(match.Groups["endA"].Value);
            var startB = int.Parse(match.Groups["startB"].Value);
            var endB = int.Parse(match.Groups["endB"].Value);

            if(contained)
                return (startA <= startB && endA >= endB) || (startB <= startA && endB >= endA);
            return (startA <= startB && endA >= startB) || (startB <= startA && endB >= startA)||
                (startA <= endB && endA >= endB) || (startB <= endA && endB >= endA);
        }
    }
}
