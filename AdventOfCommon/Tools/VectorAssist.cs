using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AdventOfCode.Tools
{
    public static class VectorAssist
    {
        public static double GetLength(Point vector)
        {
            return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2));
        }

        public static Point Minimize(Point distance)
        {
            int divisor = (int)MathHelper.GreatestCommonDivisor(distance.X, distance.Y);
            return new Point(distance.X / divisor, distance.Y / divisor);
        }

        public static double GetAngleBetween(Point vec1, Point vec2)
        {
            return Math.Acos(GetCrossProduct(vec1, vec2) / (GetLength(vec1) * GetLength(vec2))) * 180 / Math.PI;
        }

        public static int GetCrossProduct(Point vec1, Point vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }

        public static Point PointDifference(Point start, Point end)
        {
            return new Point(end.X - start.X, end.Y - start.Y);
        }

        public static int ManhattanDistance(Point start, Point end)
        {
            Point offset = PointDifference(start, end);
            return Math.Abs(offset.X) + Math.Abs(offset.Y);
        }

        public static Point[] GetPointsFromLine(Point start, Point end)
        {
            if (start.X > end.X)
            {
                Point swapBuf = start;
                start = end;
                end = swapBuf;
            }

            Point vector = PointDifference(start, end);

            List<Point> points = new List<Point>();
            Point lastPoint = start;

            for (int x = start.X; x <= end.X; ++x)
            {
                double progress = ((double)x - start.X + 1) / (vector.X + 1);
                int y = (int)Math.Round(Math.Abs(vector.Y) * progress) + Math.Min(start.Y, end.Y);
                Point target = new Point(x, y);
                if (target != lastPoint)
                    points.Add(lastPoint);
                for (int top = Math.Min(lastPoint.Y, target.Y) + 1; top < Math.Max(lastPoint.Y, target.Y); ++top)
                    points.Add(new Point(lastPoint.X, top));
                lastPoint = target;
            }
            points.Add(lastPoint);
            return points.ToArray();
        }
    }
}
