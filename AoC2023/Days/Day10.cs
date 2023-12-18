using AdventOfCode.Days.Tools.Day10;
using AdventOfCode.Exceptions;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Tracing;
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

        TraversePath(path, true);
        TraversePath(path.Reverse<DirectionalNode>().ToList(), false);

        var furthestNode = grid.MaxBy(x => x.Value.StepsFromStart).Value;

        Console.WriteLine(RenderGrid(grid, startNode, furthestNode));
        if(part2)
        {
            Console.WriteLine("  X|Y   Dir.      A In | B Out ");
            Console.WriteLine(string.Join(Environment.NewLine, path.Select(x => $"{x.X,3}|{x.Y,-3} ({TraceChars.paths[(int)x.Direction]}) {x.SideA,10} | {x.SideB}")));

            var nonLoop = grid.Select(x => x.Value).Where(x => !x.Direction.HasFlag(TraceChars.Direction.Bold)).ToList();

            var topNode = path.MinBy(x => x.Y);
            bool aIsInside = topNode.SideB.HasFlag(TraceChars.Direction.Up);
            if(!aIsInside && !topNode.SideA.HasFlag(TraceChars.Direction.Up))
                throw new InvalidOperationException($"The chosen Node {topNode} should have a UP Side. but somehow does not");

            Console.WriteLine("Determining inside Points....");
            var taskList = new List<Task>();
            foreach(var node in nonLoop)
            {
                var task = new Task(() => DetermineNode(node, path, aIsInside), CancellationToken);
                taskList.Add(task);
                task.Start();
            }

            Task.WaitAll(taskList.ToArray());
            Console.WriteLine(RenderGrid(grid, startNode, furthestNode));
            return $"The grid encloses {grid.Count(x => x.Value.IsInside)} Nodes.";
        }
        return $"The furthest node is {furthestNode.StepsFromStart} steps away from start";
    }

    private static string RenderGrid(DynamicGrid<DirectionalNode> grid, DirectionalNode startNode, DirectionalNode furthestNode)
    {
        return grid.GetStringRepresentation((x, _, _) =>
        {
            if(x == furthestNode)
                return "X";
            else if(x == startNode)
                return "S";
            else if(!x.Direction.HasFlag(TraceChars.Direction.Bold) && x.IsInside)
                return "I";
            else
                return TraceChars.paths[(int)x.Direction].ToString();
        });
    }

    private void DetermineNode(DirectionalNode node, List<DirectionalNode> wall, bool aIsInside)
    {
        var closestNode = wall.Where(x => x.X == node.X || x.Y == node.Y).MinBy(x => x.GetDistanceTo(node));
        var direction = Rotate(GetDirection(node, closestNode), 2);

        if(aIsInside)
            node.IsInside = closestNode.SideA.HasFlag(direction);
        else
            node.IsInside = closestNode.SideB.HasFlag(direction);
    }

    private static void TraversePath(List<DirectionalNode> path, bool updateSides)
    {

        TraceChars.Direction fromDirection = GetDirection(path[0], path[1]);
        path[0].Direction = fromDirection;
        fromDirection = GetDirection(path[^1], path[^2]);
        path[0].Direction |= fromDirection;



        int steps = 0;
        foreach(var node in path)
        {
            node.Direction |= TraceChars.Direction.Bold;
            if(node.StepsFromStart == -1 || node.StepsFromStart > steps)
                node.StepsFromStart = steps;
            steps++;

            if(updateSides)
            {
                var toDirection = node.Direction & ~(fromDirection | TraceChars.Direction.Bold);
                var rotateCount = 0;
                var upBit = TraceChars.Direction.Up;
                int rotationCount = 0;
                while(upBit != fromDirection)
                {
                    upBit = Rotate(upBit, 1);
                    rotationCount++;
                    if(rotationCount > 3)
                        throw new InvalidOperationException("Unable to find shift amount for Up");
                }

                var leftSide = Rotate(TraceChars.Direction.Left, rotationCount);
                var rightSide = Rotate(TraceChars.Direction.Right, rotationCount);
                var openSides = TraceChars.Direction.AllDriections & ~(fromDirection | toDirection);

                node.SideA = leftSide & openSides;
                node.SideB = rightSide & openSides;
                if((node.SideA | node.SideB) != openSides)
                {
                    if(node.SideA == TraceChars.Direction.None)
                        node.SideB |= openSides;
                    else
                        node.SideA |= openSides;
                }

                fromDirection = Rotate(toDirection, 2);
            }
        }
    }

    private static TraceChars.Direction GetDirection(DirectionalNode from, DirectionalNode to)
    {
        TraceChars.Direction fromDirection;
        var xStart = to.X - from.X;
        var yStart = to.Y - from.Y;

        if(xStart == 0)
        {
            if(yStart < 0)
                fromDirection = TraceChars.Direction.Up;
            else if(yStart > 0)
                fromDirection = TraceChars.Direction.Down;
            else throw new InvalidOperationException("Both nodes seem to be on top of each other");
        }
        else if(xStart < 0)
            fromDirection = TraceChars.Direction.Left;
        else
            fromDirection = TraceChars.Direction.Right;
        return fromDirection;
    }

    private static TraceChars.Direction Rotate(TraceChars.Direction direction, int amount)
    {
        for(int i = 0; i < (amount % 4); ++i)
        {
            direction = direction switch
            {
                TraceChars.Direction.Up => TraceChars.Direction.Right,
                TraceChars.Direction.Right => TraceChars.Direction.Down,
                TraceChars.Direction.Down => TraceChars.Direction.Left,
                TraceChars.Direction.Left => TraceChars.Direction.Up,
                _ => throw new NotSupportedException($"The Direction {direction} is not supported for Rotation")
            };
        }

        return direction;
    }
}
