using AdventOfCode.Days.Tools.Day15;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using AdventOfCode.Tools.Visualization;
using AdventOfCode.Tools.Visualization.DebugForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase, IDisposable
    {
        public override string Title => "Chiton";

        DynamicGrid<AStarNode> riskMap = new DynamicGrid<AStarNode>();
        private bool disposedValue;
        Bitmap map;
        const int scale = 5;
        VisualFormHandler formHandler = VisualFormHandler.GetInstance();
        bool part2;
        ConsoleAssist assist;

        //let's mangle my aStar implementation
        //FIXME: THAT'S A MESS
        public override string Solve(string input, bool part2)
        {
            this.part2 = part2;
            List<string> rows = GetLines(input);
            for (int y = 0; y < rows.Count; ++y)
            {
                string row = rows[y];
                for (int x = 0; x < row.Length; ++x)
                    riskMap.SetRelative(x, y, new AStarNode(x, y, int.Parse(row[x].ToString())));
            }

            if (part2)
            {
                AppendMap();
                assist = new ConsoleAssist();
            }

            map = new Bitmap(riskMap.XDim * scale, riskMap.YDim * scale);
            AStarNode[] path = Pathfind();

            double totalRisk = 0;
            for (int i = 0; i < path.Length; ++i)
            {
                var node = path[i];
                int progress = Convert.ToInt32((double)i / path.Length * 0xFF);
                map.FillRect(new Rectangle(node.X * scale, node.Y * scale, scale, scale), Color.FromArgb(0, progress, 0));
                try
                {
                    Console.SetCursorPosition(node.X, node.Y);
                    Console.Write('█');
                }
                catch { /*Out of bounds will not be rendered in the console window. too bad*/}
                if (node == path.First()) continue;
                totalRisk += node.NodeHeuristic;
            }
            formHandler.Update(map);

            Console.WriteLine();
            Console.WriteLine();

            return "Total Risk: " + Convert.ToInt32(totalRisk).ToString();
        }

        private void AppendMap()
        {
            const int appendCount = 5;
            int originalWidth = riskMap.XDim;
            int originalHeight = riskMap.YDim;
            for (int appendY = 0; appendY < appendCount; ++appendY)
                for (int appendX = 0; appendX < appendCount; ++appendX)
                {
                    if (appendX == 0 && appendY == 0) continue;
                    for (int y = 0; y < originalHeight; ++y)
                        for (int x = 0; x < originalWidth; ++x)
                        {
                            int newX = x + originalWidth * appendX;
                            int newY = y + originalHeight * appendY;
                            double value = riskMap[x, y].NodeHeuristic + appendX + appendY;
                            while (value > 9) value -= 9;
                            riskMap.SetRelative(newX, newY, new AStarNode(newX, newY, value));
                        }
                }
        }

        private AStarNode[] Pathfind()
        {
            List<AStarNodeConnection> connections = new List<AStarNodeConnection>();
            AStarNode start = riskMap.First();
            AStarNode end = riskMap.Last();
            LoadConnections(connections);
            formHandler.Show(map);

            AStarPathfinder aStar = new AStarPathfinder(connections);
            if (part2)
                aStar.OnExpanded += SimpleProgress;
            else
                aStar.OnExpanded += DrawExpansionState;
            AStarNode[] path = aStar.GetPath(start, end).ToArray();
            return path;
        }

        private void LoadConnections(List<AStarNodeConnection> connections)
        {
            foreach (DynamicGridValue<AStarNode> riskNode in riskMap)
            {
                if (riskNode.X > 0)
                {
                    var neighbour = riskMap[riskNode.X - 1, riskNode.Y];
                    connections.Add(new TwoWayNodeConnection(riskNode, neighbour));
                }
                if (riskNode.Y > 0)
                {
                    var neighbour = riskMap[riskNode.X, riskNode.Y - 1];
                    connections.Add(new TwoWayNodeConnection(riskNode, neighbour));
                }

                int risk = Convert.ToInt32(riskNode.Value.NodeHeuristic / 9.0 * 0xFF);
                map.FillRect(new Rectangle(riskNode.X * scale, riskNode.Y * scale, scale, scale), Color.FromArgb(risk, 0, 0));
                if (part2) continue;
                try
                {
                    Console.SetCursorPosition(riskNode.X, riskNode.Y);
                    Console.Write(riskNode.Value.NodeHeuristic);
                }
                catch { /*Out of bounds will not be rendered in the console window. too bad*/}
            }
        }

        private void DrawExpansionState(IReadOnlyList<AStarNode> expanded, IReadOnlyList<AStarNode> considered, AStarNode active)
        {

            double maxExpansion = double.PositiveInfinity;
            double maxLength = double.PositiveInfinity;
            if (expanded.Count > 0)
            {
                maxExpansion = expanded.Max(x => x.ExpansionPriority);
                maxLength = expanded.Max(x => x.PathLength);
            }
            if (maxLength == 0)
                maxLength = double.PositiveInfinity;
            foreach (AStarNode node in expanded)
            {
                int brightness = Convert.ToInt32(node.ExpansionPriority / maxExpansion * 0xFF);
                int risk = Convert.ToInt32(node.NodeHeuristic / 9 * 0xFF);
                int length = Convert.ToInt32(node.PathLength / maxLength * 0xFF);
                map.FillRect(new Rectangle(node.X * scale, node.Y * scale, scale, scale), Color.FromArgb(risk, length, brightness));
            }
            foreach (AStarNode node in considered)
            {
                map.FillRect(new Rectangle(node.X * scale, node.Y * scale, scale, scale), Color.FromArgb(0x7F, 0x7F, 0xFF));
            }
            map.FillRect(new Rectangle(active.X * scale, active.Y * scale, scale, scale), Color.FromArgb(0xFF, 0xFF, 0));
            formHandler.Update(map);
        }

        private void SimpleProgress(IReadOnlyList<AStarNode> expanded, IReadOnlyList<AStarNode> considered, AStarNode active)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("BUSY...");
            Console.Write(assist.GetNextProgressChar());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    map?.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
