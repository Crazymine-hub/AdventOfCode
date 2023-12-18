using AdventOfCode.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.DynamicGrid;
public static class DynamicGrid
{
    public delegate TResult ValueParseDelegate<out TResult>(char currentChar, int x, int y);
    public delegate TResult SimpleValueParseDelegate<out TResult>(char currentChar);
    public delegate TResult ValueParseDelegate3D<out TResult>(char currentChar, int x, int y, int z);

    public static DynamicGrid<T> GenerateFromInput<T>(string input, SimpleValueParseDelegate<T> valueParseDelegate)
        => GenerateFromInput(input, (v, _, _) => valueParseDelegate(v));

    public static DynamicGrid<T> GenerateFromInput<T>(string input, ValueParseDelegate<T> valueParseDelegate)
    {
        var lines = input.GetLines();
        var grid = new DynamicGrid<T>();
        for(int y = 0; y < lines.Count; ++y)
        {
            var line = lines[y];
            for(int x = 0; x < line.Length; ++x)
                grid[x, y] = valueParseDelegate.Invoke(line[x], x, y);
        }

        return grid;
    }

    public static DynamicGrid<char> GenerateCharGrid(string input)
        => GenerateFromInput(input, x => x);

    public static DynamicGrid<int> GenerateIntGrid(string input)
        => GenerateFromInput(input, x => int.Parse(x.ToString()));
}
