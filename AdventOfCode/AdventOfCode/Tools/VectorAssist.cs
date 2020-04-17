using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AdventOfCode.Tools
{
    static class VectorAssist
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
            return Math.Acos(GetCrossProduct(vec1, vec2) / (GetLength(vec1) * GetLength(vec2))) * 180 /Math.PI;
        }

        public static int GetCrossProduct(Point vec1, Point vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }
    }
}
