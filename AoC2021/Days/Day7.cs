using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    internal class Day7 : DayBase
    {
        public override string Title => "The Treachery of Whales";

        int[] positions;

        //i just couldn't resist, doing this with LINQ....
        // next one will be normal again, promise
        public override string Solve(string input, bool part2)
        {
            positions = input.Split(',').Select(x => int.Parse(x)).ToArray();
            //The position must be somewhere in between. outside wouldn't make much sense.
            int result = Enumerable.Range(positions.Min(), positions.Max())
                //calculate the fuel usage,if they were to move to that position.
                .Select(x =>
                    positions.Select(y =>
                    {
                        int difference = Math.Abs(y - x);
                        if (part2)
                            difference = difference * (difference + 1) / 2;
                        return difference;
                    })
                    //Add up the fuel usage for this step
                    .Aggregate((int current, int next) => next + current))
                .Min();
            return $"Cheapest fuel cost {result}";
        }
    }
}
