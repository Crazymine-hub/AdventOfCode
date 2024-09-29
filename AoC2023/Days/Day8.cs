using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public partial class Day8: DayBase
{
    public override string Title => "Haunted Wasteland";

    public override string Solve(string input, bool part2)
    {

        var inputSections = GetGroupedLines(input);
        var instructions = inputSections[0];

        Dictionary<string, List<string>> nodes = GetLines(inputSections[1]).Select(x => nodeRegEx().Match(x))
            .ToDictionary(x => x.Groups["NodeName"].Value, x => x.Groups["connections"].Captures.Select(x => x.Value).ToList());

        List<Node> currentNode = [("AAA", 0)];
        if(part2)
        {
            currentNode.Clear();
            currentNode.AddRange(nodes.Keys.Where(x => x[^1] == 'A').Select(x => new Node(x, 0)));
            Console.WriteLine(string.Concat(currentNode.Select(x => x.Name.PadLeft(4))));
        }
        var stepPosition = 0;
        long requiredSteps = 0;

        if(part2)
        {
            while(!currentNode.TrueForAll(x => x.Steps > 0))
            {
                DoNodeStep(instructions, nodes, currentNode, ref stepPosition, ref requiredSteps);
                foreach(var node in currentNode)
                {
                    if(node.Steps != 0) continue;
                    if(node.Name[^1] == 'Z')
                        node.Steps = requiredSteps;
                }
            }

            requiredSteps = MathHelper.LeastCommonMultiple(currentNode.Select(x => x.Steps));
        }
        else
            while(currentNode[0].Name != "ZZZ")
                DoNodeStep(instructions, nodes, currentNode, ref stepPosition, ref requiredSteps);

        return $"The path took {requiredSteps} Steps";
    }

    private static void DoNodeStep(string instructions, Dictionary<string, List<string>> nodes, List<Node> currentNode, ref int stepPosition, ref long requiredSteps)
    {
        var step = instructions[stepPosition++];
        if(stepPosition >= instructions.Length)
            stepPosition = 0;
        for(int i = 0; i < currentNode.Count; ++i)
        {
            currentNode[i].Name = nodes[currentNode[i].Name][step == 'L' ? 0 : 1];
            Console.Write(currentNode[i].Name.PadLeft(4));
        }
        requiredSteps++;
        Console.WriteLine();
    }

    [GeneratedRegex(@"^(?<NodeName>\w+) = \((?:(?<connections>\w+)(?:, )?)+\)$")]
    private static partial Regex nodeRegEx();
}

internal class Node(string node, long steps)
{
    public string Name { get; set; } = node;
    public long Steps { get; set; } = steps;

    public static implicit operator Node((string node, long steps) value) => new(value.node, value.steps);
}