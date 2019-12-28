using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Tools;

namespace AdventOfCode.Days
{
    class Day4 : IDay
    {
        private ulong[] end;
        private ulong start;
        private bool isPart2;

        public string Solve(string input, bool part2)
        {
            isPart2 = part2;
            string[] range = input.Split('-');
            if (range.Length != 2)
                throw new ArgumentOutOfRangeException("input", "Only two Values allowed!");
            if (!ulong.TryParse(range[0], out start) || !ulong.TryParse(range[1], out ulong uLimit))
                throw new ArgumentException("Input is not a Number!", "input");
            if (range[0].Length != 6 || range[1].Length != 6)
                throw new ArgumentOutOfRangeException("input", "The numbers must be exactly 6 digits long!");
            if (uLimit <= start)
                throw new ArgumentOutOfRangeException("input", "The Selected range is 0 or negative!");

            end = GetLastValue(uLimit);

            ulong[] currNumber = GetFirstValue(NumberLists.MakeArray(start));
            int doublePos = GetDoublePos(currNumber, out ulong doubleValue);
            bool endReached = true;
            List<ulong> values = new List<ulong>();
            List<ulong> discarded = new List<ulong>();
            do
            {
                if (GetDoublePos(currNumber) != -1)
                    values.Add(NumberLists.MakeNumber(currNumber));
                else
                    discarded.Add(NumberLists.MakeNumber(currNumber));
                currNumber = IncreaseValueAt(currNumber, currNumber.Length - 1);
                endReached = true;
                for (int i = 0; i < currNumber.Length; i++)
                {
                    if (currNumber[i] < end[i])
                    {
                        endReached = false;
                        continue;
                    }
                }
            } while (!endReached);

            ulong endValue = NumberLists.MakeNumber(currNumber);
            if (endValue <= uLimit && GetDoublePos(currNumber) != -1)
                values.Add(endValue);
            else
                discarded.Add(endValue);

            StringBuilder valueList = new StringBuilder();
            foreach (ulong value in values)
                valueList.AppendLine(value.ToString());
            valueList.AppendLine("DISCARDED:");
            foreach (ulong value in discarded)
                valueList.AppendLine(value.ToString());

            return string.Format("{0}\r\nFound {1} Values:\r\n", valueList.ToString(), values.Count);
        }

        private ulong[] IncreaseValueAt(ulong[] number, int position)
        {
            number[position]++;
            if (number[position] >= 10)
            {
                if (position == 0)
                    throw new OverflowException("Rollover at first digit!");
                number = IncreaseValueAt(number, position - 1);
                number[position] = number[position - 1];
            }
            return number;
        }

        private int GetDoublePos(ulong[] number)
        {
            return GetDoublePos(number, out ulong ignoredValue);
        }

        private int GetDoublePos(ulong[] number, out ulong value)
        {
            value = 0;
            bool skipPair = false;
            ulong lastFind = 0;
            int result = -1;
            for (int i = 0; i < number.Length - 1; i++)
            {
                if (skipPair)
                {
                    skipPair = false;
                    continue;
                }
                if (number[i] == number[i + 1])
                {
                    if (isPart2)
                    {
                        if ((i + 2 < number.Length && number[i + 2] == number[i] ) || number[i] == lastFind)
                        {
                            skipPair = true;
                            lastFind = number[i];
                            result = -1;
                            continue;
                        }
                    }
                    value = number[i];
                    result = i;
                    return result;
                }
            }
            return result;
        }

        private ulong[] GetFirstValue(ulong[] number)
        {
            ulong currdigit = 0;
            for (int i = 1; i < number.Length; i++)
            {
                if (number[i] < number[i - 1] || currdigit != 0)
                {
                    if (currdigit == 0)
                        currdigit = number[i - 1];
                    number[i] = currdigit;
                }
            }
            return number;
        }

        private ulong[] GetLastValue(ulong upper)
        {
            ulong[] number = GetFirstValue(NumberLists.MakeArray(upper));
            if (NumberLists.MakeNumber(number) > upper)
            {
                ulong maxVal = number[0] - 1;
                number = NumberLists.SetAll(number, 9);
                number[0] = maxVal;
            }
            return number;
        }
    }
}
