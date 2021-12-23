using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day14 : DayBase
    {
        public override string Title => "Extended Polymerization";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            List<string> polymerPlan = GetGroupedLines(input);
            Dictionary<string, string> insertionRules = new Dictionary<string, string>();
            foreach (string rule in GetLines(polymerPlan[1]))
            {
                Match ruleDefinition = Regex.Match(rule, @"([A-Z]+) -> ([A-Z]+)");
                if (!ruleDefinition.Success) throw new FormatException("Malformed rule " + rule);
                insertionRules.Add(ruleDefinition.Groups[1].Value, ruleDefinition.Groups[2].Value);
            }

            string polymer = polymerPlan[0];

            for (int i = 0; i < 10; ++i)
            {
                Console.WriteLine(polymer);
                StringBuilder newPolymer = new StringBuilder();
                for (int j = 0; j < polymer.Length - 1; ++j)
                {
                    string block = polymer[j].ToString() + polymer[j + 1];
                    newPolymer.Append(block[0]);
                    if (!insertionRules.ContainsKey(block)) continue;
                    newPolymer.Append(insertionRules[block]);
                }
                newPolymer.Append(polymer.Last());
                polymer = newPolymer.ToString();
            }
            Console.WriteLine("");
            Console.WriteLine("FINAL POLYMER:");
            Console.WriteLine(polymer);

            var elements = polymer.GroupBy(x => x);
            int leastCommonCount = elements.Min(x => x.Count());
            int mostCommonCount = elements.Max(x => x.Count());
            return $"{mostCommonCount}(mc) - {leastCommonCount}(lc) = {mostCommonCount - leastCommonCount}";
        }
    }
}
