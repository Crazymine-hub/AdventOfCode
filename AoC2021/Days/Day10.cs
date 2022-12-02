using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day10 : DayBase
    {
        public override string Title => "Syntax Scoring";

        private const string openDelimiter = "([{<";
        private const string closeDelimiter = ")]}>";

        Dictionary<char, (long, long)> scores = new Dictionary<char, (long, long)>() {
            { ')', (3, 1)},
            { ']', (57, 2) },
            { '}', (1197, 3) },
            { '>', (25137, 4) }
        };

        public override string Solve(string input, bool part2)
        {
            if (part2) Console.WriteLine("AutoComplete ON");
            long syntaxScore = 0;
            List<long> completionScores = new List<long>();
            foreach (string line in GetLines(input))
            {
                Stack<char> groupStack = new Stack<char>();
                Console.CursorLeft = 4;
                bool aborted = false;
                foreach (char delimiter in line)
                {
                    int openPosition = openDelimiter.IndexOf(delimiter);
                    int closePosition = closeDelimiter.IndexOf(delimiter);
                    Console.Write(delimiter);
                    if (openPosition >= 0)
                        groupStack.Push(delimiter);
                    else if (closePosition >= 0)
                    {
                        if (groupStack.Peek() != openDelimiter[closePosition])
                        {
                            aborted = true;
                            syntaxScore += scores[delimiter].Item1;
                            Console.CursorLeft += 3;
                            Console.Write(" SYNTAX ERROR: EXPECTED " + closeDelimiter[openDelimiter.IndexOf(groupStack.Peek())] + " but got " + delimiter);
                            break;
                        }
                        groupStack.Pop();
                        Console.CursorLeft -= 2;
                        Console.Write("  ");
                        Console.CursorLeft -= 2;
                    }
                }


                if (part2 && !aborted && groupStack.Count > 0)
                {
                    long completeScore = 0;
                    Console.Write(" AC -> ");
                    StringBuilder completion = new StringBuilder();
                    while (groupStack.Count > 0)
                    {
                        char completionChar = closeDelimiter[openDelimiter.IndexOf(groupStack.Pop())];
                        Console.Write(completionChar);
                        completion.Append(completionChar);
                        completeScore *= 5;
                        completeScore += scores[completionChar].Item2;
                    }
                    Console.Write("    Score " + completeScore);
                    completionScores.Add(completeScore);
                }

                Console.CursorLeft = 0;
                if (groupStack.Count > 0)
                {
                    if (aborted)
                        Console.WriteLine("ERR");
                    else
                        Console.WriteLine("INC");
                }
                else
                    Console.WriteLine(" OK ");
            }

            if (part2)
            {
                long completeScore = completionScores.OrderBy(x => x).Skip(completionScores.Count / 2).First();
                return "AutoComplete score: " + completeScore;
            }

            return "Syntax error score: " + syntaxScore;
        }
    }
}
