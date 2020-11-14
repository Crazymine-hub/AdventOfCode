using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Days.Classes.Day18;

namespace AdventOfCode.Days
{
    class Day18 : DayBase
    {
        const char wallChar = '#';
        List<Node> nodes;
        List<NodeConnection> connections;
        Node startNode = null;
        List<Node> targets;

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            GetNodes(input);
            return "";
        }

        private void GetNodes(string maze)
        {
            var mazeLines = maze.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            nodes = new List<Node>();
            targets = new List<Node>();
            connections = new List<NodeConnection>();
            Node[] aboveNodes = new Node[mazeLines[0].Length];


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
    }
}
