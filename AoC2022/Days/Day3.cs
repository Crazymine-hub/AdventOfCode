using AdventOfCode.Tools.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day3 : DayBase
    {
        public override string Title => "Rucksack Reorganization";

        public override string Solve(string input, bool part2)
        {
            var prioritySum = 0;
            var lines = GetLines(input);
            for (int i = 0; i < lines.Count; ++i)
            {
                if (part2)
                {
                    prioritySum += GetPriority(lines.Skip(i).Take(3));
                    i += 2;
                }
                else
                {
                    var line = lines[i];
                    var compartmentSize = line.Length / 2;
                    if (compartmentSize * 2 != line.Length) throw new ArgumentException("Unable to split for compartements.");
                    var comp1 = line.Substring(0, compartmentSize);
                    var comp2 = line.Substring(compartmentSize);
                    prioritySum += GetPriority(new string[] { comp1, comp2 });
                }
            }

            return $"The priority sum id {prioritySum}";
        }

        private int GetPriority(IEnumerable<string> compartments)
        {
            IEnumerable<char> common = compartments.First();
            foreach (var line in compartments)
            {
                common = common.Intersect(line);
                Console.WriteLine(line);
            }
            var score = 0;
            foreach (var item in common)
            {
                Console.Write(item);
                byte itemValue = (byte)item;
                itemValue -= (byte)'A';
                ++itemValue;
                if (itemValue > 26)
                    itemValue -= 32;
                else
                    itemValue += 26;
                score += itemValue;
            }
            Console.Write(' ');
            Console.WriteLine(score);
            Console.WriteLine(string.Empty.PadLeft(10, '='));
            return score;
        }
    }
}
