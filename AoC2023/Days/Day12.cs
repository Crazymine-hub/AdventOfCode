using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
        var tasks = new List<Task<int>>();
        foreach(var line in GetLines(input))
        {
            var task = Task<int>.Run(() => AnalyzeLine(line, part2));
            tasks.Add(task);
            if(TestMode)
                task.Wait();
        }
        Task.WaitAll(tasks.ToArray(), CancellationToken);

        return $"There are {tasks.Sum(x => x.Result)} valid Configurations";
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

        if(TestMode)
        {
            Console.Write(documentation[0]);
            Console.Write(" ");
            Console.WriteLine(documentation[1]);
        }

        for(ulong i = 0; i < defectiveFlags; ++i)
            if(IsConfigurationValid(machines, defectives, i, TestMode && !part2))
                validConfigCount++;
        if(TestMode)
        {
            Console.WriteLine(validConfigCount);
            Console.WriteLine();
        }
        return validConfigCount;
    }

    private bool IsConfigurationValid(List<bool?> machines, List<int> defectives, ulong states, bool printDebug)
    {
        List<bool?> fullMachines = [.. machines];
        int byteIndex = 0;
        var defectiveCount = 0;
        var groupIndex = 0;

        for(int i = 0; i < fullMachines.Count; ++i)
        {
            if(!fullMachines[i].HasValue)
            {
                fullMachines[i] = (states & (1U << byteIndex)) != 0;
                byteIndex++;
            }
        }

        if(printDebug)
            Console.Write(string.Concat(fullMachines.Select(x => x.Value ? '.' : '#')));

        //Seperate loop for beauty
        for(int i = 0; i < fullMachines.Count; ++i)
        {
            if(fullMachines[i] == true)
            {
                if(defectiveCount == 0) continue;
                if(groupIndex >= defectives.Count)
                {
                    if(printDebug)
                        Console.WriteLine($" C More defective groups than given");
                    return false;
                }
                if(defectiveCount != defectives[groupIndex])
                {
                    if(printDebug)
                        Console.WriteLine($" C Group [{groupIndex}]: {defectiveCount,4} Expected: {defectives[groupIndex]}");
                    return false;
                }

                defectiveCount = 0;
                groupIndex++;
            }
            else
                defectiveCount++;
        }

        if(defectiveCount > 0)
        {
            if(groupIndex >= defectives.Count)
            {
                if(printDebug)
                    Console.WriteLine($" E More defective groups than expected");
                return false;
            }
            if(defectiveCount != defectives[groupIndex])
            {
                if(printDebug)
                    Console.WriteLine($" E Group [{groupIndex}]: {defectiveCount,4} Expected: {defectives[groupIndex]}");
                return false;
            }
            groupIndex++;
        }

        if(groupIndex != defectives.Count)
        {
            if(printDebug)
                Console.WriteLine($" E Found {groupIndex,4} Groups. Expected: {defectives.Count}");
            return false;
        }

        if(TestMode)
        {
            if(!printDebug)
                Console.Write(string.Concat(fullMachines.Select(x => x.Value ? '.' : '#')));
            Console.WriteLine("   OK");
        }
        return true;
    }
}
