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
            List<int> adapters = GetLines(input).Select(x => int.Parse(x)).ToList();
            adapters.Add(0);
            adapters.Add(adapters.Max() + 3);
            adapters.Sort();
            if (!part2)
                return CountGaps(adapters);
            else
                return CountCombos(adapters);
        }

        private string CountCombos(List<int> adapters)
        {
            Dictionary<int, long> combinations = new Dictionary<int, long>();
            for(int i = adapters.Count - 1; i >= 0; i--)
            {
                int[] connecting = adapters.Where(x => x > adapters[i] && x <= adapters[i] + 3).ToArray();
                long connections = 0;
                Console.Write(adapters[i]);
                Console.Write(" -> ");
                foreach (int connectingAdapter in connecting)
                {
                    connections += combinations[connectingAdapter];
                    Console.Write(connectingAdapter + "(" + combinations[connectingAdapter] + ")\t");
                }
                Console.WriteLine();
                if (connections == 0) connections = 1;
                combinations.Add(adapters[i], connections);
            }
            return "Possible Connections: " + combinations[0];
        }

        private string CountGaps(List<int> adapters)
        {
            int[] differences = new int[3];
            for (int i = 1; i < adapters.Count; i++)
            {
                int diff = adapters[i] - adapters[i - 1];
                differences[diff - 1]++;
            }
            Console.WriteLine("Adapter Differences:");
            Console.WriteLine("1: " + differences[0]);
            Console.WriteLine("2: " + differences[1]);
            Console.WriteLine("3: " + differences[2]);
            return "Result: " + (differences[0] * differences[2]);
        }
    }
}
