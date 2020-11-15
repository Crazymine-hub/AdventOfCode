using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Days.Classes.Day18;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Pathfinding;

namespace AdventOfCode.Days
{
    class Day18 : DayBase
    {
        const char wallChar = '#';
        List<BaseNode> nodes;
        List<BaseNodeConnection> connections;
        BaseNode startNode = null;
        List<BaseNode> targets;
        int maxHeight = 0;
        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            GetNodes(input);
            PrintConnections(connections, false);
            
            AStar pathfind = new AStar(connections);
            var target = nodes.Last();
            foreach (Node node in nodes)
                node.UpdateTargetDistance(target);
            var path = pathfind.GetPath(startNode, target, out var traceConnections);
            PrintConnections(traceConnections, true);

            Console.SetCursorPosition(0, maxHeight);
            if (path.LastOrDefault() != target)
                return "Path not found";
            return "";
        }

        private void GetNodes(string maze)
        {
            var mazeLines = maze.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            nodes = new  List<BaseNode>();
            targets = new  List<BaseNode>();
            connections = new List<BaseNodeConnection>();
            Node[] aboveNodes = new Node[mazeLines[0].Length];
            maxHeight = mazeLines.Length + 1;


            for (int y = 0; y < mazeLines.Length; y++)
            {
                var currLine = mazeLines[y];
                Node previousNode = null;
                for (int x = 0; x < currLine.Length; x++)
                {
                    var currPos = currLine[x];
                    if (currPos != wallChar)
                    {//Maze is Surrounded by Walls(#), thus no out of range check required.
                        var above = mazeLines[y - 1][x] != wallChar;
                        var below = mazeLines[y + 1][x] != wallChar;
                        var left = mazeLines[y][x - 1] != wallChar;
                        var right = mazeLines[y][x + 1] != wallChar;

                        //!Av!BvLvR N   AvBv!Lv!R
                        if (((!above || !below || left || right) && (above || below || !left || !right)) || currPos != '.')
                        {
                            var currNode = new Node(x, y, currPos);
                            if (aboveNodes[x] != null) connections.Add(new NodeConnection(currNode, aboveNodes[x]));
                            if (previousNode != null) connections.Add(new NodeConnection(currNode, previousNode));
                            if (currNode.Key != '\0') targets.Add(currNode);
                            nodes.Add(currNode);

                            currNode.PathIndex = TraceChars.GetPathNumber(above, below, left, right, false);

                            aboveNodes[x] = currNode;
                            previousNode = currNode;
                            if (currPos == '@' && startNode == null)
                                startNode = currNode;

                            Console.Write(currPos);
                        }
                        else
                            Console.Write(' ');
                    }
                    else
                    {
                        aboveNodes[x] = null;
                        previousNode = null;
                        Console.Write(currPos);
                    }

                }
                Console.WriteLine();
            }
        }

        private void PrintConnections(List<BaseNodeConnection> connections, bool bold)
        {
            foreach (NodeConnection con in connections)
            {
                Console.SetCursorPosition(con.NodeA.X, con.NodeA.Y);
                Console.Write(TraceChars.paths[con.NodeA.PathIndex | (bold ? 16 : 0)]);
                if (con.IsHorizontal)
                {
                    for (int i = Math.Min(con.NodeA.X, con.NodeB.X) + 1; i < Math.Max(con.NodeA.X, con.NodeB.X); i++)
                    {
                        Console.CursorLeft = i;
                        Console.Write(TraceChars.GetPathChar(false, false, true, true, bold));
                    }
                }
                else
                {
                    for (int i = Math.Min(con.NodeA.Y, con.NodeB.Y) + 1; i < Math.Max(con.NodeA.Y, con.NodeB.Y); i++)
                    {
                        Console.CursorLeft--;
                        Console.CursorTop = i;
                        Console.Write(TraceChars.GetPathChar(true, true, false, false, bold));
                    }
                }
                Console.SetCursorPosition(con.NodeB.X, con.NodeB.Y);
                Console.Write(TraceChars.paths[con.NodeB.PathIndex | (bold ? 16 : 0)]);
            }
        }
    }
}
