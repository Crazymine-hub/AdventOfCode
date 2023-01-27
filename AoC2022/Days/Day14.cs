using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day14: DayBase
    {
        public override string Title => "Regolith Reservoir";
        private readonly Point sandSpawn = new Point(500, 0);
        VisualFormHandler visualForm = VisualFormHandler.GetInstance();
        private Bitmap bitmap;
        private const int PixelSize = 5;

        public override string Solve(string input, bool part2)
        {
            DynamicGrid<bool?> caves = LoadCaveLayout(input);

            bitmap = new Bitmap(caves.XDim * PixelSize, caves.YDim * PixelSize);
            bitmap.FillRect(new Rectangle(0, 0, bitmap.Width, bitmap.Height), Color.Black);

            foreach (var unit in caves)
            {
                if (!unit.Value.HasValue) continue;
                bitmap.FillRect(new Rectangle(unit.X * PixelSize, unit.Y * PixelSize, PixelSize, PixelSize), Color.Gray);
            }
            visualForm.Show(bitmap);


            ulong capacity = RunSimulation(caves, part2);
            RenderCaveSystem(sandSpawn, caves);

            return $"The system is filled with {capacity} units of sand";
        }

        private ulong RunSimulation(DynamicGrid<bool?> caves, bool part2)
        {
            ulong processedUnits = 0;
            Point current = new Point(sandSpawn.X, sandSpawn.Y);

            var maxDepth = caves.Where(x => x.Value.HasValue && !x.Value.Value).Max(x => x.Y);
            caves.MakeAvaliable(500 + caves.XOrigin, maxDepth + 1 + caves.YOrigin);

            while (true)
            {
                const int RenderOffset = 5;
                if (current.Y > maxDepth)
                {
                    if (part2)
                    {
                        caves.SetRelative(current.X, current.Y, true);
                        processedUnits++;
                        if (processedUnits % RenderOffset == 0)
                            RenderCaveSystem(current, caves);
                        current = new Point(sandSpawn.X, sandSpawn.Y);
                    }
                    else break;
                }

                if (!caves.GetRelative(current.X, current.Y + 1).HasValue)
                {
                    current.Offset(0, 1);
                    continue;
                }

                if (!caves.GetRelative(current.X - 1, current.Y + 1).HasValue)
                {
                    current.Offset(-1, 1);
                    continue;
                }

                if (!caves.GetRelative(current.X + 1, current.Y + 1).HasValue)
                {
                    current.Offset(1, 1);
                    continue;
                }

                if (part2 && caves.GetRelative(current.X, current.Y).HasValue)
                    break;

                caves.SetRelative(current.X, current.Y, true);
                processedUnits++;
                if (processedUnits % RenderOffset == 0)
                    RenderCaveSystem(current, caves);
                current = new Point(sandSpawn.X, sandSpawn.Y);
            }

            return processedUnits;
        }

        private void RenderCaveSystem(Point current, DynamicGrid<bool?> caves)
        {
            if (caves.XDim * PixelSize != bitmap.Width || caves.YDim * PixelSize != bitmap.Height)
            {
                bitmap.Dispose();
                bitmap = new Bitmap(caves.XDim * PixelSize, caves.YDim * PixelSize);
                bitmap.FillRect(0, 0, bitmap.Width, bitmap.Height, Color.Black);
            }

            foreach (var unit in caves)
            {
                if (!unit.Value.HasValue) continue;
                bitmap.FillRect(unit.X * PixelSize, unit.Y * PixelSize, PixelSize, PixelSize, unit.Value.Value ? Color.Yellow : Color.Gray);
            }
            visualForm.Update(bitmap);
        }

        private DynamicGrid<bool?> LoadCaveLayout(string input)
        {
            var caves = new DynamicGrid<bool?>();
            foreach (var line in GetLines(input))
            {
                Point? previous = null;
                foreach (Match coordinate in Regex.Matches(line, @"(\d+),(\d+)"))
                {
                    var point = new Point(int.Parse(coordinate.Groups[1].Value), int.Parse(coordinate.Groups[2].Value));
                    if (previous == null)
                    {
                        previous = point;
                        continue;
                    }

                    foreach (Point linePoint in VectorAssist.GetPointsFromLine(previous.Value, point))
                    {
                        caves.SetRelative(linePoint.X, linePoint.Y, false);
                    }
                    previous = point;
                }
            }

            caves.CutDown();

            caves.AddMargin(1);
            caves.SetRelative(sandSpawn.X, sandSpawn.Y, null);

            return caves;
        }
    }
}
