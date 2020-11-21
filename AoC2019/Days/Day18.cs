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
        List<Node> keys;
        List<Node> doors;
        int maxHeight = 0;
        int printouts = 0;
        AStar pathfind;
        ConsoleAssist outAssis = new ConsoleAssist();

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            GetNodes(input);
            PrintConnections(connections, 0, false);
            pathfind = new AStar(connections);

            int totalMoves = 0;
            string order = "";


            Console.SetCursorPosition(0, maxHeight * (printouts+1));
            var path = GetKeyOrder();
            var singleHeight = Console.CursorTop - maxHeight;
            BaseNode currStart = startNode;
            foreach (Node next in path)
            {
                pathfind.GetPath(currStart, next, out List<BaseNodeConnection> cons, out int dist);
                totalMoves += dist;
                PrintConnections(connections, maxHeight * ++printouts + singleHeight, false);
                PrintConnections(cons, maxHeight * printouts + singleHeight, true);
                currStart = next;
                order += next.Key;
            }

            Console.SetCursorPosition(0, maxHeight * ++printouts + singleHeight);
            return "Moves: " + totalMoves + " Order: " + order;
        }

        private List<Node> Prioritize()
        {
            AStar pathfind = new AStar(connections);
            List<List<Node>> unlockedPaths = new List<List<Node>>();
            List<Node> pois = keys.Concat(doors).ToList();

            Console.WriteLine("Creating Tree...");
            Console.WriteLine("Step 1/5 Preparing".PadRight(20));

            for (int i = 0; i < connections.Count(); i++)
            {
                ((NodeConnection)connections[i]).AllowPassLockedDoor = true;
                Console.CursorLeft = 0;
                Console.Write((outAssis.GetNextProgressChar() + " " + (connections.Count / 100f * i) + "%").PadRight(20));
            }

            Console.CursorLeft = 0;
            Console.WriteLine("Step 2/5 Getting Paths".PadRight(20));

            for (int i = 0; i < pois.Count(); i++)
            {
                unlockedPaths.Add(pathfind.GetPath(startNode, pois[i], out List<BaseNodeConnection> cons, out int cost).Select(x => (Node)x).ToList());
                Console.CursorLeft = 0;
                Console.Write((outAssis.GetNextProgressChar() + " " + (pois.Count / 100f * i) + "%").PadRight(20));
            }

            Console.CursorLeft = 0;
            Console.WriteLine("Step 3/5 Detecting".PadRight(20));

            for (int i = 0; i < unlockedPaths.Count(); i++)
            {
                List<Node> path = unlockedPaths[i];
                Node target = path.Last();
                Node blocker = path.LastOrDefault(x => x != target && !(x.Key == '\0' && x.Lock == '\0'));
                target.BlockedBy = blocker;
                Console.CursorLeft = 0;
                Console.Write((outAssis.GetNextProgressChar() + " " + (unlockedPaths.Count / 100f * i) + "%").PadRight(20));
            }

            Console.CursorLeft = 0;
            Console.WriteLine("Step 4/5 Ordering".PadRight(20));

            List<Node> orderedPoi = pois.Where(x => x.BlockedBy == null).ToList();

            for (int i = 0; i < orderedPoi.Count(); i++)
            {
                orderedPoi.InsertRange(i + 1, pois.Where(x => x.BlockedBy == orderedPoi[i]));
                Console.CursorLeft = 0;
                Console.Write((outAssis.GetNextProgressChar() + " " + (pois.Count / 100f * i) + "%").PadRight(20));
            }


            Console.CursorLeft = 0;
            Console.WriteLine("Step 5/5 Cleaning up".PadRight(20));

            for (int i = 0; i < connections.Count(); i++)
            {
                ((NodeConnection)connections[i]).AllowPassLockedDoor = false;
                Console.CursorLeft = 0;
                Console.Write((outAssis.GetNextProgressChar() + " " + (connections.Count / 100f * i) + "%").PadRight(20));
            }

            Console.CursorLeft = 0;
            Console.WriteLine("DONE!".PadRight(20));
            return orderedPoi;
        }

        private void GetNodes(string maze)
        {
            var mazeLines = maze.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            nodes = new List<BaseNode>();
            keys = new List<Node>();
            doors = new List<Node>();
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
                            if (currNode.Key != '\0') keys.Add(currNode);
                            if (currNode.Lock != '\0') doors.Add(currNode);
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

        private void PrintConnections(List<BaseNodeConnection> connections, int vOffset = 0, bool bold = false)
        {
            foreach (NodeConnection con in connections)
            {
                Console.SetCursorPosition(con.NodeA.X, con.NodeA.Y + vOffset);
                if (con.NodeA.Key != '\0')
                    Console.Write(con.NodeA.Key);
                else if (con.NodeA.Lock != '\0')
                    Console.Write(con.NodeA.Lock);
                else
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
                        Console.CursorTop = i + vOffset;
                        Console.Write(TraceChars.GetPathChar(true, true, false, false, bold));
                    }
                }

                Console.SetCursorPosition(con.NodeB.X, con.NodeB.Y + vOffset);
                if (con.NodeB.Key != '\0')
                    Console.Write(con.NodeB.Key);
                else if (con.NodeB.Lock != '\0')
                    Console.Write(con.NodeB.Lock);
                else
                    Console.Write(TraceChars.paths[con.NodeB.PathIndex | (bold ? 16 : 0)]);
            }
        }

        private Node GetClosestKey(Node start, List<Node> keys)
        {
            return GetClosestKey(start, keys, out int dist, out List<BaseNodeConnection> trace);
        }

        private Node GetClosestKey(Node start, List<Node> keys, out int distance, out List<BaseNodeConnection> trace)
        {
            distance = 0;
            trace = new List<BaseNodeConnection>();
            Node key = null;

            foreach (Node target in keys)
            {
                foreach (Node node in nodes)
                    node.UpdateTargetDistance(target);
                var path = pathfind.GetPath(start, target, out var traceConnections, out int moves);
                if (path.LastOrDefault() != target) continue;
                if (moves < distance || distance == 0)
                {
                    distance = moves;
                    key = target;
                    trace = traceConnections;
                }
            }
            return key;
        }

        private List<Node> GetReachableNodes(List<Node> nodes, Node tryKey = null)
        {
            List<Node> reachable = nodes.Where(x => x.BlockedBy == null).ToList();
            for (int i = 0; i < reachable.Count(); i++)
            {
                if (!reachable[i].IsUnlocked && !reachable[i].CanBeUnlocked(tryKey?.Key ?? '\0')) continue;
                reachable.InsertRange(i + 1, nodes.Where(x => x.BlockedBy == reachable[i]));
            }
            return reachable;
        }

        private List<Node> GetKeys(List<Node> nodes)
        {
            return nodes.Where(x => x.Key != '\0').ToList();
        }

        private List<Node> GetKeyOrder()
        {
            var prios = Prioritize();
            List<Node> keyOrder = new List<Node>();
            Console.Write("Finding optimal Path... ");

            while (keyOrder.Count < this.keys.Count)
            {
                Console.CursorLeft--;
                Console.Write(outAssis.GetNextProgressChar());
                foreach (Node door in prios)
                {
                    door.IsUnlocked = false;
                    foreach (Node key in keyOrder)
                        door.TryUnlock(key.Key);
                }

                var reachableNodes = GetReachableNodes(prios);
                var keys = GetKeys(reachableNodes).Where(x => !keyOrder.Contains(x)).ToList();
                Node lastKey = null;

                foreach (Node key in keys)
                {
                    Console.CursorLeft--;
                    Console.Write(outAssis.GetNextProgressChar());
                    var reach = GetReachableNodes(prios, key);
                    if (reach.Count > reachableNodes.Count)
                    {
                        reachableNodes = reach;
                        lastKey = key;
                    }
                }
                if (lastKey == null)
                {
                    while (keys.Count > 0)
                    {
                        var closest = GetClosestKey(keyOrder.Last(), keys);
                        keyOrder.Add(closest);
                        keys.Remove(closest);
                    }
                    Console.WriteLine();
                    return keyOrder;
                }
                foreach (Node node in pathfind.GetPath(keyOrder.LastOrDefault() ?? startNode, lastKey).Select(x => (Node)x))
                    if (node.Key != '\0' && !keyOrder.Contains(node)) keyOrder.Add(node);
            }

            Console.WriteLine();
            return keyOrder;
        }
    }
}
