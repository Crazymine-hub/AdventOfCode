using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day19
{
    public struct Point3: ICloneable, IEqualityComparer<Point3>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public object Clone() => CloneDirect();
        public Point3 CloneDirect() => new Point3(X, Y, Z);

        public Point3 GetRotatedPoint(int rotation)
        {
            if (rotation < 0 || rotation > 23) throw new ArgumentException($"The value {rotation} is outside the Range {{0;23}}.", nameof(rotation));
            Point3 newPoint = this.CloneDirect();
            int steps = rotation / 4;
            if (steps < 4)
            {
                //X-Rotation
                for (int i = 0; i < steps; ++i)
                {
                    int buf = newPoint.Z;
                    newPoint.Z = newPoint.Y;
                    newPoint.Y = buf;

                    if (this.Z < 0)
                        newPoint.Y = Math.Abs(newPoint.Y);
                    else
                        newPoint.Y = Math.Abs(newPoint.Y) * -1;

                    if (this.Y < 0)
                        newPoint.Z = Math.Abs(newPoint.Z) * -1;
                    else
                        newPoint.Z = Math.Abs(newPoint.Z);
                }
            }
            else
            {
                //Y-Rotation
                int buf = newPoint.X;
                newPoint.X = newPoint.Z;
                newPoint.Z = buf;
                if (steps == 4)
                {
                    if (this.X < 0)
                        newPoint.Z = Math.Abs(newPoint.Z);
                    else
                        newPoint.Z = Math.Abs(newPoint.Z) * -1;

                    if (this.Z < 0)
                        newPoint.X = Math.Abs(newPoint.X) * -1;
                    else
                        newPoint.X = Math.Abs(newPoint.X);
                }
            }

            Point3 unrotated = newPoint;
            newPoint = unrotated.CloneDirect();

            //Z Rotation
            for (int i = 0; i < rotation % 4; ++i)
            {
                int buf = newPoint.X;
                newPoint.X = newPoint.Y;
                newPoint.Y = buf;

                if (unrotated.X < 0)
                    newPoint.Y = Math.Abs(newPoint.Y);
                else
                    newPoint.Y = Math.Abs(newPoint.Y) * -1;

                if (unrotated.Y < 0)
                    newPoint.X = Math.Abs(newPoint.X) * -1;
                else
                    newPoint.X = Math.Abs(newPoint.X);
            }

            return newPoint;
        }


        public override string ToString() => $"Point3: {{{X},{Y},{Z}}}";

        public bool Equals(Point3 a, Point3 b) => (a.X == b.X && a.Y == b.Y && a.Z == b.Z);

        public int GetHashCode(Point3 obj) => X ^ Y ^ Z;

        public static Point3 operator -(Point3 a, Point3 b) => new Point3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Point3 operator +(Point3 a, Point3 b) => new Point3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    }
}
