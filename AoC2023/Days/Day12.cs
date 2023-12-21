using AdventOfCode.Exceptions;
using AdventOfCode.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AdventOfCode.Days;
public class Day12: DayBase
{
    public override string Title => "Hot Springs";


    public override string Solve(string input, bool part2)
    {
        var sum = 0;
        foreach(var line in input.GetLines())
            sum += AnalyzeLine(line, part2);

        return $"There are {sum} valid Configurations";
    }

    private int AnalyzeLine(string line, bool part2)
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


        var validConfigCount = 0;
        ulong defectiveFlags = 1U << machines.Count(x => x == null);


        var validVersions = GetValidVersions(machines, defectives[0]);

        foreach(var groupLength in defectives.Skip(1))
            validVersions = FilterGroup(validVersions, groupLength);

        validVersions = validVersions.Where(x => !x.Exists(y => y == false)).ToList();

        //if(validVersions.SelectMany(x => x).Any(x => x.HasValue))
        //    throw new ResultValidationException("Not all versions completeley resolved:\r\n" + string.Join(Environment.NewLine, validVersions.Select(LineToString)));
        if(TestMode)
            Console.WriteLine(string.Join(' ', [.. documentation, validVersions.Count]));
        return validVersions.Count;
    }

    private string LineToString(List<bool?> src) =>
            string.Concat(src.Select(x => x switch
            {
                true => '.',
                false => '#',
                null => '?'
            }));

    private List<List<bool?>> FilterGroup(List<List<bool?>> validVersions, int groupLength)
    {
        List<List<bool?>> newVersions = new();
        foreach(var version in validVersions)
            newVersions.AddRange(GetValidVersions(version, groupLength));
        return newVersions;
    }

    private List<List<bool?>> GetValidVersions(List<bool?> origin, int defectiveLength)
    {
        List<List<bool?>> validVersions = new();
        int i;
        for(i = 0; i <= origin.Count - defectiveLength; i++)
        {
            //out of range or group is split
            if(origin.Take(i).Any(x => x == false)) break;
            if(origin.Skip(i).Take(defectiveLength).Any(x => x == true)) continue;

            List<bool?> group = origin.Skip(i).ToList();
            for(int j = 0; j < defectiveLength; ++j)
                group[j] = false;
            if(defectiveLength < group.Count && !group[defectiveLength].HasValue)
                group[defectiveLength] = true;
            if(IsConfigurationValid(group, defectiveLength))
                validVersions.Add(group.Skip(defectiveLength).SkipWhile(x => x == true).ToList());
        }

        if(i == 0 && IsConfigurationValid(origin, defectiveLength))
        {
            var remainder = origin.SkipWhile(x => x.HasValue).ToList();
            if(!remainder.Any())
                validVersions.Add(remainder);
        }

        return validVersions;
    }

    private bool IsConfigurationValid(List<bool?> machines, int groupLength)
    {
        var defectiveCount = machines.SkipWhile(x => x == true).TakeWhile(x => x == false).Count();
        return defectiveCount == groupLength;
    }
}
