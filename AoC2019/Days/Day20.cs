﻿using AdventOfCode.Days.Classes.Day20;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        Node start = null;
        Node end = null;
        Dictionary<BaseNodeConnection, Tuple<List<Node>, double>> portalAccess = null;
        ConsoleAssist assis = new ConsoleAssist();
        AStar pathfind = null;

        public override string Solve(string input, bool part2)
        {
            LoadMaze(input);
            var finalPath = TraceRecursively(start, new List<NodeConnection>(), out double cost);
            if (finalPath == null) return "No Path found! (The Cake is a lie)";
            finalPath.Reverse();
            Console.WriteLine("================");

            foreach (var entry in finalPath) Console.WriteLine(entry.ToString());

            return "now you're thinking with portals. -> " + cost;
        }

        private void LoadMaze(string input)
        {
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
                    portals.Add(con.NodeB);
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
                    portals.Add(con.NodeA);
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

            start = nodes.Single(x => x.PortalCode == "AA");
            end = nodes.Single(x => x.PortalCode == "ZZ");
            portalAccess = new Dictionary<BaseNodeConnection, Tuple<List<Node>, double>>();

            pathfind = new AStar(connections);

            foreach (Node portalA in portals)
            {
                foreach (Node opposite in nodes) opposite.UpdateTargetDistance(portalA);
                foreach (Node portalB in portals.Where(x => x != portalA))
                {
                    Console.CursorLeft = 0;
                    Console.Write(assis.GetNextProgressChar());
                    var con = new BaseNodeConnection(portalA, portalB);
                    if (portalAccess.Keys.SingleOrDefault(x => x.IsSameConnection(con)) != null) continue;
                    var path = pathfind.GetPath(portalA, portalB, out double currCost).Select(x => (Node)x).ToList();
                    if (path.Last() == portalA) path = null;
                    portalAccess.Add(con, new Tuple<List<Node>, double>(path, currCost));
                }
            }
        }

        private List<BaseNodeConnection> TraceRecursively(BaseNode currNode, List<NodeConnection> usedShortcuts, out double cost)
        {
            cost = 0;
            var cons = portalAccess.Where(x => x.Value.Item1 != null && x.Key.HasConnectionTo(currNode)).ToList();
            double minCost = 0;
            List<BaseNodeConnection> result = null;
            BaseNodeConnection selCon = null;
            BaseNodeConnection selShort = null;
            double selCost = 0;
            foreach (var con in cons)
            {
                Console.CursorLeft = 0;
                Console.Write(assis.GetNextProgressChar());
                Node partner = (Node)con.Key.GetOtherNode(currNode);
                if (partner == start) continue;
                if (partner == end)
                {
                    cost = con.Value.Item2;
                    return new List<BaseNodeConnection>() { con.Key };
                }
                if (usedShortcuts.FirstOrDefault(x => x.HasConnectionTo(partner)) != null) continue;
                var shortCut = shortcuts.Single(x => x.HasConnectionTo(partner));
                usedShortcuts.Add(shortCut);
                var currPath = TraceRecursively(shortCut.GetOtherNode(partner), usedShortcuts, out double currCost);
                usedShortcuts.Remove(shortCut);
                currCost++;
                if (currPath != null && currCost < minCost || minCost == 0)
                {
                    minCost = currCost;
                    result = currPath;
                    selCost = con.Value.Item2;
                    selCon = con.Key;
                    selShort = shortCut;
                }
            }
            if (result == null || selCon == null) return null;
            cost = minCost + selCost + 1;
            result.Add(selShort);
            result.Add(selCon);
            return result;
        }

        private void GetNodes(string maze)
        {
            var mazeLines = maze.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            nodes = new List<Node>();
            portals = new List<Node>();
            connections = new List<BaseNodeConnection>();
            Node[] aboveNodes = new Node[mazeLines[0].Length];
            maxHeight = mazeLines.Length + 1;
            Point start = Point.Empty;
            Point end = Point.Empty;


            for (int y = 0; y < mazeLines.Length; y++)
            {
                var currLine = mazeLines[y].Replace(' ', '#');
                if (start == Point.Empty)
                {
                    var startX = mazeLines[y].IndexOf('#');
                    if (startX >= 0)
                        start = new Point(startX, y);
                }
                else
                {
                    var endX = mazeLines[y].LastIndexOf('#');
                    if (endX >= 0)
                        end = new Point(endX, y);
                }
                mazeLines[y] = currLine;
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
                            currNode.IsOuterPortal = x < start.X || y < start.Y || x > end.X || y > end.Y;
                            if (aboveNodes[x] != null) connections.Add(new NodeConnection(currNode, aboveNodes[x]));
                            if (previousNode != null) connections.Add(new NodeConnection(currNode, previousNode));
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
