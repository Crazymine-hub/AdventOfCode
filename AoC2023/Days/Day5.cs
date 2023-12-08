using AdventOfCode.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
        var maps = GetGroupedLines(input);
        var data = InitSeedData(maps[0], part2);

        var displayList = Enumerable.Range(0, 100).ToList();
        foreach(var map in maps.Skip(1))
        {
            var ranges = GetLines(map);
            var header = Regex.Match(ranges[0], @"(?<srcCol>\w+)-to-(?<targetCol>\w+) map:");
            var fromCol = header.Groups["srcCol"].Value;
            var toCol = header.Groups["targetCol"].Value;

            var replacements = Enumerable.Repeat(' ', 100).ToList();
            foreach(var range in ranges.Skip(1))
            {
                var rangeDefinimtion = range
                    .Split(' ')
                    .Select(x => long.Parse(x.Trim()))
                    .ToList();
                var start = rangeDefinimtion[1];
                var end = start + rangeDefinimtion[2] - 1;
                if(TestMode)
                {
                    replacements[(int)start] = '\\';
                    replacements[(int)end] = '/';
                    if(start == end)
                        replacements[(int)end] = 'X';
                }
                if(part2)
                    HandlePart2Values(data, fromCol, toCol, rangeDefinimtion[0], start, end);
                else
                    HandlePart1Values(data, fromCol, toCol, rangeDefinimtion[0], start, end);
            }
            data[toCol].AddRange(data[fromCol].Where(x => !x.Transferred).Select(x => new LongRange(x.Start, x.End)));
            if(TestMode)
            {
                Console.WriteLine(map);
                Console.WriteLine(string.Concat(displayList.Select(x => x % 10 == 0 ? '|' : ' ')));
                Console.WriteLine(string.Concat(displayList.Select(x => data[fromCol].Exists(y => y.ContainsValue(x)) ? "#" : "-")));
                Console.WriteLine(string.Concat(replacements));
                Console.WriteLine(string.Concat(displayList.Select(x => data[toCol].Exists(y => y.ContainsValue(x)) ? "#" : "-")));
            }
        }


        Console.WriteLine();
        Console.WriteLine(string.Join(" | ", data.Select(y => string.Empty.PadLeft(14, '='))));
        Console.WriteLine(string.Join(" | ", data.Keys.Select(x => x.PadLeft(14))));
        Console.WriteLine(string.Join(" | ", data.Select(y => string.Empty.PadLeft(14, '='))));
        for(int i = 0; data.Any(x => i < x.Value.Count); i++)
        {
            Console.WriteLine(string.Join(" | ", data.Select(y =>
               ((y.Value.ElementAtOrDefault(i)?.Start)?.ToString() ?? string.Empty).PadLeft(14))));
            if(part2)
            {
                Console.WriteLine(string.Join(" | ", data.Select(y =>
                   ((y.Value.ElementAtOrDefault(i)?.End)?.ToString() ?? string.Empty).PadLeft(14))));
                Console.WriteLine(string.Join(" | ", data.Select(y => string.Empty.PadLeft(14, '-'))));
            }
        }

        long lowestLocation = data["location"].Min(x => x.Start);

        return $"Closest location: {lowestLocation}";
    }


    private static void HandlePart1Values(Dictionary<string, List<LongRange>> data, string fromCol, string toCol, long newOrigin, long start, long end)
    {
        for(int i = 0; i < data[fromCol].Count; ++i)
        {
            var startRange = data[fromCol][i];
            if(startRange.Start < start || startRange.Start > end) continue;
            var newValue = newOrigin + (startRange.Start - start);
            data[toCol].Add(new LongRange(newValue, newValue));
            startRange.Transferred = true;
        }
    }
    private void HandlePart2Values(Dictionary<string, List<LongRange>> data, string fromCol, string toCol, long newOrigin, long start, long end)
    {
        for(int i = 0; i < data[fromCol].Count; ++i)
        {
            var startRange = data[fromCol][i];
            if(startRange.Transferred || end < startRange.Start || start > startRange.End) continue;

            LongRange transfer = new LongRange(start < startRange.Start ? startRange.Start : start,
                end > startRange.End ? startRange.End : end);
            if(transfer.Start != startRange.Start)
                data[fromCol].Add(new LongRange(startRange.Start, transfer.Start - 1));
            if(transfer.End != startRange.End)
                data[fromCol].Add(new LongRange(transfer.End + 1, startRange.End));

            var offset = transfer.Start - start;
            var length = transfer.End - transfer.Start;
            transfer.Start = newOrigin + offset;
            transfer.End = transfer.Start + length;
            startRange.Transferred = true;
            data[toCol].Add(transfer);
        }
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

        var seedMatches = NumberRegex().Matches(seeds).Chunk(part2 ? 2 : 1);
        foreach(Match[] seed in seedMatches)
        {
            var startValue = long.Parse(seed[0].Value);
            var endValue = startValue + (part2 ? long.Parse(seed[1].Value) : 1) - 1;
            seedData["seed"].Add(new LongRange(startValue, endValue));
        }

        return seedData;
    }

    [DebuggerDisplay("{Start}-{End} T:{Transferred}")]
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

        public bool ContainsValue(long value) => value >= Start && value <= End;
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();
}