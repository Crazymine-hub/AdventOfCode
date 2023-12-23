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
        int patternSum = 0;
        var difference = part2 ? 1 : 0;
        foreach(var pattern in input.GetGroupedLines())
        {
            patternSum += GetPatternValue(pattern, difference);
            Console.WriteLine();
        }
        return $"The Sum of the mirror coordinates is {patternSum}";
    }

    private int GetPatternValue(string pattern, int comparisonDifference)
    {
        var grid = DynamicGrid.GenerateFromInput(pattern, x => x switch
        {
            '#' => true,
            '.' => false,
            _ => throw new NotSupportedException($"Char '{x}' not supported")
        }) ;

        Console.WriteLine(pattern);

        var beforeMirror = EvaluateGridIndex(grid, false, comparisonDifference);
        if(beforeMirror.HasValue)
        {
            Console.WriteLine($"There are {beforeMirror} columns in front of the mirror");
            return beforeMirror.Value;
        }
        beforeMirror = EvaluateGridIndex(grid, true, comparisonDifference);
        if(beforeMirror.HasValue)
        {
            Console.WriteLine($"There are {beforeMirror} rows in front of the mirror");
            return beforeMirror.Value * 100;
        }

        throw new ResultValidationException("No mirror found");
    }


    private int? EvaluateGridIndex(DynamicGrid<bool> grid, bool isRow, int expectedDifference)
    {
        List<bool> GetGridValues(int index)
        {
            var values = isRow ? grid.GetRow(index) : grid.GetColumn(index);
            return values.Select(x => x.Value).ToList();
        }

        var gridDimension = isRow ? grid.YDim : grid.XDim;
        List<int> similar = new List<int>();
        var previousColumn = GetGridValues(0);
        for(int x = 1; x < gridDimension; ++x)
        {
            var column = GetGridValues(x);
            if(ListEqual(column, previousColumn, expectedDifference, out int offest))
                similar.Add(x - 1);
            previousColumn = column;
        }

        foreach(var column in similar)
        {
            var offset = 0;
            var differences = 0;
            while(true)
            {
                if(column - offset < 0 || column + 1 + offset >= gridDimension)
                    break;
                var columnA = GetGridValues(column - offset);
                var columnB = GetGridValues(column + 1 + offset);

                if(!ListEqual(columnA, columnB, expectedDifference, out int comparedDifferences))
                    break;

                differences += comparedDifferences;

                if(differences > expectedDifference)
                    break;

                if((column - offset == 0 || column + 1 + offset == gridDimension - 1) && differences == expectedDifference)
                    return column + 1;
                offset++;
            }
        }

        return null;
    }

    private bool ListEqual(List<bool> columnA, List<bool> columnB, int expectedDifference, out int difference)
    {
        difference = 0;
        if(columnA.Count != columnB.Count) return false;

        for(int i = 0; i < columnA.Count; ++i) {
            if(columnA[i] != columnB[i])
            {
                difference++;
                if(difference > expectedDifference)
                    return false;
            }
        }

        return true;
    }
}
