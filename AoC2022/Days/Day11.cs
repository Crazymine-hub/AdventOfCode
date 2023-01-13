using AdventOfCode.Days.Tools.Day11;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day11 : DayBase
    {
        public override string Title => "Monkey in the Middle";
        List<Monkey> monkeys = new List<Monkey>();

        public override string Solve(string input, bool part2)
        {
            foreach (var monkeyStatus in GetGroupedLines(input))
                try
                {
                    monkeys.Add(new Monkey(monkeyStatus, MonkeyThrowHandler, !part2));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Monkey Data failed: " + monkeyStatus, ex);
                }

            long worryCap = AdventOfCode.Tools.MathHelper.LeastCommonMultiple(monkeys.Select(x => x.WorryTest));
            foreach (var monkey in monkeys)
                monkey.WorryCap = worryCap;

            for (int i = 1; i <= (part2 ? 10_000 : 20); ++i)
            {
                foreach (Monkey monkey in monkeys)
                    monkey.ProcessItems();

                Console.WriteLine($"== After round {i} ==");
                Console.WriteLine(string.Join(Environment.NewLine, monkeys.Select(x => x.ToString())));
                Console.WriteLine();
            }

            return "Level of monkey business: " + monkeys
                .OrderByDescending(x => x.ItemsCheckedCount)
                .Take(2)
                .Select(x => x.ItemsCheckedCount)
                .Aggregate((accumulator, value) => accumulator * value)
                .ToString();
        }

        private void MonkeyThrowHandler(int monkeyNumber, int targetMonkey, long item)
        {
            monkeys[targetMonkey].RecieveItem(item);
        }
    }
}
