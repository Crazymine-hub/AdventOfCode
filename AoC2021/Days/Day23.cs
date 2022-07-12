using AdventOfCode.Days.Tools.Day23;
using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day23 : DayBase
    {
        public override string Title => "Amphipod";

        private List<AmphipodNode> nodes = new List<AmphipodNode>();
        private List<AStarNodeConnection> connections = new List<AStarNodeConnection>();
        private readonly char[] ampiphodeNames = new char[] { 'A', 'B', 'C', 'D' };
        private AStarPathfinder pathfinder;


        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            LoadLayout(input);

            foreach (var node in nodes)
            {
                Console.SetCursorPosition(node.X, node.Y);
                if(IsAtHome(node))
                    Console.Write("H");
                else if (CanBeMoved(node))
                    Console.Write("O");
                else if (node.CanOccupy)
                    Console.Write("+");
                else
                    Console.Write("X");
            }
            pathfinder = new AStarPathfinder(connections);

            Console.WriteLine();
            return Part2UnavailableMessage;
        }

        private void LoadLayout(string input)
        {
            var lines = GetLines(input);
            int lineLength = lines.Select(l => l.Length).Max();
            for (int lineNr = 0; lineNr < lines.Count; lineNr++)
            {
                int neighbourIndex = 0;
                AmphipodNode lastNode = null;
                var line = lines[lineNr].PadRight(lineLength);
                for (int charNr = 0; charNr < line.Length; charNr++)
                {
                    if (line[charNr] == ' ' || line[charNr] == '#') continue;
                    bool hasTunnel = lineNr == 1 && lines[lineNr + 1][charNr] != '#';

                    var node = new AmphipodNode(charNr,
                                                lineNr,
                                                !hasTunnel,
                                                lineNr > 1 && line[charNr] != '#' ? ampiphodeNames[neighbourIndex++] : '\0',
                                                line[charNr] == '.' ? '\0' : line[charNr]);
                    nodes.Add(node);
                    if (lastNode != null)
                        connections.Add(new AStarNodeConnection(lastNode, node));
                    lastNode = node;
                    var nodeAbove = nodes.SingleOrDefault(x => x.X == charNr && x.Y == lineNr - 1);
                    if (nodeAbove != null)
                        connections.Add(new AStarNodeConnection(node, nodeAbove));
                }
            }
        }

        private bool IsAtHome(AmphipodNode node) => node.HomeTo != '\0' && node.HomeTo == node.OccupiedBy;
        private bool CanBeMoved(AmphipodNode node) => node.OccupiedBy != '\0' && connections.Where(x => x.HasConnectionTo(node)).Any(x => ((AmphipodNode)x.GetOtherNode(node)).OccupiedBy == '\0');
    }
}
