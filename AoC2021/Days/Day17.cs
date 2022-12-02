using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Days.Tools.Day17;
using System.Threading;
using AdventOfCode.Tools.Visualization;

namespace AdventOfCode.Days
{
    internal class Day17 : DayBase, IDisposable
    {
        public override string Title => "Trick Shot";
        Point TargetTL;
        Point TargetBR;
        Rectangle drawArea;
        const int cutoffDistance = 20;
        int topOffset = 200;
        const int scale = 5;
        private bool wasDisposed;
        Bitmap throwMap;
        private bool part2;
        readonly VisualFormHandler formHandler = VisualFormHandler.GetInstance();


        public override string Solve(string input, bool part2)
        {
            this.part2 = part2;
            var targetDefinition = Regex.Match(input, @"^target area: x=(\d+)\.\.(\d+), y=(-?\d+)\.\.(-\d+)$");
            if (!targetDefinition.Success) throw new ArgumentException("The definition for the target area doesn't have the right format.", nameof(input));

            int x1 = int.Parse(targetDefinition.Groups[1].Value);
            int y1 = int.Parse(targetDefinition.Groups[3].Value);
            int x2 = int.Parse(targetDefinition.Groups[2].Value);
            int y2 = int.Parse(targetDefinition.Groups[4].Value);


            TargetTL = new Point(Math.Min(x1, x2), Math.Max(y1, y2));
            TargetBR = new Point(Math.Max(x1, x2), Math.Min(y1, y2));

            drawArea = new Rectangle(0, 0, TargetBR.X + cutoffDistance, topOffset + Math.Abs(TargetBR.Y) + cutoffDistance);
            throwMap = new Bitmap(drawArea.Size.Width * scale, drawArea.Size.Height * scale);


            throwMap.FillRect(new Rectangle(0, 0, throwMap.Width, throwMap.Height), Color.Black);
            throwMap.FillRect(new Rectangle(0, topOffset * scale, scale, scale * 2), Color.Green);
            throwMap.FillRect(new Rectangle(TargetTL.X * scale,
                                            (topOffset - TargetTL.Y + 1) * scale,
                                            Math.Abs(TargetBR.X - TargetTL.X) * scale,
                                            Math.Abs(TargetBR.Y - TargetTL.Y) * scale), Color.Green);
            formHandler.Show(throwMap);
            var successfulAttempts = AttemptThrows(out int maxHeight).SelectMany(x => x).Count(x => x.TargetHit);

            if (part2)
                return $"There are {successfulAttempts} possibilities to hit the target.";
            return $"The Best throw has a height of {maxHeight}, You show off...";
        }

        public List<List<ThrowResult>> AttemptThrows(out int maxHeight)
        {
            maxHeight = int.MinValue;
            List<List<ThrowResult>> attempts = new List<List<ThrowResult>>();
            for (int y = part2 ? -Math.Abs(TargetBR.Y) : 1; y <= Math.Abs(TargetBR.Y); ++y)
            {
                List<ThrowResult> throws = AttemptThrows(y);
                attempts.Add(throws);
                int throwHeight = throws.Max(x => x.MaxHeight);
                if (throws.Any(x => x.TargetHit) && maxHeight < throwHeight)
                    maxHeight = throwHeight;
            }
            return attempts;
        }


        public List<ThrowResult> AttemptThrows(int yVelocity)
        {
            bool furtherAttempts = true;
            List<ThrowResult> result = new List<ThrowResult>();
            for (int x = 1; furtherAttempts; ++x)
            {
                using (Bitmap throwImage = (Bitmap)throwMap.Clone())
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    ThrowResult throwResult = SimulateThrow(x, yVelocity, throwImage);
                    result.Add(throwResult);
                    furtherAttempts = !throwResult.Overshot;
                    formHandler.Update(throwImage);
                }
            }
            return result;
        }

        public ThrowResult SimulateThrow(int xVelocity, int yVelocity, Bitmap throwImage)
        {
            bool overshot = false;
            bool targetHit = false;
            int maxHeight = 0;
            Point position = new Point();
            List<Point> points = new List<Point>() { position };
            bool firstPoint = true;
            while (true)
            {
                position.Offset(xVelocity, yVelocity);
                points.Add(position);

                if (position.Y > maxHeight)
                    maxHeight = position.Y;

                if (xVelocity > 0) --xVelocity;
                --yVelocity;

                if (position.Y < TargetBR.Y - cutoffDistance || position.X < 0) break;
                if (position.X > TargetBR.X + cutoffDistance)
                {
                    overshot = position.Y > TargetTL.Y || firstPoint;
                    break;
                }

                if (TargetTL.X <= position.X && position.X <= TargetBR.X && TargetTL.Y >= position.Y && position.Y >= TargetBR.Y)
                {
                    targetHit = true;
                    DrawPoint(position, true, throwImage);
                    break;
                }
                DrawPoint(position, false, throwImage);
                firstPoint = false;
            }

            return new ThrowResult(points, maxHeight, targetHit, overshot);
        }

        private void DrawPoint(Point position, bool hit, Bitmap throwImage)
        {
            if (!drawArea.Contains(position.X, topOffset - position.Y) && !(position.X == 0 && position.Y == 0)) return;
            throwImage.FillRect(new Rectangle(position.X * scale, (topOffset - position.Y) * scale, scale, scale), hit ? Color.Orange : Color.Blue);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!wasDisposed)
            {
                if (disposing)
                {
                    throwMap?.Dispose();
                    formHandler.Dispose();
                }
                wasDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
