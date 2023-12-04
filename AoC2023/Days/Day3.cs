using AdventOfCode.Tools.DynamicGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode.Days;
public sealed class Day3: DayBase
{
    public override string Title => "";

    public override string Solve(string input, bool part2)
    {
        DynamicGrid<char?> schematic = LoadSchematic(input);
        var symbols = schematic.Where(x => x.Value.HasValue && !char.IsDigit(x.Value.Value));

        if(part2)
            symbols = symbols.Where(x => x.Value.Value == '*');

        List<(int X, int Y)> neighbourNumbers = new();
        foreach(var symbol in symbols)
        {
            var neighbours = schematic
                .GetNeighbours(symbol.X, symbol.Y)
                .Where(x => x.Value.HasValue && char.IsDigit(x.Value.Value))
                .Select(x => (x.X, x.Y))
                .ToList();
            if(part2)
            {
                neighbours = neighbours.Select(x => GetStartPosition(schematic, x.X, x.Y)).Distinct().ToList();
                if(neighbours.Count == 2)
                    neighbourNumbers.AddRange(neighbours);
            }
            else
                neighbourNumbers.AddRange(neighbours);
        }


        if(part2)
        {
            long totalRatio = neighbourNumbers
                .Select(x => GetCompletePartNumber(schematic, x.X, x.Y, out _))
                .Chunk(2)
                .Select(x => x[0] * x[1])
                .Sum();
            return $"Sum of all gear ratios: {totalRatio}";
        }

        long partSum = neighbourNumbers
            .Select(x => (partNumber: GetCompletePartNumber(schematic, x.X, x.Y, out int startX), startX, x.Y))
            .GroupBy(x => (x.startX, x.Y))
            .Sum(x => x.First().partNumber);
        return $"Sum of all partNumbers: {partSum}";
    }

    private long GetCompletePartNumber(DynamicGrid<char?> schematic, int digitX, int digitY, out int startX)
    {
        (startX, _) = GetStartPosition(schematic, digitX, digitY);

        var currX = startX;
        long partNumber = 0;
        do
        {
            partNumber *= 10;
            partNumber += long.Parse(schematic[currX, digitY].Value.ToString());
            currX++;
        } while(currX < schematic.XDim && schematic[currX, digitY].HasValue && char.IsDigit(schematic[currX, digitY].Value));
        Console.WriteLine($"Partnumber {partNumber} @{startX},{digitY}");
        return partNumber;
    }

    private static (int X, int Y) GetStartPosition(DynamicGrid<char?> schematic, int digitX, int digitY)
    {
        int startX = digitX;
        while(schematic[startX, digitY].HasValue && char.IsDigit(schematic[startX, digitY].Value))
        {
            if(--startX < 0)
                break;
        }

        return (startX + 1, digitY);
    }

    private DynamicGrid<char?> LoadSchematic(string input)
    {
        DynamicGrid<char?> schematic = new();
        var lines = GetLines(input);
        for(int y = 0; y < lines.Count; y++)
        {
            var line = lines[y];
            for(int x = 0; x < line.Length; x++)
                schematic.SetRelative(x, y, line[x] == '.' ? null : line[x]);
        }
        return schematic;
    }
}