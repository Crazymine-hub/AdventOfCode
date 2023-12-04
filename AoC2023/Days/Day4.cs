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
        var cards = GetLines(input)
            .Select(x => x.Split(":", options: splitOptions)[1]
                .Split("|", options: splitOptions)
                .Select(x => x.Split(" ", options: splitOptions))
                .ToList())
            .Select(x => new CardInfo(x))
            .ToList();

        int totalPoints = 0;
        for(int c = 0; c < cards.Count; c++)
        {
            var card = cards[c];
            Console.Write($"Card {c,-5} Copies: {card.Amount,-15}: ");
            foreach(var number in card.Card[0])
                Console.Write($"{number,-3}");
            Console.Write("|");
            foreach(var number in card.Card[1])
                Console.Write($"{number,3}");



            int winNumbers = card.Card[1].Count(x => card.Card[0].Contains(x));
            Console.Write($" => Matches: {winNumbers,-3}");

            if(part2)
            {
                for(int n = c + 1; n <= c + winNumbers; n++)
                    cards[n].Amount += card.Amount;
                Console.WriteLine();
            }
            else
            {
                if(winNumbers == 0)
                {
                    Console.WriteLine($" Points: 0");
                    continue;
                }
                var points = (int)Math.Pow(2, winNumbers - 1);
                Console.WriteLine($" Points: {points}");
                totalPoints += points;
            }
        }

        if(part2)
            return $"Total Cards {cards.Sum(x => x.Amount)}";
        return $"Total Points: {totalPoints}";
    }
}

internal class CardInfo
{
    public List<string[]> Card { get; set; }
    public long Amount { get; set; } = 1;
    public long Processed { get; set; } = 0;

    public CardInfo(List<string[]> card) =>
        Card = card;
}