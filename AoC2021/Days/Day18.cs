using AdventOfCode.Days.Tools.Day18;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public class Day18 : DayBase
    {
        public override string Title => "Snailfish";

        ISnailLiteral homework;
        List<SimpleSnailNumber> numbers = new List<SimpleSnailNumber>();
        HashSet<SnailNumber> pairs = new HashSet<SnailNumber>();
        private int previousLineEnd = 0;
        private int oldHeight;

        public override string Solve(string input, bool part2)
        {
            if (part2) return GetMaxMagnitude(input);
            foreach (string line in GetLines(input))
            {
                int position = 0;
                ISnailLiteral appended = BuildNumber(line, ref position);
                if (position != line.Length - 1) throw new FormatException($"The line {line} was not fully processed. processing stopped at position {position}{Environment.NewLine}{"^".PadLeft(position + 1)}");
                Console.WriteLine($"---- Appending {appended} ----");

                if (homework == null)
                    homework = appended;
                else
                {
                    homework = new SnailNumber(homework, appended);
                    pairs.Add(homework as SnailNumber);
                }
                byte changeStatus = 0;
                oldHeight = Console.WindowHeight - 1;
                if (Console.CursorTop > oldHeight)
                    oldHeight = Console.CursorTop + 1;
                RunReduction(homework, true);
            }
            return $"Magnitude is: {homework.Magnitude}";
        }

        private string GetMaxMagnitude(string input)
        {
            List<string> lines = new List<string>();
            foreach (string line in GetLines(input))
                lines.Add(line);

            ISnailLiteral maxMagnitude = null;
            for (int i = 0; i < lines.Count; ++i)
            {
                for (int j = 0; j < lines.Count; ++j)
                {
                    if (i == j) continue;
                    Console.WriteLine($"Adding: {lines[i]} + {lines[j]}");
                    pairs.Clear();
                    numbers.Clear();
                    int position = 0;
                    string leftLine = "";
                    lock (lines)
                    {
                        leftLine = lines[i];
                    }
                    ISnailLiteral left = BuildNumber(leftLine, ref position);
                    if (position != leftLine.Length - 1) throw new FormatException($"The line {leftLine} was not fully processed. processing stopped at position {position}{Environment.NewLine}{"^".PadLeft(position + 1)}");

                    string rightLine = "";
                    lock (lines)
                    {
                        rightLine = lines[j];
                    }
                    position = 0;
                    ISnailLiteral right = BuildNumber(rightLine, ref position);
                    if (position != rightLine.Length - 1) throw new FormatException($"The line {rightLine} was not fully processed. processing stopped at position {position}{Environment.NewLine}{"^".PadLeft(position + 1)}");

                    ISnailLiteral literal = new SnailNumber(left, right);
                    RunReduction(literal, false);

                    if(maxMagnitude == null || maxMagnitude.Magnitude < literal.Magnitude)
                        maxMagnitude = literal;
                }
            }


            if (maxMagnitude == null) return "No magnitude found at all.";

            return $"The maximal Magnitude of {maxMagnitude.Magnitude} can be achieved with {maxMagnitude}";
        }

        private void RunReduction(ISnailLiteral literal, bool writeSteps)
        {
            byte changeStatus;
            do
            {
                changeStatus = 0;
                if (writeSteps)
                    PrintStartNewReduction(literal);
                bool mustExplode = pairs.Any(x => x.Depth >= 4);
                ReduceNumber(literal, mustExplode, ref changeStatus);
                if (writeSteps)
                    PrintEndReduction(changeStatus);
            } while (changeStatus != 0);
        }

        private ISnailLiteral BuildNumber(string line, ref int position)
        {
            switch (line[position])
            {
                case '[':
                    ++position;
                    ISnailLiteral left = BuildNumber(line, ref position);
                    position += 2;
                    ISnailLiteral value = new SnailNumber(left, BuildNumber(line, ref position));
                    lock (pairs)
                    {
                        pairs.Add(value as SnailNumber);
                    }
                    ++position;
                    return value;
                case ',':
                    ++position;
                    return BuildNumber(line, ref position);
                default:
                    if (!char.IsDigit(line[position]))
                        throw new ArgumentException(string.Format("Unexpected char '{0}' at position {1} in{2}{3}{2}{4}", line[position], position, Environment.NewLine, line, "^".PadLeft(position + 1)));
                    string number = string.Join(string.Empty, line.Skip(position).TakeWhile(x => char.IsDigit(x)));

                    //this is madness
                    SimpleSnailNumber parsedNumber = new SimpleSnailNumber(int.Parse(number));
                    lock (numbers)
                    {
                        numbers.Add(parsedNumber);
                    }
                    return parsedNumber;
            }
        }

        private void ReduceNumber(ISnailLiteral processingNumber, bool mustExplode, ref byte changeStatus, int depth = 0)
        {
            Type type = processingNumber.GetType();
            if (type == typeof(SimpleSnailNumber))
            {
                if (mustExplode) return;
                SimpleSnailNumber simpleNumber = (SimpleSnailNumber)processingNumber;
                SplitNumber(simpleNumber, ref changeStatus);
                return;
            }

            SnailNumber number = (SnailNumber)processingNumber;
            ReduceNumber(number.Left, mustExplode, ref changeStatus, depth + 1);
            if (changeStatus != 0) return;

            ReduceNumber(number.Right, mustExplode, ref changeStatus, depth + 1);
            if (changeStatus != 0) return;
            ExplodeNumber(number, ref changeStatus);
        }

        private void ExplodeNumber(SnailNumber number, ref byte changeStatus)
        {
            SimpleSnailNumber leftNumber = null;
            SimpleSnailNumber rightNumber = null;
            if (number.Depth < 4 || number.Left.GetType() != typeof(SimpleSnailNumber) || number.Right.GetType() != typeof(SimpleSnailNumber)) return;
            changeStatus = 1;

            SimpleSnailNumber numberBuf = (SimpleSnailNumber)number.Right;
            int numberIndex = numbers.IndexOf(numberBuf);
            numbers.RemoveAt(numberIndex);
            if (numberIndex < numbers.Count)
            {
                rightNumber = numbers[numberIndex];
                rightNumber.Value += numberBuf.Value;
            }


            numberBuf = (SimpleSnailNumber)number.Left;
            numberIndex = numbers.IndexOf(numberBuf);
            numbers.RemoveAt(numberIndex--);
            if (numberIndex >= 0)
            {
                leftNumber = numbers[numberIndex];
                leftNumber.Value += numberBuf.Value;
            }

            numberBuf = new SimpleSnailNumber(0);
            numbers.Insert(++numberIndex, numberBuf);

            pairs.Remove(number);
            ReplaceNumber(number, numberBuf);
        }

        private void SplitNumber(SimpleSnailNumber number, ref byte changeStatus)
        {
            if (number.Value < 10) return;
            changeStatus = 2;

            int numberIndex = numbers.IndexOf(number);
            numbers.RemoveAt(numberIndex);

            int result = Math.DivRem(number.Value, 2, out int remainder);
            SimpleSnailNumber left = new SimpleSnailNumber(result);
            SimpleSnailNumber right = new SimpleSnailNumber(result + remainder);
            numbers.Insert(numberIndex, right);
            numbers.Insert(numberIndex, left);
            var newNumber = new SnailNumber(left, right);
            pairs.Add(newNumber);
            ReplaceNumber(number, newNumber);
        }

        private void ReplaceNumber(ISnailLiteral previous, ISnailLiteral newValue)
        {
            if (previous.Parent.Left == previous)
                previous.Parent.Left = newValue;
            else if (previous.Parent.Right == previous)
                previous.Parent.Right = newValue;
            else
                throw new InvalidOperationException($"The number {previous} is not part of {previous.Parent}");
        }

        private void PrintStartNewReduction(ISnailLiteral literal)
        {
            Console.Write($"Reducing: {literal}");
        }

        private void PrintEndReduction(byte changeStatus)
        {
            switch (changeStatus)
            {
                case 0: Console.WriteLine("="); break;
                case 1: Console.WriteLine("+"); break;
                case 2: Console.WriteLine("-"); break;
                default: Console.WriteLine("?"); break;
            }
        }

        private IEnumerable<ISnailLiteral> TraverseTree(ISnailLiteral rootLiteral)
        {
            yield return rootLiteral;
            SnailNumber number = rootLiteral as SnailNumber;
            if (number != null)
            {
                foreach (ISnailLiteral literal in TraverseTree(number.Left))
                    yield return literal;
                foreach (ISnailLiteral literal in TraverseTree(number))
                    yield return literal;
            }
        }
    }
}
