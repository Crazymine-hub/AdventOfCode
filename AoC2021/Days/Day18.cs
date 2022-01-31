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
            if (part2) return Part2UnavailableMessage;
            foreach (string line in GetLines(input))
            {
                int position = 0;
                ISnailLiteral appended = BuildNumber(line, ref position);
                Console.WriteLine($"---- Appending {appended} ----");
                if (position != line.Length - 1) throw new FormatException($"The line {line} was not fully processed. processing stopped at position {position}{Environment.NewLine}{"^".PadLeft(position + 1)}");

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
                do
                {
                    changeStatus = 0;
                    PrintStartNewReduction();
                    bool mustExplode = pairs.Any(x => x.Depth >= 4);
                    ReduceNumber(homework, mustExplode, ref changeStatus);
                    PrintEndReduction(changeStatus);
                } while (changeStatus != 0);
            }
            return $"Magnitude is: {homework.Magnitude}";
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
                    pairs.Add(value as SnailNumber);
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
                    numbers.Add(parsedNumber);
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
                --Console.CursorLeft;
                Console.Write(" ");
                Console.Write("^".PadLeft(simpleNumber.Value.ToString().Length));
                SplitNumber(simpleNumber, ref changeStatus, false);
                return;
            }

            --Console.CursorLeft;
            Console.Write(" ^");
            SnailNumber number = (SnailNumber)processingNumber;
            ReduceNumber(number.Left, mustExplode, ref changeStatus, depth + 1);
            if (changeStatus != 0) return;

            --Console.CursorLeft;
            Console.Write(" ^");
            ReduceNumber(number.Right, mustExplode, ref changeStatus, depth + 1);
            if (changeStatus != 0) return;
            --Console.CursorLeft;
            Console.Write(" ^");
            ExplodeNumber(number, ref changeStatus, false);
        }

        private void ExplodeNumber(SnailNumber number, ref byte changeStatus, bool needsNewLine)
        {
            SimpleSnailNumber leftNumber = null;
            SimpleSnailNumber rightNumber = null;
            if (number.Depth < 4 || number.Left.GetType() != typeof(SimpleSnailNumber) || number.Right.GetType() != typeof(SimpleSnailNumber)) return;
            if (needsNewLine)
                MakeNewIndicator(changeStatus);
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

        private void SplitNumber(SimpleSnailNumber number, ref byte changeStatus, bool needsNewLine)
        {
            if (number.Value < 10) return;
            if (needsNewLine)
                MakeNewIndicator(changeStatus);
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

        private void PrintStartNewReduction(int indicatorStartPosition = -1)
        {
            Console.Write($"Reducing: {homework}");
            previousLineEnd = Console.CursorLeft;
            Console.WriteLine();
            int top = Console.CursorTop;

            Console.CursorTop = oldHeight;
            Console.Write(string.Empty.PadLeft(Console.WindowWidth - 1));
            if (Console.WindowHeight - 1 > top)
                Console.CursorTop = Console.WindowHeight - 1;
            else
                Console.WriteLine();
            Console.CursorLeft = 0;
            Console.Write($">{homework}");
            oldHeight = Console.CursorTop;

            Console.CursorTop = top;
            if (indicatorStartPosition < 0)
                indicatorStartPosition = "Reducing: ".Length;
            Console.CursorLeft = indicatorStartPosition;
        }

        private void PrintEndReduction(byte changeStatus)
        {
            --Console.CursorTop;
            Console.CursorLeft = previousLineEnd;
            switch (changeStatus)
            {
                case 0: Console.WriteLine("="); break;
                case 1: Console.WriteLine("+"); break;
                case 2: Console.WriteLine("-"); break;
                default: Console.WriteLine("?"); break;
            }
        }

        private void MakeNewIndicator(byte changeStatus)
        {
            int left = Console.CursorLeft - 1;
            PrintEndReduction(changeStatus);
            PrintStartNewReduction(left);
            Console.Write('^');
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
