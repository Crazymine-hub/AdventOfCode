using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day23 : DayBase
    {
        public override string Title => "Crab Cups";


        Queue<int> cups = new Queue<int>();
        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";

            foreach (char cup in input)
                cups.Enqueue(int.Parse(cup.ToString()));

            for (int i = 0; i < 100; ++i)
            {
                Console.Write(string.Join(" ", cups));
                DoMove();
                Console.Write(" -> ");
                Console.WriteLine(string.Join(" ", cups));
            }

            while (cups.Peek() != 1)
                cups.Enqueue(cups.Dequeue());
            cups.Dequeue();
            return "Final cup Order: " + string.Join("", cups);
        }

        private void DoMove()
        {
            Queue<int> holding = new Queue<int>();
            int selected = cups.Dequeue();
            int target = selected;
            cups.Enqueue(selected);
            for (int i = 0; i < 3; ++i)
                holding.Enqueue(cups.Dequeue());
            while (!cups.Contains(--target))
                if (target < cups.Min())
                    target = cups.Max() + 1;
            while (cups.Peek() != target)
                cups.Enqueue(cups.Dequeue());
            cups.Enqueue(cups.Dequeue());
            while (holding.Count > 0)
                cups.Enqueue(holding.Dequeue());
            while (cups.Peek() != selected)
                cups.Enqueue(cups.Dequeue());
            cups.Enqueue(cups.Dequeue());
        }
    }
}
