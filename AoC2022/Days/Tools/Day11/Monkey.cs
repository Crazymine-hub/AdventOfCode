using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day11
{
    internal class Monkey
    {
        internal delegate void MonkeyThrowDelegate(int srcMonkeyNumber, int targetMonkey, int item);

        private Queue<int> items;
        private bool operationMultiplies;
        private int? operationOperand;
        private int testDivisor;
        private int targetMonkeyTrue;
        private int targetMonkeyFalse;
        private readonly MonkeyThrowDelegate onMonkeyThrow;

        public int MonkeyNumber { get; }
        public int ItemsCheckedCount { get; private set; } = 0;

        public Monkey(string monkeySpecification, MonkeyThrowDelegate monkeyThrow)
        {
            onMonkeyThrow = monkeyThrow;

            items = new Queue<int>();

            var regExMatch = Regex.Match(monkeySpecification, @"Monkey (\d+):");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse monkey number");
            MonkeyNumber = int.Parse(regExMatch.Groups[1].Value);

            regExMatch = Regex.Match(monkeySpecification, @"Starting items: (?:(\d+)(?:, )?)+");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse starting item list");
            foreach (Capture item in regExMatch.Groups[1].Captures)
                items.Enqueue(int.Parse(item.Value));

            regExMatch = Regex.Match(monkeySpecification, @"Operation: new = old (\+|\*) (old|\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse operation");
            operationMultiplies = regExMatch.Groups[1].Value == "*";
            operationOperand = null;
            if (regExMatch.Groups[2].Value != "old")
                operationOperand = int.Parse(regExMatch.Groups[2].Value);

            regExMatch = Regex.Match(monkeySpecification, @"Test: divisible by (\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse test condition");
            testDivisor = int.Parse(regExMatch.Groups[1].Value);

            regExMatch = Regex.Match(monkeySpecification, @"If true: throw to monkey (\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse target monkey on test success");
            targetMonkeyTrue = int.Parse(regExMatch.Groups[1].Value);

            regExMatch = Regex.Match(monkeySpecification, @"If false: throw to monkey (\d+)");
            if (!regExMatch.Success) throw new ArgumentException("Unable to parse target monkey on test fail");
            targetMonkeyFalse = int.Parse(regExMatch.Groups[1].Value);
        }

        public void RecieveItem(int itemWorry) => items.Enqueue(itemWorry);

        public void ProcessItems()
        {
            while (items.Any())
            {
                ItemsCheckedCount++;
                var item = items.Dequeue();
                var operand = operationOperand ?? item;
                if (operationMultiplies)
                    item *= operand;
                else
                    item += operand;

                item = Convert.ToInt32(Math.Floor(item / 3.0));

                if (item % testDivisor == 0)
                    onMonkeyThrow.Invoke(MonkeyNumber, targetMonkeyTrue, item);
                else
                    onMonkeyThrow.Invoke(MonkeyNumber, targetMonkeyFalse, item);
            }
        }

        public override string ToString() => $"Monkey {MonkeyNumber}: {string.Join(", ", items)} (Total inspected: {ItemsCheckedCount})";
    }
}
