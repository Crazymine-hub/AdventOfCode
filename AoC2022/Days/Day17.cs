using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
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
        HashSet<Point> occupiedPoints = new HashSet<Point>();


        private VisualFormHandler form = VisualFormHandler.GetInstance();
        private const int PixelSize = 20;
        private readonly Brush BackgroundColor = Brushes.Black;
        private readonly Brush BorderColor = Brushes.White;
        private readonly Brush FixedStoneColor = Brushes.Gray;
        private readonly Brush MovableStoneColor = Brushes.Lime;
        private readonly Brush GridColor = Brushes.DarkBlue;
        Bitmap image;
        Graphics g;
        private int imageTowerHeight;

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            _movement = input;

            form.Show();
            Console.ReadLine();


            int shapeIndex = 0;
            int sidewaysIndex = 0;
            Point rockPosition = Point.Empty;
            List<Point> currentShape = null;
            bool movedByJet = true;
            SpawnRock(ref shapeIndex, out rockPosition, out currentShape);
            RenderCaveSystem();
            int rockCount = 1;
            while (rockCount <= 2022)
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
                    SpawnRock(ref shapeIndex, out rockPosition, out currentShape);
                    rockCount++;
                    movedByJet = false;
                    RenderCaveSystem();
                }

                movedByJet = !movedByJet;
            }

            var towerHeight = occupiedPoints.Max(x => x.Y) + 1;
            return $"The Rock tower is {towerHeight} units tall";
        }

        private void SpawnRock(ref int shapeIndex, out Point rockPosition, out List<Point> currentShape)
        {
            //spawn next rock
            var top = 0;
            if (occupiedPoints.Any())
                top = occupiedPoints.Max(x => x.Y) + 1;
            top += 3;
            currentShape = _shapes[shapeIndex++];
            if (shapeIndex >= _shapes.Count) shapeIndex = 0;
            rockPosition = new Point(Math.Abs(currentShape.Min(x => x.X)) + 2, Math.Abs(currentShape.Min(x => x.Y)) + top);
        }

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
                if (targetX < 0 || targetX >= CaveWidth || targetY < 0 || occupiedPoints.Contains(new Point(targetX, targetY))) return false;
            }
            return true;
        }

        private void RenderCaveSystem()
        {
            if(image == null || occupiedPoints.Max(x => x.Y) + 1 >= imageTowerHeight)
                CreateNewImage();
            g.FillRectangle(BackgroundColor, 0, 0, image.Width, image.Height);
            g.FillRectangle(BorderColor, 0, 0, PixelSize, image.Height);
            g.FillRectangle(BorderColor, image.Width - PixelSize, 0, PixelSize, image.Height);
            g.FillRectangle(BorderColor, 0, image.Height - PixelSize, image.Width, image.Height);

            //var topPixel = int.MaxValue;
            foreach (var point in occupiedPoints)
            {
                var y = (imageTowerHeight - point.Y - 1) * PixelSize;
                g.FillRectangle(FixedStoneColor, (point.X + 1) * PixelSize, y, PixelSize, PixelSize);
                //if(y < topPixel)
                //    topPixel = y;
            }
            form.Update(image, true);
            //form.SetFocusTo(0, topPixel);
        }

        private void CreateNewImage()
        {
            g?.Dispose();
            image?.Dispose();
            if (occupiedPoints.Any())
                imageTowerHeight = occupiedPoints.Max(x => x.Y) + 1;
            imageTowerHeight += 1000;
            image = new Bitmap((CaveWidth + 2) * PixelSize, (imageTowerHeight + 1) * PixelSize);
            g = Graphics.FromImage(image);
        }
    }
}
