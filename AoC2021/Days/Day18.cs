using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day18 : DayBase
    {
        public override string Title => "Snailfish";

        object homework;


        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            foreach (string line in GetLines(input))
            {
                int position = 0;
                homework = homework == null ? BuildNumber(line, ref position) : Tuple.Create(homework, BuildNumber(line, ref position));
                if (position != line.Length - 1) throw new FormatException($"The line {line} was not fully processed. processing stopped at position {position}{Environment.NewLine}{"^".PadLeft(position + 1)}");
                homework = ReduceNumber(homework);
            }
            return "";
        }

        private object BuildNumber(string line, ref int position)
        {
            switch (line[position])
            {
                case '[':
                    ++position;
                    var left = BuildNumber(line, ref position);
                    position += 2;
                    var value = Tuple.Create(left, BuildNumber(line, ref position));
                    ++position;
                    return value;
                case ',':
                    ++position;
                    return BuildNumber(line, ref position);
                default:
                    if (!char.IsDigit(line[position]))
                        throw new ArgumentException(string.Format("Unexpected char '{0}' at position {1} in{2}{3}{2}{4}", line[position], position, Environment.NewLine, line, "^".PadLeft(position + 1)));
                    string number = string.Join(string.Empty, line.Skip(position).TakeWhile(x => char.IsDigit(x)));
                    return int.Parse(number);
            }
        }

        private object ReduceNumber(object homework)
        {
            throw new NotImplementedException();
        }
    }
}
