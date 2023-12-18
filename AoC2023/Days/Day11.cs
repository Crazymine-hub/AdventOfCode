using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public class Day11: DayBase
{
    public override string Title => "Cosmic Expansion";

    public override string Solve(string input, bool part2)
    {
        var universe = DynamicGrid.GenerateFromInput(input, x => x == '#');
        var (rowHeights, columnWidths) = Expand(universe, part2);
        RenderUniverse(universe);
        
        
        var galaxies = universe.Where(x => x.Value).ToList();
        long distanceSum = 0;

        for(int i = 0; i < galaxies.Count; i++)
            for(int j = i + 1; j < galaxies.Count; j++)
            {
                var a = galaxies[i].AsPoint();
                var b = galaxies[j].AsPoint();

                var min = Math.Min(a.X, b.X);
                var max = Math.Max(a.X, b.X);
                var width = columnWidths.Where((x, i) => i >= min && i < max).Sum();
                min = Math.Min(a.Y, b.Y);
                max = Math.Max(a.Y, b.Y);
                var height = rowHeights.Where((x, i) => i >= min && i < max).Sum();


                var distance = width + height;
                distanceSum += distance;
            }

        return $"The sum of all distances between the Galaxies is {distanceSum}";
    }


    private (List<long> rowHeights, List<long> columnWidths) Expand(DynamicGrid<bool> universe, bool part2)
    {
        List<bool> columnHasGalaxy = [.. Enumerable.Repeat(false, universe.XDim)];
        List<long> columnWidth = [.. Enumerable.Repeat(1L, universe.XDim)];
        List<long> rowHeight = [.. Enumerable.Repeat(1L, universe.XDim)];

        var part2Expansion = TestMode ? 10 : 1_000_000;
        const int part1Expansion = 2;


        for(int y = 0; y < universe.YDim; ++y)
        {
            bool rowHasGalaxy = false;
            for(int x = 0; x < universe.XDim; ++x)
            {
                rowHasGalaxy |= universe[x, y];
                columnHasGalaxy[x] |= universe[x, y];
            }

            if(!rowHasGalaxy)
                rowHeight[y] = part2 ? part2Expansion : part1Expansion;
        }

        for(int i = 0; i < columnHasGalaxy.Count; ++i)
            if(!columnHasGalaxy[i])
                columnWidth[i] = part2 ? part2Expansion : part1Expansion;
        return (rowHeight, columnWidth);
    }
    private void RenderUniverse(DynamicGrid<bool> universe) =>
        Console.WriteLine(universe.GetStringRepresentation((val, _, _) => (val ? '#' : '.').ToString()));
}
