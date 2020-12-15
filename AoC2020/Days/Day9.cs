using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day9 : DayBase
    {
        public override string Title => "Encoding Error";
        const int praembelLength = 25;

        public override string Solve(string input, bool part2)
        {
            List<long> praembel = new List<long>();
            List<long> numbers = new List<long>();
            long? invalidNumber = null;
            foreach (string line in GetLines(input))
            {
                long number = long.Parse(line);
                if (invalidNumber == null)
                {
                    if (praembel.Count >= praembelLength)
                    {
                        if (!InPraembel(number, praembel)) invalidNumber = number;
                        praembel.RemoveAt(0);
                    }
                    praembel.Add(number);
                }
                numbers.Add(number);
            }

            if (invalidNumber == null) return "No invalid number found!";

            if (part2)
            {
                int numLength = 2;
                while(numLength < numbers.Count)
                {
                    for(int pos = 0; pos < numbers.Count - numLength; pos++)
                    {
                        long sum = 0;
                        List<long> currNumbers = new List<long>();
                        for (int i = pos; i < numLength; i++)
                        {
                            currNumbers.Add(numbers[i]);
                            sum += numbers[i];
                        }
                        if (sum == invalidNumber)
                            return "The weakness is: " + (currNumbers.Min() + currNumbers.Max());
                    }
                    numLength++;
                }
                return "The Weakness could not be found!";
            }
            else
            {
                return "The Invalid number is: " + invalidNumber?.ToString();
            }
        }

        private bool InPraembel(long number, List<long> praembel)
        {
            return MathHelper.GetFactorsByResult(praembel.ToArray(), 2, number, false, out long sum) != null;
        }
    }
}
