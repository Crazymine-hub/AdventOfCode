using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days;
public class Day17: DayBase
{
    private AStarNode startNode;

    public override string Title => "Clumsy Crucible";

    public override string Solve(string input, bool part2)
    {
        if(part2) return Part2UnavailableMessage;

        var nodes = DynamicGrid.GenerateFromInput(input, (v, x, y) => new AStarNode(x, y, int.Parse(v.ToString())));
        List<AStarNodeConnection> connections = [];
        foreach(var node in nodes)
        {
            if(node.X > 0)
                connections.Add(new AStarNodeConnection(nodes[node.X - 1, node.Y], node.Value));
            if(node.Y > 0)
                connections.Add(new AStarNodeConnection(nodes[node.X, node.Y - 1], node.Value));
        }

        Console.WriteLine(nodes.GetStringRepresentation((v, _, _) => v.NodeHeuristic.ToString()));
        startNode = nodes[0, 0];
        var endNode = nodes[nodes.XDim - 1, nodes.YDim - 1];

        var pathFind = new AStarPathfinder(connections);
        pathFind.CanUsePathCallback += IsPathValid;

        var path = pathFind.GetPath(startNode, endNode);
        var heatLoss = path.Skip(1).Sum(x => x.NodeHeuristic);

        Console.WriteLine(nodes.GetStringRepresentation((v, _, _) => path.Contains(v) ? "#" : v.NodeHeuristic.ToString()));

        return $"The heat loss on the best path is {heatLoss}.";
    }

    private bool IsPathValid(AStarNodeConnection nodeConnection, AStarNode currentNode)
    {
        var awayNode = currentNode.PreviousNode?.PreviousNode;
        if(awayNode is null || awayNode == startNode) return true;
        var nextNode = nodeConnection.GetOtherNode(currentNode);
        var difference = VectorAssist.ManhattanDistance(new Point(nextNode.X, nextNode.Y), new Point(awayNode.X, awayNode.Y));
        return difference < 3;
    }
}
