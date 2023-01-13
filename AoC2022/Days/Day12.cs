using AdventOfCode.Days.Tools.Day12;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day12 : DayBase
    {
        public override string Title => "Hill Climbing Algorithm";

        const int pixelSize = 5;
        private VisualFormHandler form;

        public override string Solve(string input, bool part2)
        {
            DynamicGrid<HeightNode> map;
            HeightNode startNode, endNode;
            List<AStarNodeConnection> connections;
            LoadMap(input, out map, out startNode, out endNode, out connections);

            List<AStarNode> shortestPath = null;
            AStarPathfinder aStar = new AStarPathfinder(connections);
            List<HeightNode> startNodes = new List<HeightNode>() { startNode };
            if (part2)
            {
                var stepStones = map.Select(x => x.Value).Where(x => x.Height == 1);
                startNodes = new List<HeightNode>();
                foreach (var stepStone in stepStones)
                {
                    startNodes.AddRange(map.GetNeighbours(stepStone.X, stepStone.Y, false, true)
                        .Select(x => x.Value)
                        .Where(x => x != null && x.Height == 0 && !startNodes.Contains(x)));
                }
            }

            form = VisualFormHandler.GetInstance();
            form.Show();

            foreach (var start in startNodes)
            {
                Console.WriteLine($"{start} -> {endNode}");
                var path = aStar.GetPath(start, endNode).ToList();
                var image = PrepareImage(map, start, endNode);
                Task.Delay(10).Wait();
                if (path.Count == 0)
                {
                    Console.WriteLine("No Path");
                    continue;
                }
                RenderPath(image, map, path);
                if (shortestPath == null || path.Count < shortestPath.Count)
                    shortestPath = path;
                Console.WriteLine();
            }

            if (shortestPath == null) throw new Exception("No Path found");


            RenderPath(PrepareImage(map, null, null), map, shortestPath);
            return $"You made {shortestPath.Count - 1} Steps";
        }

        private void LoadMap(string input, out DynamicGrid<HeightNode> map, out HeightNode startNode, out HeightNode endNode, out List<AStarNodeConnection> connections)
        {
            Console.Write("Loading Map Data...");
            var con = new ConsoleAssist();
            Console.Write(con.GetNextProgressChar());
            Console.CursorLeft--;
            map = new DynamicGrid<HeightNode>();
            startNode = null;
            endNode = null;
            connections = new List<AStarNodeConnection>();
            var mapLines = GetLines(input);
            for (int y = 0; y < mapLines.Count; ++y)
            {
                var line = mapLines[y];
                for (int x = 0; x < line.Length; ++x)
                {
                    Console.Write(con.GetNextProgressChar());
                    Console.CursorLeft--;
                    var nodeValue = line[x] - 'a';
                    var node = new HeightNode(x, y, nodeValue);
                    if (line[x] == 'S')
                    {
                        startNode = new HeightNode(x, y, 0);
                        node = startNode;
                    }
                    if (line[x] == 'E')
                    {
                        endNode = new HeightNode(x, y, 25);
                        node = endNode;
                    }
                    map.SetRelative(x, y, node);
                }
            }

            if (startNode == null || endNode == null) throw new InvalidOperationException("Either the start or end could not be found.");

            Console.WriteLine(' ');
            Console.Write("Building Connections...");
            Console.Write(con.GetNextProgressChar());
            Console.CursorLeft--;
            foreach (var start in map)
            {
                foreach (var target in map.GetNeighbours(start.X, start.Y, diagonal: false))
                {
                    Console.Write(con.GetNextProgressChar());
                    Console.CursorLeft--;
                    if (target.Value == null || start.Value == target.Value) continue;
                    var difference = target.Value.Height - start.Value.Height;
                    var direction = ConnectionDirection.None;
                    if (difference <= 1)
                        direction |= AdventOfCode.Tools.Pathfinding.ConnectionDirection.AToB;
                    if (difference >= -1)
                        direction |= AdventOfCode.Tools.Pathfinding.ConnectionDirection.BToA;
                    var connection = new AStarNodeConnection(start, target, direction);
                    if (!connections.Any(x => x.IsSameConnection(connection)))
                        connections.Add(connection);
                }
            }
            Console.WriteLine(' ');
        }


        private Bitmap PrepareImage(DynamicGrid<HeightNode> map, HeightNode startNode, HeightNode endNode)
        {
            Console.WriteLine(map.GetStringRepresentation((HeightNode node, int x, int y) => (node == startNode || node == endNode) ? " " : Convert.ToChar(node.Height + 'a').ToString()));

            Bitmap bitmap = new Bitmap(map.XDim * pixelSize, map.YDim * pixelSize);
            foreach (var point in map)
            {
                var value = Convert.ToInt32(Math.Round(point.Value.Height / 25.0 * 255));
                var color = Color.FromArgb(value, value, value);
                if (point.Value == startNode)
                    color = Color.Red;
                if (point.Value == endNode)
                    color = Color.Blue;
                bitmap.FillRect(new Rectangle(point.X * pixelSize, point.Y * pixelSize, pixelSize, pixelSize), color);
            }

            form.Update(bitmap);
            return bitmap;
        }


        private void RenderPath(Bitmap bitmap, DynamicGrid<HeightNode> map, List<AStarNode> path)
        {
            Console.WriteLine(map.GetStringRepresentation((HeightNode node, int x, int y) => path.Contains(node) ? "#" : Convert.ToChar(node.Height + 'a').ToString()));

            foreach (var point in map)
            {
                var value = Convert.ToInt32(Math.Round(point.Value.Height / 25.0 * 255));
                var color = Color.FromArgb(value, value, value);
                if (path.Contains(point))
                {
                    value = Convert.ToInt32(Math.Round(path.IndexOf(point) * 1.0 / path.Count * 255));
                    color = Color.FromArgb(255 - value, 0, value);
                }
                bitmap.FillRect(new Rectangle(point.X * pixelSize, point.Y * pixelSize, pixelSize, pixelSize), color);
            }

            form.Update(bitmap);
        }
    }
}
