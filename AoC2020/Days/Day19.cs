using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.RegularExpressions; <- My initial solution may or may has not used RegEx.
//using System.Threading.Tasks;         <- Or multitasking. O:D

namespace AdventOfCode.Days
{
    class Day19 : DayBase
    {
        public override string Title => "Monster Messages";

        List<string> rule42 = null;
        List<string> rule31 = null;

        public override string Solve(string input, bool part2)
        {
            var groups = GetGroupedLines(input);
            //read the rules and then analyze the messages.
            CreateRuleset(GetLines(groups[0]));
            int matches = 0;
            foreach (string message in GetLines(groups[1]))
            {
                Console.WriteLine("    :" + message);
                bool isMatched = IsValid(message, part2);
                if (isMatched)
                {
                    ++matches;
                    --Console.CursorTop;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" OK");
                }
                else
                {
                    --Console.CursorTop;
                    Console.CursorLeft = 0;
                    Console.WriteLine("ERR");
                }
            }

            return "Matches: " + matches;
        }

        private void CreateRuleset(List<string> patternRules)
        {
            var resolved = new Queue<(int, List<string>)>();
            List<(int, List<string>)> unresolved = new List<(int, List<string>)>();
            foreach (var pattern in patternRules)
            {   //get the rule number and all it's options
                List<string> rule = new List<string>();
                string[] ruleparts = pattern.Split(':');
                int ruleNr = int.Parse(ruleparts[0]);
                ruleparts = ruleparts[1].Split('|');

                foreach (var expression in ruleparts)
                {
                    string ruleOption = "";
                    //get each expression in this option
                    foreach (var exPart in expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        ruleOption += " " + exPart.Replace("\"", "") + " ";
                    rule.Add(ruleOption);
                }
                //resolved rules only contain letters and no digits
                if (!rule.Any(option => option.Any(char.IsDigit)))
                    resolved.Enqueue((ruleNr, rule));
                else
                    unresolved.Add((ruleNr, rule));
            }


            //Thanks to u/andrewsredditstuff who I took lots of inspiration from to get this part done.
            while (resolved.Count > 0)
            {   //get the next resolved rule and create it's search pattern
                (int ruleNr, List<string> combos) = resolved.Dequeue();
                string pattern = " " + ruleNr + " ";
                for (int i = 0; i < unresolved.Count; ++i)
                {   //find all rules, that contain our resolved rule in at least one option
                    bool updated = false;
                    (int matchRuleNr, List<string> options) = unresolved[i];
                    if (options.Count == 0) continue;
                    while (options.Any(x => x.Contains(pattern)))
                    {
                        List<string> newPatterns = new List<string>();
                        foreach (string option in options)
                        {
                            if (!option.Contains(pattern)) newPatterns.Add(option); //keep this option as is, since it doesn't contain our rule
                            else
                            {
                                foreach (string combo in combos)// our resolved rule contains multiple options aswell
                                {
                                    //find the first occurrance of our resolved rule. An option may contain multiple occurences of our resolved rule.
                                    int pos = option.IndexOf(pattern);
                                    //replace the found occurence with the current option from the resolved rule.
                                    string newRule = option.Substring(0, pos) + " " + combo + " " + option.Substring(pos + pattern.Length);
                                    //Add the option to the new optionset for the active rule
                                    if (newRule.Any(char.IsDigit) || matchRuleNr != 0)
                                        newPatterns.Add(newRule);
                                }
                                updated = true;
                            }
                        }
                        options = newPatterns;
                    }

                    //A rule is fully resoved, when all of its options contain no digits.
                    if (!options.Any(x => x.Any(char.IsDigit)))
                    {
                        //remove the spaces and put it in the right list. (Save rules 42 and 31, the rest are used to resolve more rules)
                        options = options.Select(x => x.Replace(" ", "")).ToList();
                        if (matchRuleNr == 42) rule42 = options;
                        else if (matchRuleNr == 31) rule31 = options;
                        else resolved.Enqueue((matchRuleNr, options));
                        //Empty this rule to avoid trying to resolve it again
                        unresolved[i] = (matchRuleNr, new List<string>());
                    }
                    else if (updated)
                        unresolved[i] = (matchRuleNr, options);
                }
                //remove all emptied rules.
                unresolved.RemoveAll(x => x.Item2.Count == 0);
            }
        }

        private bool IsValid(string message, bool part2)
        { 
            bool InnerMatch11(string messagePart)
            {
                //find the options that match rule 42 and keep their length
                List<int> startConsume = new List<int>();
                startConsume.AddRange(rule42.Where(x => messagePart.StartsWith(x)).Select(x => x.Length).Distinct());

                //find all options, that match rule 31 and keep their start index
                List<int> endMatches = rule31.Where(x => messagePart.EndsWith(x)).Select(x => messagePart.Length - x.Length).ToList();

                //for part 1 we only need to find 1 occurence.
                //the offset index of one 42 option and the start index of one 31 option should be the same
                if (!part2) return startConsume.Distinct().Any(consumed => endMatches.Contains(consumed));

                //in part 2 we need to check all possible offsets against all possible end starting points
                foreach(int start in startConsume)
                    foreach(int end in endMatches)
                    {
                        if (start == end) // start and end are the same. we got a match
                            return true;
                        if (start > end) continue; // start is after the end, perhaps another match?

                        //Recursive call with the so far unmatched part of the string
                        if (InnerMatch11(messagePart.Substring(start, end - start)))
                            return true;
                    }

                //We didn't find anything :(
                return false;
            }

            //this method matches rule 0 hardcoded with the following definitions
            //rule 0 is defined as 8 11
            //rule 8 is defined as 42 or 42 | 42 8 in part 2 (meaning at least one occurence of rule 42)
            //rule 11 is defined as 42 31 or 42 31 | 42 11 31 in part 2 (meaning equal amount of rules 42 and 31)
            //Hartcoding rules normally shouldn't happen, but we don't need to be as flexible.
            //I also tried to do it without hardcoding.
            
            //Match rule 8 and find all possible offsets for rule 42
            List<int> starts = new List<int>();
            List<int> newStarts = null;
            while (newStarts == null || newStarts.Count != 0)
            {
                newStarts = new List<int>();
                int offset = starts.LastOrDefault();
                //reduce the message to the part to be analyzed
                string messagePart = message.Substring(offset);
                foreach (var rule in rule42.Where(x => messagePart.StartsWith(x)))
                    newStarts.Add(rule.Length + offset);
                //add the found offsets to all possible ofssets
                starts.AddRange(newStarts.Distinct());
                //in part 1 we only need to find one occurence
                if (!part2) newStarts = new List<int>();
            }

            // try to match rule 11 against all possible offsets
            foreach(int start in starts)
                if (InnerMatch11(message.Substring(start)))
                    return true;
            return false;
        }

        //private bool MatchesRule(string text, int ruleNr, ref int startPos)
        //{
        //    List<string> msg = new List<string>(),
        //        validMessages = new List<string>();
        //    var rule = rules[ruleNr];
        //    List<int> possibleOptions = new List<int>();
        //    for (int i = 0; i < rule.Count; i++)
        //    {
        //        var option = rule[i];
        //        int offset = startPos;
        //        foreach (var expression in option)
        //        {
        //            Console.Write(" ");
        //            Console.CursorLeft = offset + 5;
        //            Console.Write("^");
        //            --Console.CursorLeft;
        //            if (offset + expression.Length <= text.Length && text.Substring(offset, expression.Length) == expression)
        //            {
        //                offset += expression.Length;
        //                continue;
        //            }
        //            if (int.TryParse(expression, out int reference)|| !MatchesRule(text, reference, ref offset)) break;
        //        }
        //    }
        //    switch (possibleOptions.Count)
        //    {
        //        case 1:
        //            startPos = possibleOptions.ElementAt(0).Value;
        //            return true;
        //        case 2:
        //            return false;
        //        default:
        //            return false;
        //    }
        //}
    }
}
