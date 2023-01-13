using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day11
{
    internal class Monkey
    {
        internal delegate void MonkeyThrowDelegate(int srcMonkeyNumber, int targetMonkey, long item);

        private Queue<long> items;
        private bool operationMultiplies;
        private long? operationOperand;
        private long testDivisor;
        private int targetMonkeyTrue;
        private int targetMonkeyFalse;
        private readonly MonkeyThrowDelegate onMonkeyThrow;
        private readonly bool worryDecay;

        public int MonkeyNumber { get; }
        public long ItemsCheckedCount { get; private set; } = 0;
        public long WorryCap { get; set; }
        public long WorryTest => testDivisor;

        public Monkey(string monkeySpecification, MonkeyThrowDelegate monkeyThrow, bool worryDecay)
        {
            onMonkeyThrow = monkeyThrow;
            this.worryDecay = worryDecay;
            items = new Queue<long>();

            var regExMatch = Regex.Match(monkeySpecification, @"Monkey (\d+):");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse monkey number");
            MonkeyNumber = int.Parse(regExMatch.Groups[1].Value);

            regExMatch = Regex.Match(monkeySpecification, @"Starting items: (?:(\d+)(?:, )?)+");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse starting item list");
            foreach (Capture item in regExMatch.Groups[1].Captures)
                items.Enqueue(long.Parse(item.Value));

            regExMatch = Regex.Match(monkeySpecification, @"Operation: new = old (\+|\*) (old|\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse operation");
            operationMultiplies = regExMatch.Groups[1].Value == "*";
            operationOperand = null;
            if (regExMatch.Groups[2].Value != "old")
                operationOperand = long.Parse(regExMatch.Groups[2].Value);

            regExMatch = Regex.Match(monkeySpecification, @"Test: divisible by (\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse test condition");
            testDivisor = long.Parse(regExMatch.Groups[1].Value);

            regExMatch = Regex.Match(monkeySpecification, @"If true: throw to monkey (\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse target monkey on test success");
            targetMonkeyTrue = int.Parse(regExMatch.Groups[1].Value);

            regExMatch = Regex.Match(monkeySpecification, @"If false: throw to monkey (\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse target monkey on test fail");
            targetMonkeyFalse = int.Parse(regExMatch.Groups[1].Value);
        }

        public void RecieveItem(long itemWorry) => items.Enqueue(itemWorry);

        public void ProcessItems()
        {
            while (items.Any())
            {
                ItemsCheckedCount++;
                var item = items.Dequeue();
                var operand = operationOperand ?? item;
                checked
                {
                    if (operationMultiplies)
                        item *= operand;
                    else
                        item += operand;

                    if (worryDecay)
                        item = Convert.ToInt64(Math.Floor(item / 3.0M));
                }

                item %= WorryCap;

                if (item % testDivisor == 0)
                    onMonkeyThrow.Invoke(MonkeyNumber, targetMonkeyTrue, item);
                else
                    onMonkeyThrow.Invoke(MonkeyNumber, targetMonkeyFalse, item);
            }
        }

        public override string ToString() => $"Monkey {MonkeyNumber} inspected items {ItemsCheckedCount} times";
    }
}
