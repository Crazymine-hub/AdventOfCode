using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public override string Title => "Sonar Sweep";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            int[] depths = GetLines(input).Select(x => int.Parse(x)).ToArray();
            return GetFalloff(depths);
        }

        private string GetFalloff(int[] depths)
        {
            int previos = depths.First();
            int increase = 0;
            foreach(int depthPoint in depths)
            {
                if(depthPoint > previos)
                    increase++;
                previos = depthPoint;
            }
            return $"Depth Increasing Rate: {increase}";
        }
    }
}
