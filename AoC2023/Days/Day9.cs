using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
internal class Day9: DayBase
{
    public override string Title => "Mirage Maintenance";

    public override string Solve(string input, bool part2)
    {
        long sum = 0;

        foreach(var line in GetLines(input))
        {
            Console.Write(line);
            long value = Extrapolate(line, part2);
            Console.Write(" > ");
            Console.WriteLine(value);
            sum += value;
        }
        return $"The sum of all extrapolated values is {sum}";
    }

    private static long Extrapolate(string line, bool part2)
    {
        List<long> currentList = line.Split(' ').Select(x => long.Parse(x)).ToList();
        List<long> oldList;

        Stack<long> lastDigits = new();

        while(!currentList.TrueForAll(x => x == 0))
        {
            oldList = currentList;
            lastDigits.Push(oldList[part2 ? 0 : ^1]);
            currentList = new List<long>();

            for(int i = 1; i < oldList.Count; i++)
                currentList.Add(oldList[i] - oldList[i - 1]);
        }

        long lastDigit = 0;

        while(lastDigits.Count > 0)
            if(part2)
                lastDigit = lastDigits.Pop() - lastDigit;
            else
                lastDigit += lastDigits.Pop();

        return lastDigit;
    }
}
