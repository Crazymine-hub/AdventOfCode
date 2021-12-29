using AdventOfCode.Days.Tools.Day15;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using AdventOfCode.Tools.Visualization;
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

        //let's mangle my aStar implementation
        //FIXME: THAT'S A MESS
        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            List<string> rows = GetLines(input);
            for (int y = 0; y < rows.Count; ++y)
            {
                string row = rows[y];
                for (int x = 0; x < row.Length; ++x)
                    riskMap.SetRelative(x, y, new AStarNode(x, y, int.Parse(row[x].ToString())));
            }

            map = new Bitmap(riskMap.XDim * scale, riskMap.YDim * scale);

            List<AStarNodeConnection> connections = new List<AStarNodeConnection>();
            AStarNode start = riskMap.First();
            AStarNode end = riskMap.Last();

            foreach (DynamicGridValue<AStarNode> riskNode in riskMap)
            {
                List<double> areaHeat = new List<double>() { riskNode.Value.NodeHeuristic };
                if (riskNode.X > 0)
                {
                    var neighbour = riskMap[riskNode.X - 1, riskNode.Y];
                    connections.Add(new TwoWayNodeConnection(riskNode, neighbour));
                    areaHeat.Add(neighbour.NodeHeuristic);
                }
                if (riskNode.Y > 0)
                {
                    var neighbour = riskMap[riskNode.X, riskNode.Y - 1];
                    connections.Add(new TwoWayNodeConnection(riskNode, neighbour));
                    areaHeat.Add(neighbour.NodeHeuristic);
                }
                if (riskNode.X < riskMap.XDim - 1)
                    areaHeat.Add(riskMap[riskNode.X + 1, riskNode.Y].NodeHeuristic);
                if (riskNode.Y < riskMap.YDim - 1)
                    areaHeat.Add(riskMap[riskNode.X, riskNode.Y + 1].NodeHeuristic);


                int risk = Convert.ToInt32(riskNode.Value.NodeHeuristic / 9.0 * 0xFF);
                map.FillRect(new Rectangle(riskNode.X * scale, riskNode.Y * scale, scale, scale), Color.FromArgb(risk, risk, risk));
                try
                {
                    Console.SetCursorPosition(riskNode.X, riskNode.Y);
                    Console.Write(riskNode.Value.NodeHeuristic);
                }
                catch { /*Out of bounds will not be rendered in the console window. too bad*/}
            }
            VisualFormHandler.Instance.Show(map);

            AStarPathfinder aStar = new AStarPathfinder(connections);
            aStar.OnExpanded += DrawExpansionState;
            AStarNode[] path = aStar.GetPath(start, end).ToArray();
            double totalRisk = 0;
            for (int i = 0; i < path.Length; ++i)
            {
                var node = path[i];
                int progress = Convert.ToInt32((double)i / path.Length * 0xFF);
                map.FillRect(new Rectangle(node.X * scale, node.Y * scale, scale, scale), Color.FromArgb(255 - progress, progress, 0));
                try
                {
                    Console.SetCursorPosition(node.X, node.Y);
                    Console.Write('█');
                }
                catch { /*Out of bounds will not be rendered in the console window. too bad*/}
                if (node == path.First()) continue;
                totalRisk += node.NodeHeuristic;
            }
            VisualFormHandler.Instance.Update(map);

            Console.WriteLine();
            Console.WriteLine();

            return "Total Risk: " + Convert.ToInt32(totalRisk).ToString();
        }

        private void DrawExpansionState(IReadOnlyList<AStarNode> expanded, IReadOnlyList<AStarNode> considered, AStarNode active)
        {
            double maxExpansion = double.PositiveInfinity;
            if(expanded.Count > 0)
                maxExpansion = expanded.Max(x => x.ExpansionPriority);
            foreach (AStarNode node in expanded)
            {
                int brightness = Convert.ToInt32(node.ExpansionPriority / maxExpansion * 0xFF);
                map.FillRect(new Rectangle(node.X * scale, node.Y * scale, scale, scale), Color.FromArgb(0, 0, brightness));
            }
            foreach (AStarNode node in considered)
            {
                map.FillRect(new Rectangle(node.X * scale, node.Y * scale, scale, scale), Color.FromArgb(0x7F, 0x7F, 0xFF));
            }
            map.FillRect(new Rectangle(active.X * scale, active.Y * scale, scale, scale), Color.FromArgb(0xFF, 0xFF, 0));
            VisualFormHandler.Instance.Update(map);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    map.Dispose();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~Day15()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
