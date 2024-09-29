using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public class Day14: DayBase
{
    public override string Title => "Parabolic Reflector Dish";

    private enum RockState
    {
        None,
        Solid,
        Rolling
    }

    public override string Solve(string input, bool part2)
    {
        var startPattern = DynamicGrid.GenerateFromInput(input, x => x switch
        {
            '.' => RockState.None,
            '#' => RockState.Solid,
            'O' => RockState.Rolling,
            _ => throw new NotSupportedException($"Invalid char '{x}'")
        });


        Console.WriteLine(GetRockPattern(startPattern));
        Console.WriteLine();

        var cycleCount = part2 ? 1_000_000_000 : 1;
        List<Direction> cycleDirections = [Direction.North, Direction.West, Direction.South, Direction.East];

        List<string> pastLayouts = [];

        int i = 0;
        int ancestorIndex = -1;
        for(i = 0; i < cycleCount; ++i)
        {
            foreach(var direction in cycleDirections)
            {
                var shiftAny = false;
                do
                {
                    shiftAny = ShiftRocks(startPattern, direction);
                    if(!part2)
                    {
                        Console.WriteLine(GetRockPattern(startPattern));
                        Console.WriteLine();
                    }
                }
                while(shiftAny);
                if(!part2) break;
            }
            if(i % 10_000 == 0)
                Console.WriteLine($"{(i) * 100d / cycleCount}%");

            var layout = GetRockPattern(startPattern);
            ancestorIndex = pastLayouts.IndexOf(layout);
            if(ancestorIndex > -1)
                break;
            else
                pastLayouts.Add(layout);
        }

        

        if(part2)
        {
            //stole this math from u/Singing-In-The-Storm
            //https://github.com/JoanaBLate/advent-of-code-js/blob/88c7dba55d5647fb00a0373cf9334f89c80ac995/2023/day14/solve2.js
            var initialSpins = ancestorIndex;
            var spinsPerLoop = pastLayouts.Count - initialSpins;
            var remainingSpins = (cycleCount - initialSpins) % spinsPerLoop;
            var index = initialSpins + remainingSpins - 1;
            startPattern = DynamicGrid.GenerateFromInput(pastLayouts[index], x => x switch
            {
                '.' => RockState.None,
                '#' => RockState.Solid,
                'O' => RockState.Rolling,
                _ => throw new NotSupportedException($"Invalid char '{x}'")
            });
        }

        Console.WriteLine(GetRockPattern(startPattern));
        int weight = GetRockLoad(startPattern);

        return $"The load on the plattform is {weight}";
    }

    private static int GetRockLoad(DynamicGrid<RockState> startPattern)
    {
        var weight = 0;
        foreach(var rock in startPattern.Where(x => x.Value == RockState.Rolling))
            weight += startPattern.YDim - rock.Y;
        return weight;
    }

    private static string GetRockPattern(DynamicGrid<RockState> pattern) =>
        pattern.GetStringRepresentation((RockState state, int x, int y) => state switch
    {
        RockState.None => ".",
        RockState.Solid => "#",
        RockState.Rolling => "O",
        _ => throw new NotSupportedException($"Unsupported State {state}")
    });

    private static bool ShiftRocks(DynamicGrid<RockState> grid, Direction direction)
    {
        bool shiftAny = false;


        var rangeA = Enumerable.Range(0, grid.XDim).ToList();
        var rangeB = Enumerable.Range(0, grid.YDim).ToList();
        if(direction == Direction.South || direction == Direction.East)
        {
            rangeA.Reverse();
            rangeB.Reverse();
        }

        var isRow = direction == Direction.West || direction == Direction.East;
        if(isRow)
            (rangeB, rangeA) = (rangeA, rangeB);

        rangeB.RemoveAt(0);
        foreach(var coordA in rangeA)
        {
            foreach(var coordB in rangeB)
            {
                var x = isRow ? coordB : coordA;
                var y = isRow ? coordA : coordB;
                var currentValue = grid[x, y];
                if(currentValue != RockState.Rolling) continue;
                var targetX = x;
                var targetY = y;
                switch(direction)
                {
                    case Direction.North:
                        targetY -= 1;
                        break;
                    case Direction.South:
                        targetY += 1;
                        break;
                    case Direction.East:
                        targetX += 1;
                        break;
                    case Direction.West:
                        targetX -= 1;
                        break;
                }

                if(grid[targetX, targetY] == RockState.None)
                {
                    grid[targetX, targetY] = currentValue;
                    grid[x, y] = RockState.None;
                    shiftAny = true;
                }
            }
        }
        return shiftAny;
    }
}
