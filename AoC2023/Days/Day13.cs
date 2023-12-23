using AdventOfCode.Exceptions;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public class Day13: DayBase
{
    public override string Title => "Point of Incidence";

    public override string Solve(string input, bool part2)
    {
        if(part2) return Part2UnavailableMessage;

        int patternSum = 0;
        foreach(var pattern in input.GetGroupedLines())
        {
            patternSum += GetPatternValue(pattern);
            Console.WriteLine();
        }
        return $"The Sum of the mirror coordinates is {patternSum}";
    }

    private int GetPatternValue(string pattern)
    {
        var grid = DynamicGrid.GenerateFromInput(pattern, x => x switch
        {
            '#' => true,
            '.' => false
        });

        Console.WriteLine(pattern);

        //forward column
        int? beforeMirrorIndex = null;
        int mirrorIndex = grid.XDim - 1;
        var mirrorColumn = grid.GetColumn(mirrorIndex).Select(x => x.Value).ToList();
        for(int x = 0; x < grid.XDim - 1; ++x)
        {
            var column = grid.GetColumn(x).Select(x => x.Value).ToList();
            if(column.SequenceEqual(mirrorColumn))
            {
                if(mirrorIndex == x + 1)
                {
                    beforeMirrorIndex = x;
                    break;
                }
                else
                {
                    mirrorIndex--;
                    mirrorColumn = grid.GetColumn(mirrorIndex).Select(x => x.Value).ToList();
                }
            }
        }

        if(beforeMirrorIndex.HasValue)
        {
            Console.WriteLine($"F Mirror has {beforeMirrorIndex.Value + 1} columns before it");
            return beforeMirrorIndex.Value + 1;
        }

        //forward row
        beforeMirrorIndex = null;
        mirrorIndex = grid.YDim - 1;
        mirrorColumn = grid.GetRow(mirrorIndex).Select(x => x.Value).ToList();
        for(int y = 0; y < grid.YDim - 1; ++y)
        {
            var column = grid.GetRow(y).Select(x => x.Value).ToList();
            if(column.SequenceEqual(mirrorColumn))
            {
                if(mirrorIndex == y + 1)
                {
                    beforeMirrorIndex = y;
                    break;
                }
                else
                {
                    mirrorIndex--;
                    mirrorColumn = grid.GetRow(mirrorIndex).Select(x => x.Value).ToList();
                }
            }
        }

        if(beforeMirrorIndex.HasValue)
        {
            Console.WriteLine($"F Mirror has {beforeMirrorIndex.Value + 1} rows before it");
            return (beforeMirrorIndex.Value + 1) * 100;
        }

        //backward column
        beforeMirrorIndex = null;
        mirrorIndex = 0;
        mirrorColumn = grid.GetColumn(mirrorIndex).Select(x => x.Value).ToList();
        for(int x = grid.XDim - 1; x > 0; --x)
        {
            var column = grid.GetColumn(x).Select(x => x.Value).ToList();
            if(column.SequenceEqual(mirrorColumn))
            {
                if(mirrorIndex == x - 1)
                {
                    beforeMirrorIndex = mirrorIndex;
                    break;
                }
                else
                {
                    mirrorIndex++;
                    mirrorColumn = grid.GetColumn(mirrorIndex).Select(x => x.Value).ToList();
                }
            }
        }

        if(beforeMirrorIndex.HasValue)
        {
            Console.WriteLine($"B Mirror has {beforeMirrorIndex.Value + 1} columns before it");
            return beforeMirrorIndex.Value + 1;
        }

        //backward row
        beforeMirrorIndex = null;
        mirrorIndex = 0;
        mirrorColumn = grid.GetRow(mirrorIndex).Select(x => x.Value).ToList();
        for(int y = grid.YDim - 1; y > 0; --y)
        {
            var column = grid.GetRow(y).Select(x => x.Value).ToList();
            if(column.SequenceEqual(mirrorColumn))
            {
                if(mirrorIndex == y - 1)
                {
                    beforeMirrorIndex = mirrorIndex;
                    break;
                }
                else
                {
                    mirrorIndex++;
                    mirrorColumn = grid.GetRow(mirrorIndex).Select(x => x.Value).ToList();
                }
            }
        }

        if(beforeMirrorIndex.HasValue)
        {
            Console.WriteLine($"B Mirror has {beforeMirrorIndex.Value + 1} rows before it");
            return (beforeMirrorIndex.Value + 1) * 100;
        }

        Console.WriteLine("No mirror found");
        return 0;
    }
}
