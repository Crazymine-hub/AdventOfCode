using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public class Day15: DayBase
{
    public override string Title => "Lens Library";

    public override string Solve(string input, bool part2)
    {
        var sections = input.Split(',');
        if(!part2)
            return $"the sum of all hash values is {sections.Select(GetStringHash).Sum()}";

        Dictionary<int, List<string>> boxes = [];

        Dictionary<string, int> lenses = new Dictionary<string, int>();

        foreach(var instruction in sections)
        {
            var segments = instruction.Split('=', '-');
            string label = segments[0];
            var boxNr = GetStringHash(label);
            if(instruction.Contains('-'))
            {
                if(!boxes.ContainsKey(boxNr)) continue;
                boxes[boxNr].Remove(label);
                lenses.Remove(label);
            }
            else
            {
                if(!boxes.ContainsKey(boxNr)) boxes.Add(boxNr, []);
                var focalLength = int.Parse(segments[1]);
                if(boxes[boxNr].Contains(label))
                {
                    lenses[label] = focalLength;
                }
                else
                {
                    boxes[boxNr].Add(label);
                    lenses[label] = focalLength;
                }
            }
        }

        var configPower = 0;
        foreach(var box in boxes)
            for(int i = 0; i < box.Value.Count; ++i)
                configPower += (1 + box.Key) * (i + 1) * lenses[box.Value[i]];

        return $"The configuration Power is {configPower}";
    }

    public int GetStringHash(string input)
    {
        int hash = 0;
        foreach(var character in Encoding.ASCII.GetBytes(input))
        {
            hash += character;
            hash *= 17;
            hash %= 256;
        }

        return hash;
    }
}
