using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day14 : DayBase
    {
        public override string Title => "Extended Polymerization";
        ConsoleAssist consoleAssist = new ConsoleAssist();
        Dictionary<string, string> insertionRules;
        int iterationCount;

        // new algorithm thanks to u/mlhpdx and u/udvlp in this reddit thread:
        // https://www.reddit.com/r/adventofcode/comments/rfzq6f/comment/hokc9ma

        public override string Solve(string input, bool part2)
        {
            iterationCount = (part2 ? 40 : 10);
            List<string> polymerPlan = GetGroupedLines(input);
            insertionRules = new Dictionary<string, string>();
            foreach (string rule in GetLines(polymerPlan[1]))
            {
                Match ruleDefinition = Regex.Match(rule, @"([A-Z]+) -> ([A-Z]+)");
                if (!ruleDefinition.Success) throw new FormatException("Malformed rule " + rule);
                insertionRules.Add(ruleDefinition.Groups[1].Value, ruleDefinition.Groups[2].Value);
            }

            string polymer = polymerPlan[0];

            Dictionary<string, ulong> pairs = new Dictionary<string, ulong>();
            Dictionary<string, ulong> elements = new Dictionary<string, ulong>();

            for(int i = 0; i < polymer.Length - 1; i++)
            {
                string group = polymer.Substring(i, 2);
                if(!pairs.ContainsKey(group))
                    pairs.Add(group, 0);
                ++pairs[group];
            }

            foreach(char element in polymer)
            {
                string elementString = element.ToString();
                if (!elements.ContainsKey(elementString))
                    elements.Add(elementString, 0);
                ++elements[elementString];
            }

            for(int i = 0;i < iterationCount; ++i)
            {
                Dictionary<string, ulong> newPairs = new Dictionary<string, ulong>();
                foreach(KeyValuePair<string, ulong> pair in pairs)
                {

                    if (insertionRules.ContainsKey(pair.Key))
                    {
                        string newKey = pair.Key[0] + insertionRules[pair.Key];
                        if(!newPairs.ContainsKey(newKey))
                            newPairs.Add(newKey, 0);
                        newPairs[newKey] += pair.Value;
                        
                        newKey = insertionRules[pair.Key] + pair.Key[1];
                        if(!newPairs.ContainsKey(newKey))
                            newPairs.Add(newKey, 0);
                        newPairs[newKey] += pair.Value;
                        
                        newKey = insertionRules[pair.Key];
                        if(!elements.ContainsKey(newKey))
                            elements.Add(newKey, 0);
                        elements[newKey] += pair.Value;


                    }
                    else newPairs.Add(pair.Key, pair.Value);
                }
                pairs = newPairs;
            }

            ulong mostCommonCount = elements.Values.Max();
            ulong leastCommonCount = elements.Values.Min();


            return $"{mostCommonCount}(mc) - {leastCommonCount}(lc) = {mostCommonCount - leastCommonCount}";
        }

        private string ProcessPolymer(string polymer, int depth)
        {
            if (depth > iterationCount - 1) return polymer;
            StringBuilder newPolymer = new StringBuilder();
            for (int i = polymer.Length - 2; i >= 0 ; --i)
            {
                string polymerPart = polymer.Substring(i, 2);
                newPolymer.Insert(0, polymerPart[1]);
                if (insertionRules.ContainsKey(polymerPart))
                {
                    newPolymer.Insert(0, insertionRules[polymerPart]);
                }
            }
            newPolymer.Insert(0, polymer.First());
            return ProcessPolymer(newPolymer.ToString(), depth + 1);
        }
    }
}
