using AdventOfCode.Tools;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day17: DayBase
    {
        public override string Title => "Pyroclastic Flow";
        private string _movement;
        private List<List<Point>> _shapes = new List<List<Point>>
        {
            // - Shape
            new List<Point>
            {
                new Point(0, 0),     //Left
                new Point(1, 0),
                new Point(2, 0),
                new Point(3, 0),
            },
            // + Shape
            new List<Point> {
                new Point(0,0),     //center
                new Point(1,0),     //Right arm
                new Point(-1,0),    //Left arm
                new Point(0,1),     //Bottom arm
                new Point(0,-1),    //Top arm
            },
            // ┘ Shape
            new List<Point>
            {
                new Point(0, 0),     //Bottom right corner
                new Point(-1, 0),   //Left arm
                new Point(-2, 0),
                new Point(0, 1),   //Top arm
                new Point(0, 2),
            },
            // │ Shape
            new List<Point>
            {
                new Point(0, 0),     //Top
                new Point(0, 1),
                new Point(0, 2),
                new Point(0, 3),
            },
            // ■ Shape
            new List<Point>
            {
                new Point(0, 0),
                new Point(1, 0),
                new Point(0, 1),
                new Point(1, 1),
            }
        };
        private const int CaveWidth = 7;
        private HashSet<Point> occupiedPoints = new HashSet<Point>();
        private ConsoleAssist ConsoleAssist = new ConsoleAssist();

        private long hiddenTowerHeight;

        public override string Solve(string input, bool part2)
        {
            _movement = input;
            int shapeIndex = 0;
            int sidewaysIndex = 0;
            Point rockPosition = Point.Empty;
            List<Point> currentShape = null;
            bool movedByJet = true;
            SpawnRock(ref shapeIndex, out rockPosition, out currentShape);
            uint rockCount = 1;
            var maxRocks = part2 ? 1_000_000_000_000 : 2022;
            decimal progress = -1;
            while (rockCount <= maxRocks)
            {
                var nextPosition = new Point(rockPosition.X, rockPosition.Y);
                if (movedByJet)
                {
                    bool moveLeft = _movement[sidewaysIndex++] == '<';
                    if (sidewaysIndex >= _movement.Length) sidewaysIndex = 0;
                    nextPosition.Offset(moveLeft ? -1 : 1, 0);
                }
                else
                {
                    nextPosition.Offset(0, -1);
                }

                if (CanPlaceShape(currentShape, nextPosition))
                {
                    rockPosition = nextPosition;
                }
                else if (!movedByJet)
                {
                    SetGrid(currentShape, rockPosition);
                    HideLowerCave();
                    //RenderCaveSystem();
                    SpawnRock(ref shapeIndex, out rockPosition, out currentShape);
                    var newProgress = Math.Round(rockCount * 100 / (decimal)maxRocks, 3);
                    if(newProgress > progress)
                    {
                        progress = newProgress;
                        Console.CursorLeft = 0;
                        Console.Write(progress.ToString("0.000"));
                        Console.Write("% ");
                        Console.Write(ConsoleAssist.GetNextProgressChar());
                    }
                    rockCount++;
                    movedByJet = false;
                }

                movedByJet = !movedByJet;
            }

            var towerHeight = GetTowerHeight() + hiddenTowerHeight;
            Console.WriteLine();
            return $"The Rock tower is {towerHeight} units tall";
        }

        private void SpawnRock(ref int shapeIndex, out Point rockPosition, out List<Point> currentShape)
        {
            //spawn next rock
            var top = 0;
            if (occupiedPoints.Any())
                top = GetTowerHeight();
            top += 3;
            currentShape = _shapes[shapeIndex++];
            if (shapeIndex >= _shapes.Count) shapeIndex = 0;
            rockPosition = new Point(Math.Abs(currentShape.Min(x => x.X)) + 2, Math.Abs(currentShape.Min(x => x.Y)) + top);
        }

        private int GetTowerHeight() => occupiedPoints.Max(x => x.Y) + 1;

        private void SetGrid(List<Point> shape, Point shapePosition)
        {
            foreach (var point in shape)
                occupiedPoints.Add(new Point(shapePosition.X + point.X, shapePosition.Y + point.Y));
        }

        private bool CanPlaceShape(List<Point> shape, Point shapePosition)
        {
            foreach (var point in shape)
            {
                var targetX = shapePosition.X + point.X;
                var targetY = shapePosition.Y + point.Y;
                if (targetY < 0) ;
                if (targetX < 0 || targetX >= CaveWidth || targetY < 0 || occupiedPoints.Contains(new Point(targetX, targetY))) return false;
            }
            return true;
        }

        private void HideLowerCave()
        {
            var offset = occupiedPoints.GroupBy(x => x.X).Select(x => new Point(x.Key, x.Max(p => p.Y))).Min(x => x.Y);
            if (offset == 0) return;
            //var oldHeight = GetTowerHeight() + hiddenTowerHeight;
            //Console.WriteLine($"Height: {oldHeight}");
            //var toRemove = occupiedPoints.Where(p => p.Y < hiddenHeightStep).ToHashSet();
            //allPoints = allPoints.Concat(toRemove.Select(p => new Point(p.X, p.Y + (int)hiddenTowerHeight))).ToHashSet();

            occupiedPoints.RemoveWhere(x => x.Y < offset);
            occupiedPoints = occupiedPoints.Select(x => new Point(x.X, x.Y - offset)).ToHashSet();
            hiddenTowerHeight += offset;
        }
    }
}
