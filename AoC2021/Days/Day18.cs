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


        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            foreach (string line in GetLines(input))
            {
                int position = 0;
                ISnailLiteral appended = BuildNumber(line, ref position);
                Console.WriteLine($"---- Appending {appended} ----");
                Task.Delay(1000).Wait();

                homework = homework == null ? appended : new SnailNumber(homework, appended);
                if (position != line.Length - 1) throw new FormatException($"The line {line} was not fully processed. processing stopped at position {position}{Environment.NewLine}{"^".PadLeft(position + 1)}");
                byte changeStatus = 0;
                int oldHeight = Console.WindowHeight - 1;
                if(Console.CursorTop > oldHeight)
                    oldHeight = Console.CursorTop + 1;
                do
                {
                    Console.Write($"Reducing: {homework}");
                    int left = Console.CursorLeft;
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
                    Console.CursorLeft = "Reducing: ".Length;
                    changeStatus = 0;
                    homework = ReduceNumber(homework, ref changeStatus);

                    --Console.CursorTop;
                    Console.CursorLeft = left;
                    switch (changeStatus)
                    {
                        case 0: Console.WriteLine("="); break;
                        case 1: Console.WriteLine("+"); break;
                        case 2: Console.WriteLine("-"); break;
                        default: Console.WriteLine("?"); break;
                    }
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

        private ISnailLiteral ReduceNumber(ISnailLiteral processingNumber, ref byte changeStatus, int depth = 0)
        {
            Type type = processingNumber.GetType();
            if (type == typeof(SimpleSnailNumber))
                return SplitNumber((SimpleSnailNumber)processingNumber, ref changeStatus);

            --Console.CursorLeft;
            Console.Write(" ^");
            SnailNumber number = (SnailNumber)processingNumber;
            number.Left = ReduceNumber(number.Left, ref changeStatus, depth + 1);
            if (changeStatus != 0) return number;

            --Console.CursorLeft;
            Console.Write(" ^");
            number.Right = ReduceNumber(number.Right, ref changeStatus, depth + 1);
            if (changeStatus != 0) return number;
            if (depth >= 4) return ExplodeNumber(number, ref changeStatus);

            --Console.CursorLeft;
            Console.Write(" ^");
            return number;
        }

        private ISnailLiteral ExplodeNumber(SnailNumber number, ref byte changeStatus)
        {
            if (number.Left.GetType() != typeof(SimpleSnailNumber) || number.Right.GetType() != typeof(SimpleSnailNumber)) return number;
            changeStatus = 1;

            SimpleSnailNumber numberBuf = (SimpleSnailNumber)number.Right;
            int numberIndex = numbers.IndexOf(numberBuf);
            numbers.RemoveAt(numberIndex);
            if (numberIndex < numbers.Count)
                numbers[numberIndex].Value += numberBuf.Value;
            numberBuf = (SimpleSnailNumber)number.Left;
            numberIndex = numbers.IndexOf(numberBuf);
            numbers.RemoveAt(numberIndex--);
            if (numberIndex >= 0)
                numbers[numberIndex].Value += numberBuf.Value;
            numberBuf = new SimpleSnailNumber(0);
            numbers.Insert(++numberIndex, numberBuf);
            return numberBuf;
        }

        private ISnailLiteral SplitNumber(SimpleSnailNumber number, ref byte changeStatus)
        {
            --Console.CursorLeft;
            Console.Write(" ");
            Console.Write("^".PadLeft(number.Value.ToString().Length));
            if (number.Value < 10) return number;
            changeStatus = 2;

            int numberIndex = numbers.IndexOf(number);
            numbers.RemoveAt(numberIndex);

            int result = Math.DivRem(number.Value, 2, out int remainder);
            SimpleSnailNumber left = new SimpleSnailNumber(result);
            SimpleSnailNumber right = new SimpleSnailNumber(result + remainder);
            numbers.Insert(numberIndex, right);
            numbers.Insert(numberIndex, left);
            return new SnailNumber(left, right);
        }
    }
}
