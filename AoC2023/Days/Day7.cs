using AdventOfCode.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Days.Tools.Day7;

namespace AdventOfCode.Days;
public partial class Day7: DayBase
{
    public override string Title => "Camel Cards";
    readonly List<char> cardOrder = [
        'A',
        'K',
        'Q',
        'J',
        'T',
        '9',
        '8',
        '7',
        '6',
        '5',
        '4',
        '3',
        '2'
    ];

    public override string Solve(string input, bool part2)
    {
        if(part2)
        {
            cardOrder.Remove('J');
            cardOrder.Add('J');
        }

        var games = GetLines(input).Select(x => new CardSet(x, part2)).ToList();
        var rankGroups = games.GroupBy(x => x.Type).OrderBy(x => x.Key).ToList();

        int lastRank = 0;
        foreach(var group in rankGroups)
            RankGames([.. group], ref lastRank);

        if(games.Any(x => x.Rank == 0))
            throw new ResultValidationException("At least one game stayed unranked");

        return $"Your total winnings are {games.Sum(x => x.Prize * x.Rank)}";
    }

    private void RankGames(List<CardSet> group, ref int lastRank, int cardIndex = 0)
    {
        if(group.Count == 1)
        {
            group[0].Rank = ++lastRank;
            return;
        }

        if(cardIndex < 0 || cardIndex > 4) throw new ArgumentOutOfRangeException(nameof(cardIndex));

        var cardSets = group.GroupBy(x => x.Cards[cardIndex]).OrderByDescending(x => cardOrder.IndexOf(x.Key)).ToList();
        foreach(var cardSet in cardSets)
            RankGames([.. cardSet], ref lastRank, cardIndex + 1);
    }
}
