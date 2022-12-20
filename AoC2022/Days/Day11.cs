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
            if (part2) return Part2UnavailableMessage;

            foreach (var monkeyStatus in GetGroupedLines(input))
                try
                {
                    monkeys.Add(new Monkey(monkeyStatus, MonkeyThrowHandler));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Monkey Data failed: " + monkeyStatus, ex);
                }

            for (int i = 0; i < 20; ++i)
            {
                foreach (Monkey monkey in monkeys)
                    monkey.ProcessItems();

                Console.WriteLine(string.Join(Environment.NewLine, monkeys.OrderByDescending(x => x.ItemsCheckedCount).Select(x => x.ToString())));
                Console.WriteLine();
            }

            return "Multiplied most active inspections: " + monkeys
                .OrderByDescending(x => x.ItemsCheckedCount)
                .Take(2)
                .Select(x => x.ItemsCheckedCount)
                .Aggregate((accumulator, value) => accumulator * value)
                .ToString();
        }

        private void MonkeyThrowHandler(int monkeyNumber, int targetMonkey, int item)
        {
            Console.WriteLine($"Monkey {monkeyNumber} -> {item} -> {targetMonkey}");
            monkeys[targetMonkey].RecieveItem(item);
        } 
    }
}
