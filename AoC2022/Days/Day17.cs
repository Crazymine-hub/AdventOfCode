using AdventOfCode.Days.Tools.Day17;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private List<ShapePlacement> placementList = new List<ShapePlacement>();
        private bool patternApplied = false;

        private long hiddenTowerHeight;

        public override string Solve(string input, bool part2)
        {
            _movement = input;
            FallRocks(part2);

            var towerHeight = GetTowerHeight() + hiddenTowerHeight;
            Console.WriteLine();
            return $"The Rock tower is {towerHeight} units tall";
        }

        private void FallRocks(bool part2)
        {
            int shapeIndex = 0;
            int jetIndex = 0;
            bool movedByJet = true;

            Point rockPosition;
            List<Point> currentShape;

            SpawnRock(ref shapeIndex, out rockPosition, out currentShape);
            long rockCount = 1;
            var maxRocks = part2 ? 1_000_000_000_000 : 2022;
            Regex expression = new Regex(@"(?<main>(?<v>;\d+)+?)(\k<main>)+", RegexOptions.Compiled);
            while (rockCount <= maxRocks)
            {
                var nextPosition = new Point(rockPosition.X, rockPosition.Y);
                if (movedByJet)
                {
                    bool moveLeft = _movement[jetIndex++] == '<';
                    if (jetIndex >= _movement.Length) jetIndex = 0;
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
                    SetGrid(currentShape, rockPosition, shapeIndex, jetIndex);
                    //HideLowerCave();
                    SpawnRock(ref shapeIndex, out rockPosition, out currentShape);
                    rockCount++;
                    rockCount += SearchPattern(maxRocks, rockCount);
                    movedByJet = false;
                }

                movedByJet = !movedByJet;
            }
        }

        private long SearchPattern(long maxBlocks, long fallenBlocks)
        {
            if (patternApplied) return 0;
            var endPlacement = placementList.Last();
            var scanIndex = placementList.Count - 1;
            var pattern = new List<ShapePlacement>() { endPlacement };
            int offset = 0;

            var startPoints = placementList.Where(x => x.PlacementCode == endPlacement.PlacementCode && x.ListIndex != endPlacement.ListIndex)
                .Select(x => placementList.IndexOf(x))
                .ToHashSet();
            if (!startPoints.Any()) return 0;

            while (scanIndex > 0)
            {
                scanIndex--;
                offset++;
                var scanItem = placementList[scanIndex];
                pattern.Add(scanItem);
                var points = placementList.Where(x => x.PlacementCode == scanItem.PlacementCode && x.JetIndex == scanItem.JetIndex && x.ListIndex != scanItem.ListIndex)
                    .Select(x => x.ListIndex)
                    .ToHashSet();
                var newPoints = startPoints.Where(x => points.Contains(x - offset)).ToHashSet();
                if (!newPoints.Any())
                {
                    if (startPoints.Count != 1 || offset <= 1 || startPoints.Single() != scanIndex) return 0;
                    offset--;
                    scanIndex++;
                    pattern.RemoveAt(pattern.Count - 1);
                    Console.WriteLine("PATTERN FOUND:");

                    var addedHeight = GetPatternHeight(pattern);
                    var addedBlocks = (long)pattern.Count;
                    var remainingBlocks = checked((long)(maxBlocks - fallenBlocks));
                    var stacks = remainingBlocks / addedBlocks;
                    var remainder = remainingBlocks % addedBlocks;
                    hiddenTowerHeight += addedHeight * stacks;
                    patternApplied = true;
                    return addedBlocks * stacks;
                }
                startPoints = newPoints;
            }
            return 0;
        }

        private long GetPatternHeight(List<ShapePlacement> pattern)
        {
            var beforePattern = placementList[placementList.Count - pattern.Count - 1];
            var beforeTop = beforePattern.Height + _shapes[beforePattern.ShapeIndex].Max(x => x.Y);

            var patternEnd = pattern.First();
            var patternTop = patternEnd.Height + _shapes[patternEnd.ShapeIndex].Max(x => x.Y);

            return patternTop - beforeTop;
        }

        private void GetShapeList()
        {
            using (var file = new StreamWriter(new FileStream(@"C:\Temp\Pattern.txt", FileMode.Create, FileAccess.ReadWrite)))
            {
                foreach (var item in placementList.Select(x => x.PlacementCode.ToString()).TakeWhile((x, i) => i < placementList.Count - 1))
                {
                    file.Write(item);
                    file.Write(", ");
                }
                file.Write(placementList.Last().PlacementCode.ToString());
            }
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

        private void SetGrid(List<Point> shape, Point shapePosition, int shapeIndex, int jetIndex)
        {
            placementList.Add(new ShapePlacement(shapePosition.Y + hiddenTowerHeight, shapeIndex, shapePosition.X, placementList.Count, jetIndex));
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
    }
}
