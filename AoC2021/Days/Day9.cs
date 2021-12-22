using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day9 : DayBase
    {
        public override string Title => "Smoke Basin";

        int[][] grid;
        const int scale = 4;
        readonly char[] gradient = { '-', '=', '≡', '/', '░', '▒', '▓', '█' };

        public override string Solve(string input, bool part2)
        {
            grid = GetLines(input).Select(line => line.Select(pos => int.Parse(pos.ToString())).ToArray()).ToArray();

            using (Bitmap map = new Bitmap(grid[0].Length * scale, grid.Length * scale))
            {
                VisualFormHandler.Instance.Show((Image)map.Clone());

                int lowSum = 0;
                Dictionary<Point, List<Point>> basins = new Dictionary<Point, List<Point>>();
                for (int y = 0; y < grid.Length; ++y)
                {
                    for (int x = 0; x < grid[y].Length; ++x)
                    {
                        int depthValue = int.MaxValue;
                        foreach (Point location in GetAdjacant(x, y))
                        {
                            int value = GetGridValue(location);
                            if (value < depthValue)
                                depthValue = value;
                        }
                        if (grid[y][x] < depthValue)
                        {
                            lowSum += grid[y][x] + 1;
                            basins.Add(new Point(x, y), new List<Point>());
                            map.FillRect(new Rectangle(x * scale, y * scale, scale, scale), Color.Red);
                            Console.Write(grid[y][x]);
                        }
                        else
                        {
                            double progress = grid[y][x] / 9.0;
                            int brightness = Convert.ToInt32(progress * 0xFF);
                            map.FillRect(new Rectangle(x * scale, y * scale, scale, scale), Color.FromArgb(brightness, brightness, brightness));

                            brightness = Convert.ToInt32(progress * (gradient.Length - 1));
                            Console.Write(gradient[brightness]);
                        }
                    }
                    VisualFormHandler.Instance.Update((Image)map.Clone());
                    Console.WriteLine();
                }
                if (!part2)
                    return "Risk Level: " + lowSum;
                for (int i = 0; i < basins.Count; ++i)
                {
                    var basinCenter = basins.ElementAt(i);
                    List<Point> points = new List<Point>();
                    points.Add(basinCenter.Key);
                    for (int j = 0; j < points.Count; j++)
                        points.AddRange(GetAdjacant(points[j].X, points[j].Y).Except(points).Where(x => GetGridValue(x) < 9));
                    basins[basinCenter.Key] = points;
                }
                var largestBasins = basins.OrderByDescending(x => x.Value.Count()).Take(3);
                foreach (var basin in largestBasins)
                {
                    foreach (var point in basin.Value)
                    {
                        double progress = GetGridValue(point) / 9.0;
                        int brightness = Convert.ToInt32(progress * 0xFF);
                        map.FillRect(new Rectangle(point.X * scale, point.Y * scale, scale, scale), Color.FromArgb(0, brightness, 0));
                    }
                    VisualFormHandler.Instance.Update((Image)map.Clone());
                }


                int sizeValue = largestBasins.Select(x => x.Value.Count).Aggregate(MultiplyBasinSizes);
                return "The basin sizes combined make " + sizeValue;
            }
        }

        private int MultiplyBasinSizes(int current, int next)
        {
            if (current <= 0) return next;
            return current * next;
        }

        private IEnumerable<Point> GetAdjacant(int x, int y)
        {
            yield return new Point(x - 1, y);
            yield return new Point(x + 1, y);
            yield return new Point(x, y - 1);
            yield return new Point(x, y + 1);
        }

        private int GetGridValue(Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.Y >= grid.Length || location.X >= grid[location.Y].Length)
                return int.MaxValue;
            return grid[location.Y][location.X];
        }
    }
}
