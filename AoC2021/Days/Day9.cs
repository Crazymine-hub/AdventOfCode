using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day9 : DayBase
    {
        public override string Title => "Smoke Basin";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            var grid = GetLines(input).Select(line => line.Select(pos => int.Parse(pos.ToString())).ToArray()).ToArray();
            int lowSum = 0;
            for (int y = 0; y < grid.Length; ++y)
            {
                for (int x = 0; x < grid[y].Length; ++x)
                {
                    int above = int.MaxValue;
                    int below = int.MaxValue;
                    int left = int.MaxValue;
                    int right = int.MaxValue;
                    if (y > 0)
                        above = grid[y - 1][x];
                    if (y < grid.Length - 1)
                        below = grid[y + 1][x];
                    if (x > 0)
                        left = grid[y][x - 1];
                    if (x < grid[y].Length - 1)
                        right = grid[y][x + 1];
                    if(grid[y][x] < above && grid[y][x] < below && grid[y][x] < left && grid[y][x] < right) 
                        lowSum += grid[y][x] + 1;
                }
            }
            return "Risk Level: " + lowSum;
        }
    }
}
