using AdventOfCode.Days.Tools.Day10;
using AdventOfCode.Exceptions;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public class Day10: DayBase
{
    public override string Title => "Pipe Maze";

    public override string Solve(string input, bool part2)
    {
        if(part2) return Part2UnavailableMessage;

        //Mapping TraceChars via constants
        DynamicGrid<DirectionalNode> grid = DynamicGrid.GenerateFromInput(input, (value, x, y) =>
        new DirectionalNode(x, y,
        value switch
        {
            '|' => TraceChars.Direction.UpDown,
            '-' => TraceChars.Direction.LeftRight,
            'L' => TraceChars.Direction.UpRight,
            'J' => TraceChars.Direction.UpLeft,
            '7' => TraceChars.Direction.DownLeft,
            'F' => TraceChars.Direction.DownRight,
            '.' => TraceChars.Direction.None,
            'S' => TraceChars.Direction.AllDriections | TraceChars.Direction.Bold,
            _ => throw new ArgumentException($"The Char {value} could not be mapped to the grid ({x + 1} , {y + 1})"),
        }));

        var startNode = grid.Single(x => x.Value.Direction == (TraceChars.Direction.AllDriections | TraceChars.Direction.Bold)).Value;

        var connections = new List<BaseNodeConnection>();

        Console.WriteLine("Loading Grid....");
        foreach(var node in grid.Select(x => x.Value))
        {
            foreach(var neighbour in grid.GetNeighbours(node.X, node.Y, false).Select(x => x.Value).Where(x => x is not null))
            {
                if(connections.Exists(x => x.HasConnectionTo(node) && x.HasConnectionTo(neighbour))) continue;

                var xDir = neighbour.X - node.X;
                var yDir = neighbour.Y - node.Y;

                if((xDir == 0 && yDir == -1 && node.Direction.HasFlag(TraceChars.Direction.Up) && neighbour.Direction.HasFlag(TraceChars.Direction.Down)) ||
                    (xDir == 0 && yDir == 1 && node.Direction.HasFlag(TraceChars.Direction.Down) && neighbour.Direction.HasFlag(TraceChars.Direction.Up)) ||
                    (xDir == 1 && yDir == 0 && node.Direction.HasFlag(TraceChars.Direction.Right) && neighbour.Direction.HasFlag(TraceChars.Direction.Left)) ||
                   (xDir == -1 && yDir == 0 && node.Direction.HasFlag(TraceChars.Direction.Left) && neighbour.Direction.HasFlag(TraceChars.Direction.Right)))
                    connections.Add(new BaseNodeConnection(node, neighbour));
            }
        }

        var startNeighbours = connections.Where(x => x.HasConnectionTo(startNode)).Select(x => (DirectionalNode)x.GetOtherNode(startNode)).ToList();

        Console.WriteLine("Searching Path...");
        var lastNode = startNeighbours[0];
        var endNode = startNeighbours[1];
        List<DirectionalNode> path = [startNode, lastNode];
        while(lastNode != endNode && lastNode is not null)
        {
            var nextNode = (DirectionalNode)connections.Where(x => x.HasConnectionTo(lastNode))
                                        .Select(x => x.GetOtherNode(lastNode))
                                        .Except(path)
                                        .SingleOrDefault();
            path.Add(nextNode);
            lastNode = nextNode;
        }

        path.Add(startNode);

        TraversePath(path);
        TraversePath(path.Reverse<DirectionalNode>());

        var furthestNode = grid.MaxBy(x => x.Value.StepsFromStart).Value;

        Console.WriteLine(grid.GetStringRepresentation((x, _, _) => x == furthestNode ? "X" : TraceChars.paths[(int)x.Direction].ToString()));
        return $"The furthest node is {furthestNode.StepsFromStart} steps away from start";
    }

    private static void TraversePath(IEnumerable<DirectionalNode> path)
    {
        int steps = 0;
        foreach(var node in path)
        {
            node.Direction |= TraceChars.Direction.Bold;
            if(node.StepsFromStart == -1 || node.StepsFromStart > steps)
                node.StepsFromStart = steps;
            steps++;
        }
    }
}
