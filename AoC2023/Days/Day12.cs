using AdventOfCode.Exceptions;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AdventOfCode.Days;
public class Day12: DayBase
{
    public override string Title => "Hot Springs";


    public override string Solve(string input, bool part2)
    {
        var sum = 0L;
        foreach(var line in input.GetLines())
        {
            sum += AnalyzeLine(line, part2);
        }


        return $"There are {sum} valid Configurations";
    }

    private long AnalyzeLine(string line, bool part2)
    {
        var documentation = line.Split(' ');
        if(part2)
        {
            //until now Brute-Force seemed like a good idea.
            //except that was to be expected
            documentation[0] = string.Join('?', Enumerable.Repeat(documentation[0], 5));
            documentation[1] = string.Join(',', Enumerable.Repeat(documentation[1], 5));
        }
        var machines = documentation[0].Select(x => (bool?)(x switch
        {
            '?' => null,
            '.' => true,
            '#' => false,
            _ => throw new NotSupportedException()
        })).ToList();
        var defectives = documentation[1].Split(',').Select(int.Parse).ToList();


        var validVersions = GetValidVersions(machines, defectives[0], 0)
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.LongCount());

        foreach(var groupLength in defectives.Skip(1))
            validVersions = FilterGroup(machines, validVersions, groupLength);

        var finalVersions = validVersions.Where(x => !machines.Skip(x.Key).Any(y => y == false)).ToList();

        var patternCount = finalVersions.Sum(x => x.Value);
        //if(TestMode)
        Console.WriteLine(string.Join(' ', [.. documentation, patternCount]));
        return patternCount;
    }

    private Dictionary<int, long> FilterGroup(List<bool?> origin, Dictionary<int, long> validIndices, int groupLength)
    {
        Dictionary<int, long> newVersions = new();
        foreach(var index in validIndices)
        {
            var results = GetValidVersions(origin, groupLength, index.Key);
            foreach(var result in results)
            {
                if(!newVersions.ContainsKey(result))
                    newVersions.Add(result, 0);
                newVersions[result] += index.Value;
            }
        }
        return newVersions;
    }

    private List<int> GetValidVersions(List<bool?> machines, int defectiveLength, int startIndex)
    {
        List<int> validIndices = new();
        for(int i = startIndex; i <= machines.Count - defectiveLength; i++)
        {
            if(i > 0 && machines[i - 1] == false) break;
            bool isValid = true;
            bool allSet = true;
            for(int j = 0; j < defectiveLength; ++j)
            {
                if(machines[i + j] == true)
                {
                    isValid = false;
                    allSet = false;
                    break;
                }
                else if(machines[i + j] == null)
                    allSet = false;
            }
            if(isValid && IsConfigurationValid(machines, i, defectiveLength, out bool hitsEnd))
            {
                validIndices.Add(i + defectiveLength + 1);
            }
            if(allSet) break;
        }
        return validIndices;
    }

    private bool IsConfigurationValid(List<bool?> machines, int startIndex, int groupLength, out bool hitsEnd)
    {
        hitsEnd = startIndex + groupLength == machines.Count;
        return hitsEnd || machines[startIndex + groupLength] != false;
    }

    private string LineToString(List<bool?> src) =>
            string.Concat(src.Select(x => x switch
            {
                true => '.',
                false => '#',
                null => '?'
            }));
}
