using System;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days.Tools.Day7;
internal sealed class CardSet
{
    public enum HandType
    {
        HighCard,//?????
        OnePair,//AA???
        TwoPair,//AABB?
        ThreeOfAKind,//AAA??
        FullHouse,//AAABB
        FourOfAKind,//AAAA?
        FiveOfAKind,//AAAAA
    }

    public string Cards { get; }

    public int Prize { get; }
    public HandType Type { get; }

    public int Rank { get; set; } = 0;

    public CardSet(string game, bool part2)
    {
        var gameDetails = game.Split(' ');
        Cards = gameDetails[0];
        Prize = int.Parse(gameDetails[1]);
        var cardGroups = Cards.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        if(part2 && cardGroups.ContainsKey('J') && cardGroups.Keys.Count > 1)
        {
            var joker = cardGroups.Single(x => x.Key == 'J').Value;
            cardGroups.Remove('J');
            var optimalKey = cardGroups.MaxBy(x => x.Value).Key;
            cardGroups[optimalKey] += joker;
        }
        Type = cardGroups.Count switch
        {
            5 => HandType.HighCard,
            4 => HandType.OnePair,
            3 => cardGroups.Any(x => x.Value == 2) ? HandType.TwoPair : HandType.ThreeOfAKind,
            2 => cardGroups.Any(x => x.Value == 2) ? HandType.FullHouse : HandType.FourOfAKind,
            1 => HandType.FiveOfAKind,
            _ => throw new InvalidDataException($"The Hand {Cards} has {cardGroups.Count} different cards. How?")
        };
    }
}