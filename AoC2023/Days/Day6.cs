using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public partial class Day6: DayBase
{
    public override string Title => "Wait For It";

    public override string Solve(string input, bool part2)
    {
        if(part2)
            input = input.Replace(" ", "");
        var lines = GetLines(input);
        List<long> times = NumberRegEx().Matches(lines[0]).Select(x => long.Parse(x.Value)).ToList();
        List<long> distances = NumberRegEx().Matches(lines[1]).Select(x => long.Parse(x.Value)).ToList();

        long waysToWin = 1;
        for(int game = 0; game < times.Count; game++)
            waysToWin *= SweepGame(times[game], distances[game]);
        return $"Multiplication of all winnable games: {waysToWin}";
    }

    private long SweepGame(long time, long distance)
    {
        long winnableGames = 0;
        for (long speed = 0; speed <= time; ++speed)
        {
            var result = speed * (time - speed);
            if(result > distance)
                winnableGames++;
        }
        return winnableGames;
    }


    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegEx();
}
