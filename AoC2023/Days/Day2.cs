using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days;

public class Day2: DayBase
{
    public override string Title => "Cube Conundrum";

    private static readonly Regex gameMatch = new(@"^Game (?<gameId>\d+): (?<game>.*)$");
    private static readonly Regex cubeMatch = new(@"(?<amount>\d+) (?<color>(?:red|green|blue))");

    public override string Solve(string input, bool part2)
    {
        Dictionary<string, int> totalDice = new() {
            { "red", part2 ? 0 : 12 },
            { "green", part2 ? 0: 13 },
            { "blue", part2 ? 0: 14 }
        };

        long gameSum = 0;
        foreach(string gameNote in GetLines(input))
        {
            var gameDescription = gameMatch.Match(gameNote);
            int gameId = int.Parse(gameDescription.Groups["gameId"].Value);
            string game = gameDescription.Groups["game"].Value;
            Console.Write($"Analyzing Game {gameId}".PadRight(20));

            if(part2)
                gameSum += GetGamePower(game);
            else
                gameSum += IsGamePossible(game, totalDice) ? gameId : 0;
        }

        Console.WriteLine();
        return $"Sum of all possible games: {gameSum}";
    }


    private static bool IsGamePossible(string game, Dictionary<string, int> diceLimit)
    {
        string[] steps = game.Split(';');
        foreach(string step in steps)
        {
            var cubes = MatchCubes(step);
            if(cubes.Any(x => x.amount > diceLimit[x.color]))
            {
                Console.WriteLine($"=> Not possible");
                return false;
            }
        }
        Console.WriteLine("=> Possible");
        return true;
    }

    private long GetGamePower(string game)
    {
        Dictionary<string, int> gameCubes = MatchCubes(game).GroupBy(x => x.color).ToDictionary(x => x.Key, x => x.Max(x => x.amount));
        var power = gameCubes.Select(x => (long)x.Value).Aggregate((prod, x) => x * prod);
        Console.WriteLine($"=> Power: {power}");
        return power;
    }

    private static IEnumerable<(int amount, string color)> MatchCubes(string game) =>
        cubeMatch.Matches(game)
            .Select(cube => (amount: int.Parse(cube.Groups["amount"].Value), color: cube.Groups["color"].Value));

}
