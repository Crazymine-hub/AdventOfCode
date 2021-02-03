using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day23 : DayBase
    {
        public override string Title => "Crab Cups";


        int[] cups;
        int minTarget = 0;
        int maxTarget = 0;
        public override string Solve(string input, bool part2)
        {
            if (part2)
                cups = new int[1000_001];
            else
                cups = new int[input.Length + 1];


            int prevCup = 0;
            foreach (char cup in input)
            {
                int currCup = int.Parse(cup.ToString());
                cups[prevCup] = currCup;
                prevCup = currCup;
            }

            if (part2)
            {
                for (int currCup = cups.Max() + 1; currCup <= 1000_000; ++currCup)
                {
                    cups[prevCup] = currCup;
                    prevCup = currCup;
                }
            }

            cups[prevCup] = cups[0];
            cups[0] = cups.Min();
            minTarget = cups.Min();
            maxTarget = cups.Max();

            int activeCup = int.Parse(input[0].ToString());
            if (part2)
            {
                Stopwatch stopwatch = new Stopwatch();
                double prevProg = -1;
                for (int i = 0; i < 10_000_000; ++i)
                {
                    if (i % 1000 == 0)
                    {
                        TimeSpan eta = new TimeSpan(stopwatch.ElapsedTicks / 100 * (10_000_000 - i));
                        stopwatch.Restart();
                        double progress = i / 10_000_000.0;
                        prevProg = progress;
                        Console.WriteLine(progress.ToString("0.00%"));
                        Console.Write("Remaining: ");
                        Console.Write(string.Format("{0:%d} Days {0:%h} Hours {0:%m} Minutes {0:%s} Seconds", eta));
                        Console.SetCursorPosition(0, 0);
                    }
                    activeCup = DoMove(activeCup);
                }
                long result = cups[1];
                result *= cups[result];
                return "Product of first two cups is :" + result;
            }


            for (int i = 0; i < 100; ++i)
            {
                Console.Write(PrintCups(activeCup, false));
                activeCup = DoMove(activeCup);
                Console.Write(" -> ");
                Console.WriteLine(PrintCups(activeCup, false));
            }

            return "Final cup Order: " + PrintCups();
        }

        private int DoMove(int currCup)
        {
            int[] held = new int[3];
            held[0] = cups[currCup];
            held[1] = cups[held[0]];
            held[2] = cups[held[1]];
            cups[currCup] = cups[held[2]];
            int target = currCup;
            do
            {
                --target;
                if (target < minTarget)
                    target = maxTarget;
            } while (held.Contains(target));

            int successor = cups[target];
            cups[target] = held[0];
            cups[held[2]] = successor;
            return cups[currCup];
        }

        private string PrintCups(int startCup = 1, bool removeStartCup = true)
        {
            int currCup = startCup;
            string result = "";
            do
            {
                currCup = cups[currCup];
                result += currCup + " ";
            } while (currCup != startCup);

            if (removeStartCup)
                result = result.Remove(result.Length - 2 - startCup.ToString().Length);
            else
                result = result.Remove(result.Length - 1);
            return result;
        }
    }
}
