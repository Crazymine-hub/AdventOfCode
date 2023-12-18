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
        if(part2) return Part2UnavailableMessage;

        int index = 1;
        var universe = DynamicGrid.GenerateFromInput(input, x => x == '#');



        RenderUniverse(universe);
        Expand(universe);
        Console.WriteLine();
        Console.WriteLine("EXPANSION");
        Console.WriteLine();
        RenderUniverse(universe);
        
        
        var galaxies = universe.Where(x => x.Value).ToList();
        var distanceSum = 0;

        for(int i = 0; i < galaxies.Count; i++)
            for(int j = i; j < galaxies.Count; j++)
            {
                var a = galaxies[i];
                var b = galaxies[j];

                var distance = VectorAssist.ManhattanDistance(a.AsPoint(), b.AsPoint());
                distanceSum += distance;
            }

        return $"The sum of all distances between the Galaxies is {distanceSum}";
    }


    private void Expand(DynamicGrid<bool> universe)
    {
        List<bool> columnHasGalaxy = [.. Enumerable.Repeat(false, universe.XDim)];
        for(int y = 0; y < universe.YDim; ++y)
        {
            bool rowHasGalaxy = false;
            for(int x = 0; x < universe.XDim; ++x)
            {
                rowHasGalaxy |= universe[x, y];
                columnHasGalaxy[x] |= universe[x, y];
            }

            if(!rowHasGalaxy)
            {
                universe.InsertRow(y);
                y++;
            }
        }

        for(int i = 0; i < columnHasGalaxy.Count; ++i)
            if(!columnHasGalaxy[i])
            {
                universe.InsertColumn(i);
                columnHasGalaxy.Insert(i, false);
                ++i;
            }

    }
    private void RenderUniverse(DynamicGrid<bool> universe) =>
        Console.WriteLine(universe.GetStringRepresentation((val, _, _) => (val ? '#' : '.').ToString()));
}
