using AdventOfCode.Tools.Graphics;
using AdventOfCode.Tools.SpecificBitwise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day22
{
    public class CubeArea
    {
        public Point3 Start { get; }
        public Point3 End { get; }
        public Point3 Direction => End - Start;

        public CubeArea(Point3 start, Point3 end)
        {
            (Start, End) = Normailze(start, end);
        }

        public bool Contains(Point3 point) =>
            point.X >= Start.X && point.X <= End.X &&
            point.Y >= Start.Y && point.Y <= End.Y &&
            point.Z >= Start.Z && point.Z <= End.Z;

        public List<CubeArea> Substract(CubeArea cuboid)
        {
            List<CubeArea> result = new List<CubeArea>();

            CubeArea substractionArea = GetIntersectingCube(cuboid);

            if (substractionArea == null)
            {
                result.Add(this);
                return result;
            }

                
            result.AddRange(SubstractCorner(substractionArea, this, false, false));
            result.AddRange(SubstractCorner(substractionArea, this, true, result.Any()));

            return result;
        }

        private static List<CubeArea> SubstractCorner(CubeArea substractionCuboid, CubeArea fullCuboid, bool reverse, bool selfLimit)
        {
            var closeCorner = reverse ? fullCuboid.End : fullCuboid.Start;
            var farCorner = reverse ? fullCuboid.Start : fullCuboid.End;
            var substractionCorner = reverse ? substractionCuboid.End : substractionCuboid.Start;
            var limitCorner = reverse ? substractionCuboid.Start : substractionCuboid.End;
            List<CubeArea> result = new List<CubeArea>();
            if (substractionCorner != closeCorner)
            {
                var newCorners = GetAllCorners(closeCorner, farCorner).ToList();
                var freeOffset = (closeCorner - substractionCorner).GetSignInfo();
                var remainderAnchor = substractionCorner + freeOffset;
                foreach (Point3 corner in newCorners)
                {
                    if (corner == farCorner) continue;
                    var movingOffset = (corner - closeCorner).GetSignInfo();
                    if (!IsAvailable(movingOffset, freeOffset)) continue;
                    var newStart = remainderAnchor + movingOffset;
                    var newEnd = corner;
                    if (selfLimit)
                        newEnd = new Point3(Math.Max(limitCorner.X, newEnd.X), Math.Max(limitCorner.Y, newEnd.Y), Math.Max(limitCorner.Z, newEnd.Z));
                    if (AreSidesSwapped(movingOffset, (newEnd - newStart).GetSignInfo())) continue;
                    result.Add(new CubeArea(newStart, newEnd));
                }
            }
            return result;
        }

        private static bool AreSidesSwapped(Point3 oldDirection, Point3 newDirection) =>
            !((oldDirection.X == 0 || oldDirection.X == newDirection.X || newDirection.X == 0) &&
            (oldDirection.Y == 0 || oldDirection.Y == newDirection.Y || newDirection.Y == 0) &&
            (oldDirection.Z == 0 || oldDirection.Z == newDirection.Z || newDirection.Z == 0));

        private static bool IsAvailable(Point3 movingAxes, Point3 freeAxes)
        {
            int movingMask = IntBitwise.GetValue((new int[] { movingAxes.X, movingAxes.Y, movingAxes.Z }).Select(x => x != 0));
            int freeMask = IntBitwise.GetValue((new int[] { freeAxes.X, freeAxes.Y, freeAxes.Z }).Select(x => x != 0));
            return (~movingMask & freeMask) != 0;
        }

        public static IEnumerable<Point3> GetAllCorners(Point3 start, Point3 end)
        {
            var changes = end - start;
            yield return start;
            if (changes.X != 0)
            {
                yield return new Point3(end.X, start.Y, start.Z);
                if (changes.Y != 0)
                    yield return new Point3(end.X, end.Y, start.Z);
            }
            if (changes.Y != 0)
                yield return new Point3(start.X, end.Y, start.Z);
            if (changes.Z != 0)
            {
                yield return new Point3(start.X, start.Y, end.Z);
                if (changes.X != 0)
                {
                    yield return new Point3(end.X, start.Y, end.Z);
                    if (changes.Y != 0)
                        yield return end;
                }
                if (changes.Y != 0)
                    yield return new Point3(start.X, end.Y, end.Z);
            }
        }

        public long Volume()
        {
            var sideLengths = End - Start;
            return Math.Abs(sideLengths.X + 1L) * Math.Abs(sideLengths.Y + 1L) * Math.Abs(sideLengths.Z + 1L);
        }

        public override string ToString() => $"Cuboid {Start} {End}";

        public CubeArea GetIntersectingCube(CubeArea cuboid)
        {
            Point3 subStart = cuboid.Start;
            Point3 subEnd = cuboid.End;

            if (subStart.X < Start.X)
                subStart.X = Start.X;
            if (subStart.Y < Start.Y)
                subStart.Y = Start.Y;
            if (subStart.Z < Start.Z)
                subStart.Z = Start.Z;

            var innerDirection = cuboid.End - subStart;
            if (innerDirection.X < 0 || innerDirection.Y < 0 || innerDirection.Z < 0)
                return null;

            if (subEnd.X > End.X)
                subEnd.X = End.X;
            if (subEnd.Y > End.Y)
                subEnd.Y = End.Y;
            if (subEnd.Z > End.Z)
                subEnd.Z = End.Z;

            innerDirection = subEnd - cuboid.Start;
            if (innerDirection.X < 0 || innerDirection.Y < 0 || innerDirection.Z < 0)
                return null;
            return new CubeArea(subStart, subEnd);
        }

        private static (Point3 newStart, Point3 newEnd) Normailze(Point3 start, Point3 end)
        {
            var direction = end - start;
            if (direction.X < 0)
                (start.X, end.X) = (end.X, start.X);
            if (direction.Y < 0)
                (start.Y, end.Y) = (end.Y, start.Y);
            if (direction.Z < 0)
                (start.Z, end.Z) = (end.Z, start.Z);
            return (start, end);
        }
    }
}
