using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase
    {
        public override string Title => "Rambunctious Recitation";

        public override string Solve(string input, bool part2)
        {
            //Initialize and make an array from the input.
            Dictionary<int, int> numberRound = new Dictionary<int, int>();
            int lastNumber = 0;
            int[] starters = input.Split(',').Select(x => int.Parse(x)).ToArray();

            //Loop until the target height position was reached
            for (int i = 0; i < (part2 ? 30000000 : 2020); i++)
            {
                //while in range of the starter numbers, use these
                if (i < starters.Length)
                {
                    lastNumber = starters[i];
                    //Keep track of their mentioned position
                    if (!numberRound.ContainsKey(lastNumber)) numberRound.Add(lastNumber, 0);
                    numberRound[lastNumber] = i;
                }
                else
                {   //Keep the last number for tracking and check their appearance
                    int previous = lastNumber;
                    if (numberRound.ContainsKey(lastNumber) && numberRound[lastNumber] != i - 1)
                        lastNumber = i - 1 - numberRound[lastNumber];
                    else
                        lastNumber = 0;
                    //Track last appearance of previous number.
                    if (!numberRound.ContainsKey(previous)) numberRound.Add(previous, 0);
                    numberRound[previous] = i - 1;
                }
                // Proof that logging can make things painfully slow :D
                //Console.WriteLine("Turn {0}: {1}", i + 1, lastNumber);
            }
            return string.Format("The 2020th number is: " + lastNumber);
        }
    }
}
