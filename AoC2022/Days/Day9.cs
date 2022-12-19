using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Graphics;
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
    public class Day9 : DayBase
    {
        public override string Title => "Rope Bridge";
        private RenderMode _renderMode = RenderMode.Image;

        public override string Solve(string input, bool part2)
        {
            DynamicGrid<bool> ropeGrid = new DynamicGrid<bool>();
            Point3 head = new Point3(0, 0, 0);
            Point3[] tails = new Point3[part2 ? 9 : 1];
            ropeGrid.SetRelative(0, 0, true);
            var visForm = VisualFormHandler.GetInstance();
            visForm.Show();
            foreach (var movement in GetLines(input))
            {
                Point3 direction;
                switch (movement.First())
                {
                    case 'U':
                        direction = new Point3(0, -1, 0);
                        break;
                    case 'D':
                        direction = new Point3(0, 1, 0);
                        break;
                    case 'L':
                        direction = new Point3(-1, 0, 0);
                        break;
                    case 'R':
                        direction = new Point3(1, 0, 0);
                        break;
                    default: throw new InvalidOperationException($"Unknown movement direction '{movement.First()}'");
                }
                int repetitions = int.Parse(movement.Substring(2));

                for (int repetition = 0; repetition < repetitions; repetition++)
                {
                    head = head + direction;

                    for (int i = 0; i < tails.Length; ++i)
                        tails[i] = ApplyMovement(i == 0 ? head : tails[i - 1], tails[i]);
                    ropeGrid.SetRelative(tails.Last().X, tails.Last().Y, true);
                    head = RenderGrid(ropeGrid, head, tails, visForm, _renderMode);
                }
            }

            return $"Visited {ropeGrid.Count(x => x.Value)} locations";
        }

        private static Point3 RenderGrid(DynamicGrid<bool> ropeGrid, Point3 head, Point3[] tails, VisualFormHandler visForm, RenderMode renderMode)
        {
            ropeGrid.MakeAvaliable(head.X + ropeGrid.XOrigin, head.Y + ropeGrid.YOrigin);
            var pixelSize = 5;
            if (renderMode.HasFlag(RenderMode.Image))
            {
                var img = new Bitmap(ropeGrid.XDim * pixelSize, ropeGrid.YDim * pixelSize);
                foreach (var point in ropeGrid)
                {
                    if (point.Z != 0) continue;
                    bool hasTail = false;
                    for (int i = 0; i < tails.Length; ++i)
                    {
                        if (point.X == tails[i].X + ropeGrid.XOrigin && point.Y == tails[i].Y + ropeGrid.YOrigin)
                        {
                            img.FillRect(new Rectangle(point.X * pixelSize, point.Y * pixelSize, pixelSize, pixelSize), Color.Red);
                            hasTail = true;
                        }
                    }
                    if (hasTail) continue;
                    if (point.X == head.X + ropeGrid.XOrigin && point.Y == head.Y + ropeGrid.YOrigin)
                    {
                        img.FillRect(new Rectangle(point.X * pixelSize, point.Y * pixelSize, pixelSize, pixelSize), Color.Green);
                        continue;
                    }

                    if (point.Value)
                    {
                        img.FillRect(new Rectangle(point.X * pixelSize, point.Y * pixelSize, pixelSize, pixelSize), Color.Orange);
                        continue;
                    }
                    img.FillRect(new Rectangle(point.X * pixelSize, point.Y * pixelSize, pixelSize, pixelSize), Color.DarkGray);
                }
                visForm.Update(img, false);
            }

            if (renderMode.HasFlag(RenderMode.Console))
            {
                Console.Clear();
                Console.WriteLine(ropeGrid.GetStringRepresentation((value, x, y) =>
                {
                    bool hasTail = false;
                    for (int i = 0; i < tails.Length; ++i)
                        if (x == tails[i].X + ropeGrid.XOrigin && y == tails[i].Y + ropeGrid.YOrigin)
                            return i.ToString();

                    if (x == head.X + ropeGrid.XOrigin && y == head.Y + ropeGrid.YOrigin) return "H";
                    if (value) return "█";
                    return "▫";
                }));
                Console.WriteLine();
                Console.WriteLine();
            }
            return head;
        }

        private static Point3 ApplyMovement(Point3 head, Point3 tail)
        {
            var distance = head - tail;
            if (Math.Abs(distance.X) > 1)
            {
                tail = tail + new Point3(distance.GetSignInfo().X, 0, 0);
                if (Math.Abs(distance.Y) > 0)
                {
                    tail = tail + new Point3(0, distance.GetSignInfo().Y, 0);
                    return tail;
                }
            }
            if (Math.Abs(distance.Y) > 1)
            {
                tail = tail + new Point3(0, distance.GetSignInfo().Y, 0);
                if (Math.Abs(distance.X) > 0)
                    tail = tail + new Point3(distance.GetSignInfo().X, 0, 0);
            }
            return tail;
        }
    }
}
