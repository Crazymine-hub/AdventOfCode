using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Graphics
{
    [DebuggerDisplay("Point3 ({X}, {Y}, {Z})")]
    public struct Point3 : ICloneable, IEquatable<Point3>
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


        public Point3 Rotate(int rotation)
        {
            if (rotation < 0 || rotation > 24) throw new ArgumentException($"The value {rotation} is outside the Range {{0;23}}.", nameof(rotation));
            Point3 newPoint = CloneDirect();
            int steps = rotation / 4;

            Point3 unrotated = newPoint;
            newPoint = unrotated.CloneDirect();

            if (steps < 4)
            {
                //X-Rotation
                for (int i = 0; i < steps; ++i)
                {
                    int buf = newPoint.Z;
                    newPoint.Z = newPoint.Y;
                    newPoint.Y = buf;

                    if (unrotated.Z < 0)
                        newPoint.Y = Math.Abs(newPoint.Y);
                    else
                        newPoint.Y = Math.Abs(newPoint.Y) * -1;

                    if (unrotated.Y < 0)
                        newPoint.Z = Math.Abs(newPoint.Z) * -1;
                    else
                        newPoint.Z = Math.Abs(newPoint.Z);

                    unrotated = newPoint;
                    newPoint = unrotated.CloneDirect();
                }
            }
            else
            {
                //Y-Rotation
                int buf = newPoint.X;
                newPoint.X = newPoint.Z;
                newPoint.Z = buf;
                if (steps == 4)
                { //right round
                    if (unrotated.X < 0)
                        newPoint.Z = Math.Abs(newPoint.Z);
                    else
                        newPoint.Z = Math.Abs(newPoint.Z) * -1;

                    if (unrotated.Z < 0)
                        newPoint.X = Math.Abs(newPoint.X) * -1;
                    else
                        newPoint.X = Math.Abs(newPoint.X);
                }
                else
                {
                    //left round
                    if (unrotated.X < 0)
                        newPoint.Z = Math.Abs(newPoint.Z) * -1;
                    else
                        newPoint.Z = Math.Abs(newPoint.Z);

                    if (unrotated.Z < 0)
                        newPoint.X = Math.Abs(newPoint.X);
                    else
                        newPoint.X = Math.Abs(newPoint.X) * -1;
                }
            }


            unrotated = newPoint;
            newPoint = unrotated.CloneDirect();

            //Z Rotation
            for (int i = 0; i < rotation % 4; ++i)
            {
                int buf = newPoint.X;
                newPoint.X = newPoint.Y;
                newPoint.Y = buf;

                if (unrotated.X < 0)
                    newPoint.Y = Math.Abs(newPoint.Y) * -1;
                else
                    newPoint.Y = Math.Abs(newPoint.Y);

                if (unrotated.Y < 0)
                    newPoint.X = Math.Abs(newPoint.X);
                else
                    newPoint.X = Math.Abs(newPoint.X) * -1;

                unrotated = newPoint;
                newPoint = unrotated.CloneDirect();
            }

            return newPoint;
        }

        public Point3 Invert() => new Point3(-X, -Y, -Z);

        public override string ToString() => $"Point3: {{{X},{Y},{Z}}}";

        public static string GetStanfordPly(IEnumerable<Point3> points, string comment = "")
        {
            StringBuilder plyFile = new StringBuilder();
            plyFile.AppendLine("ply");
            plyFile.AppendLine("format ascii 1.0");
            foreach (string commentLine in comment.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                plyFile.AppendLine($"comment {commentLine}");
            plyFile.AppendLine($"element vertex {points.Count()}");
            plyFile.AppendLine("property float x");
            plyFile.AppendLine("property float y");
            plyFile.AppendLine("property float z");
            plyFile.AppendLine("end_header");
            foreach (var point in points)
            {
                plyFile.Append($"{(point.X / 100.0).ToString(System.Globalization.NumberFormatInfo.InvariantInfo)} ");
                plyFile.Append($"{(point.Y / 100.0).ToString(System.Globalization.NumberFormatInfo.InvariantInfo)} ");
                plyFile.Append($"{(point.Z / 100.0).ToString(System.Globalization.NumberFormatInfo.InvariantInfo)}");
                plyFile.AppendLine();
            }
            return plyFile.ToString();
        }

        public static int ManhattanDistance(Point3 a, Point3 b)
        {
            var vector = b - a;
            return Math.Abs(vector.X) + Math.Abs(vector.Y) + Math.Abs(vector.Z);
        }

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Point3 && Equals((Point3)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }


        public static Point3 operator -(Point3 a, Point3 b) => new Point3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Point3 operator +(Point3 a, Point3 b) => new Point3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static bool operator ==(Point3 a, Point3 b) => a.Equals(b);
        public static bool operator !=(Point3 a, Point3 b) => !a.Equals(b);

        #region Assist Functions
        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        public Point3 Minimize()
        {
            int divisor = (int)MathHelper.GreatestCommonDivisor(X, Y, Z);
            return new Point3(X / divisor, Y / divisor, Z / divisor);
        }

        public Point3 GetSignInfo()
        {
            return new Point3(Math.Sign(X), Math.Sign(Y), Math.Sign(Z));
        }

        public double GetAngleBetween(Point3 vec2)
        {
            return Math.Acos(GetCrossProduct(vec2) / (GetLength() * vec2.GetLength())) * 180 / Math.PI;
        }

        public int GetCrossProduct(Point3 vec2)
        {
            return X * vec2.X + Y * vec2.Y + Z * vec2.Z;
        }
        #endregion
    }
}
