using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Pathfinding;
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

        DynamicGrid<BaseNode> riskMap = new DynamicGrid<BaseNode>();
        private bool disposedValue;
        Bitmap map;
        const int scale = 5;

        //let's mangle my aStar implementation
        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            List<string> rows = GetLines(input);
            for (int y = 0; y < rows.Count; ++y)
            {
                string row = rows[y];
                for (int x = 0; x < row.Length; ++x)
                    riskMap.SetRelative(x, y, new BaseNode(x, y) { ClientHeat = int.Parse(row[x].ToString())} );
            }

            map = new Bitmap(riskMap.XDim * scale, riskMap.YDim* scale);

            List<BaseNodeConnection> connections = new List<BaseNodeConnection>();
            BaseNode start = riskMap.First();
            BaseNode end = riskMap.Last();

            foreach(DynamicGridValue<BaseNode> riskNode in riskMap)
            {
                List<double> areaHeat = new List<double>() { riskNode.Value.ClientHeat};
                if (riskNode.X > 0)
                {
                    var neighbour = riskMap[riskNode.X - 1, riskNode.Y];
                    connections.Add(new BaseNodeConnection(riskNode, neighbour));
                    areaHeat.Add(neighbour.ClientHeat);
                }
                if (riskNode.Y > 0)
                {
                    var neighbour = riskMap[riskNode.X, riskNode.Y - 1];
                    connections.Add(new BaseNodeConnection(riskNode, neighbour));
                    areaHeat.Add(neighbour.ClientHeat);
                }
                if (riskNode.X < riskMap.XDim - 1)
                    areaHeat.Add(riskMap[riskNode.X + 1, riskNode.Y].ClientHeat);
                if (riskNode.Y < riskMap.YDim - 1)
                    areaHeat.Add(riskMap[riskNode.X, riskNode.Y + 1].ClientHeat);

                riskNode.Value.Heat = riskNode.Value.ClientHeat / 9; // areaHeat.Aggregate((double accumulator, double next) => accumulator + next) / 45.0;


                int risk = Convert.ToInt32(riskNode.Value.Heat * 0xFF);
                map.FillRect(new Rectangle(riskNode.X * scale, riskNode.Y * scale, scale, scale), Color.FromArgb(risk, risk, risk));
                try
                {
                    Console.SetCursorPosition(riskNode.X, riskNode.Y);
                    Console.Write(riskNode.Value.ClientHeat);
                }
                catch { /*Out of bounds will not be rendered in the console window. too bad*/}
            }
            VisualFormHandler.Instance.Show(map);

            AStar aStar = new AStar(connections);
            BaseNode[] path = aStar.GetPath(start, end);
            double totalRisk = 0;
            for(int i = 0; i < path.Length; ++i)
            {
                var node = path[i];
                int risk = Convert.ToInt32(node.Heat * 0xFF);
                int progress = Convert.ToInt32((double)i / path.Length * 0x7F);
                map.FillRect(new Rectangle(node.X * scale, node.Y * scale, scale, scale), Color.FromArgb(risk, progress, 0));
                //try
                //{
                //    Console.SetCursorPosition(node.X, node.Y);
                //    Console.Write('█');
                //}
                //catch { /*Out of bounds will not be rendered in the console window. too bad*/}
                if (node == path.First()) continue;
                totalRisk += node.ClientHeat;
            }
            VisualFormHandler.Instance.Update(map);

            return "Total Risk: " + Convert.ToInt32(totalRisk).ToString();
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
