using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Graphics;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day5 : DayBase, IDisposable
    {
        public override string Title => "Hydrothermal Venture";

        List<Line> smokerLines = new List<Line>();
        private bool isDisposed;
        private Bitmap smokerPlan = null;
        private DynamicGrid<int> smokerGrid = new DynamicGrid<int>();
        const int scale = 1;

        public override string Solve(string input, bool part2)
        {
            foreach (string vectorExpression in GetLines(input))
            {
                var expression = Regex.Match(vectorExpression, @"^(\d+),(\d+) -> (\d+),(\d+)$");
                if (!expression.Success) throw new Exception("Unable to parse vector expression");
                int x1 = int.Parse(expression.Groups[1].Value);
                int y1 = int.Parse(expression.Groups[2].Value);
                int x2 = int.Parse(expression.Groups[3].Value);
                int y2 = int.Parse(expression.Groups[4].Value);
                smokerLines.Add(new Line(new Point(x1, y1), new Point(x2, y2)));
            }
            int height = smokerLines.Max(x => Math.Max(x.Start.Y, x.End.Y)) + 1;
            int width = smokerLines.Max(x => Math.Max(x.Start.X, x.End.X)) + 1;
            smokerPlan = new Bitmap(width * scale, height * scale);
            smokerPlan.FillRect(new Rectangle(0, 0, width * scale, height * scale), Color.Black);
            VisualFormHandler.Instance.Show((Image)smokerPlan.Clone());
            MapSmokers(part2);
            double max = smokerGrid.Max();
            int higherCount = 0;
            int offset = (scale - 1) / 2;
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    double relative = smokerGrid[x, y] / max;
                    if (smokerGrid[x, y] >= 2)
                    {
                        higherCount++;
                        Console.WriteLine($"Intersection at ({x}|{y}) ({higherCount})");
                        smokerPlan.FillRect(new Rectangle(x * scale, y * scale, scale, scale), ColorHelper.ColorFromHSV(360 * relative, 1, 1));
                    }
                }
            VisualFormHandler.Instance.Update(smokerPlan);
            return $"Found {higherCount} dangerous areas";
        }

        private void MapSmokers(bool useAll)
        {
            IEnumerable<Line> smokers = smokerLines;
            if (!useAll)
                smokers = smokers.Where(x =>
                {
                    Point vector = VectorAssist.PointDifference(x.Start, x.End);
                    return vector.X == 0 || vector.Y == 0;
                });

            foreach (Line line in smokers)
            {
                Console.WriteLine("Processing " + line.ToString());
                var points = VectorAssist.GetPointsFromLine(line.Start, line.End);
                foreach (Point point in points)
                {
                    smokerGrid.MakeAvaliable(point.X, point.Y);
                    ++smokerGrid[point.X, point.Y];
                }
                smokerPlan.DrawPoints(points.Select(x => new Point(x.X * scale, x.Y * scale)), Color.FromArgb(20, 20, 20), scale);
                VisualFormHandler.Instance.Update(smokerPlan);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    smokerPlan?.Dispose();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                isDisposed = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~Day5()
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
