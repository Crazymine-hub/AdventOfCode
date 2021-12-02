using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    internal class Day2 : DayBase
    {
        public override string Title => "Dive!";

        int depth = 0;
        int left = 0;
        int aim = 0;
        bool part2 = false;


        public override string Solve(string input, bool part2)
        {
            this.part2 = part2;
            List<string> instructions = GetLines(input);
            Console.WriteLine("Diving");
            foreach (string instruction in instructions)
                FollowInstruction(instruction);
            return $"Went to x:{left} y:{depth} PositionNr: {left * depth}";
        }

        private void FollowInstruction(string instruction)
        {
            Match parsed = Regex.Match(instruction, @"([a-z]+) (\d+)");
            int value = int.Parse(parsed.Groups[2].Value);
            if (!part2)
                Console.WriteLine($"Going {parsed.Groups[1].Value} for {value} units.");
            switch (parsed.Groups[1].Value)
            {
                case "forward":
                    if (part2)
                    {
                        Console.WriteLine($"Going for Aim {aim} and {value} units.");
                        depth += aim * value;
                    }
                    left += value;
                    break;
                case "up":
                    if (part2)
                    {
                        Console.WriteLine($"Aiming up {value} units.");
                        aim -= value;
                        break;
                    }
                    depth -= value;
                    break;
                case "down":
                    if (part2)
                    {
                        Console.WriteLine($"Aiming down {value} units.");
                        aim += value;
                        break;
                    }
                    depth += value;
                    break;
                default:
                    throw new Exception($"Unexpected Value: {parsed.Groups[1].Value}");
            }

        }
    }
}
