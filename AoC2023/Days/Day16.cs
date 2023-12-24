using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Days.Tools.Day16;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace AdventOfCode.Days;
public partial class Day16: DayBase
{
    public override string Title => "The Floor Will Be Lava";

    public override string Solve(string input, bool part2)
    {
        var contraption = DynamicGrid.GenerateFromInput(input, x => new ContraptionItem(x, energized: false));
        Console.WriteLine(contraption.GetStringRepresentation((v, x, y) => v.Config.ToString()));

        if(!part2)
        {
            LightBeam startBeam = (-1, 0, TraceChars.Direction.Right);
            return $"There are {SimulateRay(contraption, startBeam)} energized fields";
        }

        var mostTiles = 0;
        for(int x= 0; x < contraption.XDim; ++x)
        {
            var energized = SimulateRay(contraption, new LightBeam(x, -1, TraceChars.Direction.Down));
            if(energized > mostTiles)
                mostTiles = energized;
            energized = SimulateRay(contraption, new LightBeam(x, contraption.YDim, TraceChars.Direction.Up));
            if(energized > mostTiles)
                mostTiles = energized;
        }
        for(int y = 0; y < contraption.XDim; ++y)
        {
            var energized = SimulateRay(contraption, new LightBeam(-1, y, TraceChars.Direction.Right));
            if(energized > mostTiles)
                mostTiles = energized;
            energized = SimulateRay(contraption, new LightBeam(contraption.XDim, y, TraceChars.Direction.Left));
            if(energized > mostTiles)
                mostTiles = energized;
        }

        return $"The best configuration has {mostTiles} energized fields.";
    }

    private static int SimulateRay(DynamicGrid<ContraptionItem> contraption, LightBeam startBeam)
    {
        foreach(var item in contraption)
            item.Value.Energized = false;
        int unexploredIndex = 0;
        List<LightBeam> unexplored = [startBeam];

        LightBeam currentBeam = unexplored[0];
        while(true)
        {
            switch(currentBeam.Direction)
            {
                case TraceChars.Direction.Left:
                    currentBeam.X--;
                    break;
                case TraceChars.Direction.Right:
                    currentBeam.X++;
                    break;
                case TraceChars.Direction.Up:
                    currentBeam.Y--;
                    break;
                case TraceChars.Direction.Down:
                    currentBeam.Y++;
                    break;
            }

            if(!contraption.InRange(currentBeam.X, currentBeam.Y))
            {
                ++unexploredIndex;
                if(unexploredIndex == unexplored.Count) break;
                currentBeam = unexplored[unexploredIndex];
                continue;
            }
            var currentItem = contraption[currentBeam.X, currentBeam.Y];
            currentItem.Energized = true;

            switch(currentItem.Config)
            {
                case '.': continue;
                case '/':
                    currentBeam.Direction = currentBeam.Direction switch
                    {
                        TraceChars.Direction.Up => TraceChars.Direction.Right,
                        TraceChars.Direction.Right => TraceChars.Direction.Up,
                        TraceChars.Direction.Down => TraceChars.Direction.Left,
                        TraceChars.Direction.Left => TraceChars.Direction.Down,
                        _ => throw new NotSupportedException($"Invalid Direction to mirror / {currentBeam.Direction}")
                    };
                    break;
                case '\\':
                    currentBeam.Direction = currentBeam.Direction switch
                    {
                        TraceChars.Direction.Up => TraceChars.Direction.Left,
                        TraceChars.Direction.Left => TraceChars.Direction.Up,
                        TraceChars.Direction.Down => TraceChars.Direction.Right,
                        TraceChars.Direction.Right => TraceChars.Direction.Down,
                        _ => throw new NotSupportedException($"Invalid Direction to mirror \\ {currentBeam.Direction}")
                    };
                    break;
                case '-':
                    if((currentBeam.Direction & (TraceChars.Direction.UpDown)) != 0)
                    {
                        currentBeam.Direction = TraceChars.Direction.Left;
                        var otherDirection = new LightBeam(currentBeam.X, currentBeam.Y, TraceChars.Direction.Right);
                        if(!unexplored.Contains(otherDirection))
                            unexplored.Add(otherDirection);
                    }
                    break;
                case '|':
                    if((currentBeam.Direction & (TraceChars.Direction.LeftRight)) != 0)
                    {
                        currentBeam.Direction = TraceChars.Direction.Up;
                        var otherDirection = new LightBeam(currentBeam.X, currentBeam.Y, TraceChars.Direction.Down);
                        if(!unexplored.Contains(otherDirection))
                            unexplored.Add(otherDirection);
                    }
                    break;
                default:
                    throw new NotSupportedException($"Invalid Configuration {currentItem.Config}");

            }
        }

        var energized = contraption.Count(x => x.Value.Energized);
        Console.WriteLine(contraption.GetStringRepresentation((v, x, y) => v.Energized ? "E" : v.Config.ToString()));
        Console.WriteLine(energized);
        Console.WriteLine();
        Console.WriteLine();

        return energized;
    }
}
