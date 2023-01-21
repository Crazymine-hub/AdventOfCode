﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day13: DayBase
    {
        public override string Title => "Distress Signal";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            var pairs = GetGroupedLines(input);
            int indexSum = 0;
            for(int i = 0; i < pairs.Count; ++i)
            {
                var pair = GetLines(pairs[i]);
                if (pair.Count != 2) throw new FormatException("The separated content doesn't make a pair.");
                var correctOrder = CheckPair(pair[0], pair[1]);
                if (correctOrder)
                    indexSum += i + 1;
                Console.WriteLine(pair[0]);
                Console.WriteLine(pair[1]);
                Console.WriteLine(correctOrder ? "correct" : "reversed");
                Console.WriteLine();
            }
            return $"The Sum of all right ordered indices is {indexSum}";
        }

        private bool CheckPair(string a, string b)
        {
            int? leftNumber = null;
            int? rightNumber = null;
            for (int i = 0; i < Math.Max(a.Length, b.Length); i++)
            {
                if (a[i] == '[' && b[i] == '[')
                    continue;
                else if ((a[i] == ',' || a[i] == ']') && (b[i] == ',' || b[i] == ']'))
                {
                    if (leftNumber == rightNumber)
                    {
                        leftNumber = null;
                        rightNumber = null;
                        if (a[i] == ']' && b[i] == ',')
                            return true;
                        else if (a[i] == ',' && b[i] == ']')
                            return false;
                        continue;
                    }
                    return (leftNumber ?? -1) < (rightNumber ?? -1);
                }
                else if (a[i] == '[' && char.IsDigit(b[i]))
                {
                    var digitEnd = i;
                    while (char.IsDigit(b[digitEnd]))
                        digitEnd++;
                    b = b.Insert(digitEnd, "]");
                    b = b.Insert(i, "[");
                    --i;
                }
                else if (char.IsDigit(a[i]) && b[i] == '[')
                {
                    var digitEnd = i;
                    while (char.IsDigit(a[digitEnd]))
                        digitEnd++;
                    a = a.Insert(digitEnd, "]");
                    a = a.Insert(i, "[");
                    --i;
                }
                else if (char.IsDigit(a[i]) || char.IsDigit(b[i]))
                {
                    if (char.IsDigit(a[i]))
                    {
                        leftNumber = leftNumber * 10 ?? 0;
                        leftNumber += int.Parse(a[i].ToString());
                    }
                    else
                        a = a.Insert(i, " ");

                    if (char.IsDigit(b[i]))
                    {
                        rightNumber = rightNumber * 10 ?? 0;
                        rightNumber += int.Parse(b[i].ToString());
                    }
                    else
                        b = b.Insert(i, " ");
                }
                else if (a[i] == ']' && b[i] != ']')
                    return true;
                else if (a[i] != ']' && b[i] == ']')
                    return false;
                else throw new FormatException("Not a covered Case");
            }

            throw new InvalidOperationException("The Arrays could not be ordered.");
        }
    }
}
