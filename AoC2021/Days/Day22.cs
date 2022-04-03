using AdventOfCode.Tools.DynamicGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day22 : DayBase
    {
        public override string Title => "Reactor Reboot";

        private DynamicGrid<bool> grid = new DynamicGrid<bool>();
        int lowerLimit = int.MinValue;
        int upperLimit = int.MaxValue;

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            lowerLimit = -50;
            upperLimit = 50;
            foreach (string instruction in GetLines(input))
            {
                var parseInfo = Regex.Match(instruction, @"(?<state>on|off) (?:(?:x|y|z)=(?<start>-?\d+)..(?<end>-?\d+),?)+");
                if (!parseInfo.Success) throw new InvalidOperationException("Unable to parse input.");
                ApplyInstruction(parseInfo);
            }

            var cubeCount = grid.Select(x => x.Value ? 1:0).Aggregate((int aggregator, int next) => aggregator += next);

            return $"There are {cubeCount} cubes enabled in the reactor";
        }

        private void ApplyInstruction(Match parseInfo)
        {
            bool targetValue = parseInfo.Groups["state"].Value == "on";
            int zStart = int.Parse(parseInfo.Groups["start"].Captures[2].Value);
            int zEnd = int.Parse(parseInfo.Groups["end"].Captures[2].Value);
            int yStart = int.Parse(parseInfo.Groups["start"].Captures[1].Value);
            int yEnd = int.Parse(parseInfo.Groups["end"].Captures[1].Value);
            int xStart = int.Parse(parseInfo.Groups["start"].Captures[0].Value);
            int xEnd = int.Parse(parseInfo.Groups["end"].Captures[0].Value);
            for (int z = zStart; z <= zEnd; ++z)
            {
                if (z < lowerLimit || z > upperLimit) continue;
                for (int y = yStart; y <= yEnd; ++y)
                {
                    if (y < lowerLimit || y > upperLimit) continue;
                    for (int x = xStart; x <= xEnd; ++x)
                    {
                        if (x < lowerLimit || x > upperLimit) continue;
                        grid.SetRelative(x, y, z, targetValue);
                    }
                }
            }
        }
    }
}
