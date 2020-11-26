using AdventOfCode.Days.Classes.Day20;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day20 : DayBase
    {
        private List<Node> nodes;
        private List<Node> portals;
        private List<BaseNodeConnection> connections;
        private List<NodeConnection> shortcuts;
        private int maxHeight = 0;
        private const char wallChar = '#';

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            GetNodes(input);
            BaseNodeConnection.PrintConnections(connections, 0);
            Console.SetCursorPosition(0, maxHeight);
            shortcuts = new List<NodeConnection>();
            List<Node> unused = new List<Node>();
            foreach (NodeConnection con in connections.Where(x => x.NodeA.CharRepresentation != '\0' && x.NodeB.CharRepresentation != '\0'))
            {
                Console.Write("" + con.NodeA.CharRepresentation + con.NodeB.CharRepresentation + " -> ");
                if (connections.Where(x => x.HasConnectionTo(con.NodeA)).Count() == 1)
                {
                    unused.Add(con.NodeA);
                    if (con.NodeA.X < con.NodeB.X || con.NodeA.Y < con.NodeB.Y)
                    {
                        con.NodeB.PortalCode = con.NodeA.PortalCode + con.NodeB.PortalCode;
                        if (con.NodeA.X < con.NodeB.X)
                            con.NodeB.X++;
                        else
                            con.NodeB.Y++;

                    }
                    else
                    {
                        con.NodeB.PortalCode += con.NodeA.PortalCode;
                        if (con.NodeA.X > con.NodeB.X)
                            con.NodeB.X--;
                        else
                            con.NodeB.Y--;
                    }
                    connections.Single(x => x.HasConnectionTo(con.NodeB) && !ReferenceEquals(con, x)).RefreshDistance();
                    Console.WriteLine(con.NodeB.PortalCode);
                }
                else
                {
                    unused.Add(con.NodeB);
                    if (con.NodeA.X < con.NodeB.X || con.NodeA.Y < con.NodeB.Y)
                    {
                        con.NodeA.PortalCode += con.NodeB.PortalCode;
                        if (con.NodeA.X < con.NodeB.X)
                            con.NodeA.X--;
                        else
                            con.NodeA.Y--;
                    }
                    else
                    {
                        con.NodeA.PortalCode = con.NodeB.PortalCode + con.NodeA.PortalCode;
                        if (con.NodeA.X > con.NodeB.X)
                            con.NodeA.X++;
                        else
                            con.NodeA.Y++;
                    }
                    connections.Single(x => x.HasConnectionTo(con.NodeA) && !ReferenceEquals(con, x)).RefreshDistance();
                    Console.WriteLine(con.NodeA.PortalCode);
                }
            }

            foreach (Node node in unused)
            {
                connections = connections.Where(x => !x.HasConnectionTo(node)).ToList();
                nodes.Remove(node);
            }
            Console.WriteLine("================");

            foreach (Node node in nodes.Where(x => !string.IsNullOrWhiteSpace(x.PortalCode)))
            {
                Console.Write(node.PortalCode + " ");
                var partner = nodes.FirstOrDefault(x => x.PortalCode == node.PortalCode && !ReferenceEquals(x, node));
                if (partner != null && shortcuts.FirstOrDefault(x => x.HasConnectionTo(partner)) == null)
                {
                    var con = new NodeConnection(node, partner, 1);
                    shortcuts.Add(con);
                    Console.Write(" @ing: " + con.ToString());
                }
                else Console.Write(" Already Connected");
                Console.WriteLine();
            }

            var start = nodes.Single(x => x.PortalCode == "AA");
            var end = nodes.Single(x => x.PortalCode == "ZZ");
            foreach (Node node in nodes) node.UpdateTargetDistance(start);
            var pathfind = new AStar(connections.Concat(shortcuts).ToList());


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("PATH");
            double cost = 0;
            List<BaseNodeConnection> cons = null;
            foreach (Node node in pathfind.GetPath(start, end, out cons, out cost))
                Console.WriteLine(node);

            Console.WriteLine();
            Console.WriteLine("CONNS");
            foreach (NodeConnection con in cons)
                Console.WriteLine(con.ToString());

            return "now you're thinking with portals. -> " + cost;
        }

        private void GetNodes(string maze)
        {
            var mazeLines = maze.Replace(' ', '#').Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            nodes = new List<Node>();
            portals = new List<Node>();
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
                    {
                        var above = false;
                        var below = false;
                        var left = false;
                        var right = false;
                        if (y > 0) above = mazeLines[y - 1][x] != wallChar;
                        if (y < mazeLines.Length - 1) below = mazeLines[y + 1][x] != wallChar;
                        if (x > 0) left = mazeLines[y][x - 1] != wallChar;
                        if (x < currLine.Length - 1) right = mazeLines[y][x + 1] != wallChar;

                        //!Av!BvLvR N   AvBv!Lv!R
                        if (((!above || !below || left || right) && (above || below || !left || !right)) || currPos != '.')
                        {
                            var currNode = new Node(x, y, currPos);
                            if (aboveNodes[x] != null) connections.Add(new NodeConnection(currNode, aboveNodes[x]));
                            if (previousNode != null) connections.Add(new NodeConnection(currNode, previousNode));
                            if (!string.IsNullOrWhiteSpace(currNode.PortalCode)) portals.Add(currNode);
                            nodes.Add(currNode);

                            currNode.PathIndex = TraceChars.GetPathNumber(above, below, left, right, false);

                            aboveNodes[x] = currNode;
                            previousNode = currNode;

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
    }
}
