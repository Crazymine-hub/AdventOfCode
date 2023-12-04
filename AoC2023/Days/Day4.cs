using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public class Day4: DayBase
{
    public override string Title => "Scratchcards";

    private StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    public override string Solve(string input, bool part2)
    {
        if(part2) return Part2UnavailableMessage;
        var cards = GetLines(input)
            .Select(x => x.Split(":", options: splitOptions)[1]
                .Split("|", options: splitOptions)
                .Select(x => x.Split(" ", options: splitOptions))
                .ToList())
            .ToList();

        int totalPoints = 0;
        foreach(var card in cards)
        {
            var numbers = card.SelectMany(x => x).ToList();
            for(int i = 0; i < numbers.Count; ++i)
            {
                Console.Write(numbers[i].ToString().PadRight(3));
                if(i > 0 && i == numbers.Count % 2)
                    Console.Write("| ");
            }
            int winNumbers = card[1].Count(x => card[0].Contains(x));

            Console.Write($"=> Matches: {winNumbers,-3}");
            if(winNumbers == 0)
            {
                Console.WriteLine($" Points: 0");
                continue;
            }
            var points = (int)Math.Pow(2, winNumbers - 1);
            Console.WriteLine($" Points: {points}");
            totalPoints += points;
        }

        return $"Total Points: {totalPoints}";
    }
}
