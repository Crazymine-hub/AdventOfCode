using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day10 : DayBase
    {
        public override string Title => "Adapter Array";


        public override string Solve(string input, bool part2)
        {
            //Read all adapters as Int and add assumed adapters (outlet and device)
            List<int> adapters = GetLines(input).Select(x => int.Parse(x)).ToList();
            adapters.Add(0);
            adapters.Add(adapters.Max() + 3);
            //Sort adapters and solve
            adapters.Sort();
            if (!part2)
                return CountGaps(adapters);
            else
                return CountCombos(adapters);
        }

        private string CountGaps(List<int> adapters)
        {
            //calculate the joltage gap between adapters and count how many of each possible gaps (1-3) exist
            int[] differences = new int[3];
            for (int i = 1; i < adapters.Count; i++)
            {
                int diff = adapters[i] - adapters[i - 1];
                differences[diff - 1]++;
            }

            //Print Counter and the result for solving the puzzle
            Console.WriteLine("Adapter Differences:");
            Console.WriteLine("1: " + differences[0]);
            Console.WriteLine("2: " + differences[1]);
            Console.WriteLine("3: " + differences[2]);
            return "Result: " + (differences[0] * differences[2]);
        }

        private string CountCombos(List<int> adapters)
        {
            //Keep all possible connections for each adapter in this dictionary
            Dictionary<int, long> combinations = new Dictionary<int, long>();
            //Start with the last adapter and then go backwards
            for(int i = adapters.Count - 1; i >= 0; i--)
            {
                //find all adapters, that can connect to this adapter
                int[] connecting = adapters.Where(x => x > adapters[i] && x <= adapters[i] + 3).ToArray();
                long connections = 0;
                Console.Write(adapters[i]);
                Console.Write(" -> ");
                //Get their connection possibilities and add them up
                foreach (int connectingAdapter in connecting)
                {
                    connections += combinations[connectingAdapter];
                    Console.Write(connectingAdapter + "(" + combinations[connectingAdapter] + ")\t");
                }
                Console.WriteLine();
                //The first used adapter (device) needs to have a connection count of 1 instead the detected 0 for the math to work
                if (connections == 0) connections = 1;
                //make the calculated connection possibilities this adapters connection possibillities.
                combinations.Add(adapters[i], connections);
            }
            return "Possible Connections: " + combinations[0];
        }
    }
}
