using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
internal partial class Day5: DayBase
{
    public override string Title => "If You Give A Seed A Fertilizer";

    public override string Solve(string input, bool part2)
    {
        if(part2) return Part2UnavailableMessage;

        var maps = GetGroupedLines(input);
        var data = InitSeedData(maps[0], part2);

        foreach(var map in maps.Skip(1))
        {
            var ranges = GetLines(map);
            var header = Regex.Match(ranges[0], @"(?<srcCol>\w+)-to-(?<targetCol>\w+) map:");
            var fromCol = header.Groups["srcCol"].Value;
            var toCol = header.Groups["targetCol"].Value;

            foreach(var range in ranges.Skip(1))
            {
                var rangeDefinimtion = range
                    .Split(' ')
                    .Select(x => long.Parse(x.Trim()))
                    .ToList();
                var start = rangeDefinimtion[1];
                var end = start + rangeDefinimtion[2] - 1;
                foreach(var startRange in data[fromCol])
                {
                    if(startRange.Start < start || startRange.Start > end) continue;
                    data[toCol].Add(new LongRange(rangeDefinimtion[0] + (startRange.Start - start), 0));
                    startRange.Transferred = true;
                }
            }
            data[toCol].AddRange(data[fromCol].Where(x => !x.Transferred).Select(x => new LongRange(x.Start, x.End)));
        }

        Console.WriteLine(string.Join(" | ", data.Keys.Select(x => x.PadLeft(14))));
        for(int i = 0; data.Any(x => i < x.Value.Count); i++)
        {
            Console.WriteLine(string.Join(" | ", data.Select(y =>
               ((y.Value.ElementAtOrDefault(i)?.Start)?.ToString() ?? string.Empty).PadLeft(14))));
        }

        long lowestLocation = data["location"].Min(x => x.Start);

        return $"Closest location: {lowestLocation}";
    }

    private static Dictionary<string, List<LongRange>> InitSeedData(string seeds, bool part2)
    {
        Dictionary<string, List<LongRange>> seedData = new()
        {
            { "seed", [] },
            { "soil", [] },
            { "fertilizer", [] },
            { "water", [] },
            { "light", [] },
            { "temperature", [] },
            { "humidity", [] },
            { "location", [] }
        };

        foreach(Match seed in NumberRegex().Matches(seeds))
            seedData["seed"].Add(new LongRange(long.Parse(seed.Value), 0));

        return seedData;
    }

    private sealed class LongRange
    {
        public long Start { get; set; }
        public long End { get; set; }

        public bool Transferred { get; set; }

        public LongRange(long start, long end)
        {
            Start = start;
            End = end;
        }

        public static implicit operator (long start, long end)(LongRange value) => (value.Start, value.End);
        public static implicit operator LongRange((long start, long end) value) => new
            LongRange(value.start, value.end);
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();
}