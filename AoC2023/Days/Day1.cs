using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days;

public class Day1: DayBase
{
    public override string Title => "Trebuchet?!";

    private List<string> digits;


    public override string Solve(string input, bool part2)
    {
        digits = [];
        if(part2)
            digits.AddRange(new string[]{
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine" });

        digits.AddRange(new string[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
        });

        long total = 0;
        foreach(string line in GetLines(input))
        {
            var digit1Data = digits.Select(x => (index: line.IndexOf(x), digit: x)).Where(x => x.index >= 0).Min();
            var digit2Data = digits.Select(x => (index: line.LastIndexOf(x), digit: x)).Where(x => x.index >= 0).Max();

            string digit1 = GetDigitValue(line, digit1Data);
            string digit2 = GetDigitValue(line, digit2Data);
            Console.WriteLine($"{line} => {digit1Data.digit} {digit2Data.digit} => {digit1}{digit2}");
            total += int.Parse(digit1.ToString() + digit2);
        }
        Console.WriteLine();
        return "Sum: " + total.ToString();
    }

    private string GetDigitValue(string line, (int index, string digit) digit1Data) => char.IsDigit(line[digit1Data.index]) ? line[digit1Data.index].ToString() : (digits.IndexOf(digit1Data.digit) + 1).ToString();
}
