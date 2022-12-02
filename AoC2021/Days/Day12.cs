using AdventOfCode.Days.Tools.Day12;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day12 : DayBase
    {
        public override string Title => "Passage Pathing";
        List<CaveNode> caves = new List<CaveNode>();
        List<BaseNodeConnection> connections = new List<BaseNodeConnection>();
        bool part2 = false;
        CaveNode startNode;
        CaveNode endNode;


        public override string Solve(string input, bool part2)
        {
            this.part2 = part2;
            foreach (var line in GetLines(input))
            {
                var caveNames = line.Split('-');
                var cave0 = caves.FirstOrDefault(x => x.Name == caveNames[0]) ?? new CaveNode(caveNames[0], caveNames[0].ToUpper() == caveNames[0]);
                var cave1 = caves.FirstOrDefault(x => x.Name == caveNames[1]) ?? new CaveNode(caveNames[1], caveNames[1].ToUpper() == caveNames[1]);
                var connection = new BaseNodeConnection(cave0, cave1);
                if (!caves.Contains(cave0))
                    caves.Add(cave0);
                if (!caves.Contains(cave1))
                    caves.Add(cave1);
                connections.Add(connection);
            }
            startNode = caves.First(x => x.Name == "start");
            endNode = caves.First(x => x.Name == "end");
            AllPathTraversal traversal = new AllPathTraversal(connections, startNode, endNode, IsNodeRevisitable);
            var result = traversal.FindAllPaths();
            //foreach (var path in result)
            //{
            //    foreach (var node in path)
            //        Console.Write(node.ToString() + "->");
            //    Console.CursorLeft -= 2;
            //    Console.WriteLine("  ");
            //}
            return $"Found {result.Count} paths";
        }

        private bool IsNodeRevisitable(IReadOnlyList<BaseNode> path, BaseNode nextNode)
        {
            if (((CaveNode)nextNode).Revisitable) return true;

            if (!part2)
                return !path.Any(x => x.Equals(nextNode));

            if (nextNode == startNode) return false;

            var smallCaveSelector = path.Where(x => !((CaveNode)x).Revisitable);

            return !path.Contains(nextNode) || smallCaveSelector.Distinct().Count() == smallCaveSelector.Count();

        }
    }
}
