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
            //Prepare our array to be of the right length (index 0 will be ignored, thus 1 more than required length)
            //the index is the value of the cup and the value is the clockwise next cup in the circle
            if (part2)
                cups = new int[1000_001];
            else
                cups = new int[input.Length + 1];

            //Fill the array with our input cups
            int prevCup = 0;
            foreach (char cup in input)
            {
                int currCup = int.Parse(cup.ToString());
                cups[prevCup] = currCup;
                prevCup = currCup;
            }

            if (part2)
            {//fill our array up to one million
                for (int currCup = cups.Max() + 1; currCup <= 1000_000; ++currCup)
                {
                    cups[prevCup] = currCup;
                    prevCup = currCup;
                }
            }

            //have the last cup loop to the first loop (kept in position 0)
            cups[prevCup] = cups[0];
            //get the minimum and maximum value of our cups (should be 1 an 1.000.000)
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
                        //just display a progress and an estimate of remaining time
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
            //take our 3 cups that are next to the currentCup
            int[] held = new int[3];
            held[0] = cups[currCup];
            held[1] = cups[held[0]];
            held[2] = cups[held[1]];
            cups[currCup] = cups[held[2]];
            int target = currCup;

            //get the next available cup that is one lower and not one of the taken
            do
            {
                --target;
                //if we are below our minimum, reset to our maximum
                if (target < minTarget)     //fun fact: if you use the Min and Max methods here, the runtime increases to several hours.
                    target = maxTarget;     //even funnier fact: it took me mor hours than I'd like to admit to figure that out.
            } while (held.Contains(target));

            //reassign the next cup of our found cup, to be the first one taken out
            int successor = cups[target];
            cups[target] = held[0];
            //reassign the cup, following our last taken out to be the one that originally came after our target
            cups[held[2]] = successor;
            //return the cup, that follows our selected cup.
            return cups[currCup];
        }

        private string PrintCups(int startCup = 1, bool removeStartCup = true)
        {
            int currCup = startCup;
            string result = "";
            //make a string from our cups
            do
            {
                currCup = cups[currCup];
                result += currCup + " ";
            } while (currCup != startCup);

            //our starter cup is the last one, so we just remove the last 2 spaces and the number in between, if requested
            //otherwise, we only remove the last space
            if (removeStartCup)
                result = result.Remove(result.Length - 2 - startCup.ToString().Length);
            else
                result = result.Remove(result.Length - 1);
            return result;
        }
    }
}
