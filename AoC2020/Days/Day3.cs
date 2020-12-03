using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day3 : DayBase
    {
        public override string Title => "Toboggan Trajectory";
        private string[] map;
        private Point position = new Point();
        Dictionary<Point, int> slopes = new Dictionary<Point, int>();

        public override string Solve(string input, bool part2)
        {
            map = GetLines(input).ToArray();
            Console.Write(input);

            slopes.Add(new Point(1, 1), 0);
            slopes.Add(new Point(3, 1), 0);
            slopes.Add(new Point(5, 1), 0);
            slopes.Add(new Point(7, 1), 0);
            slopes.Add(new Point(1, 2), 0);

            int minLine = -1;
            long product = 1;
            for (int i = 0; i < slopes.Count; i++)
            {
                var slope = slopes.ElementAt(i);
                slopes[slope.Key] = TestSlope(slope.Key.X, slope.Key.Y);
                slope = slopes.ElementAt(i);
                Console.SetCursorPosition(0, map.Length + i);
                Console.Write($"  R:{slope.Key.X} D:{slope.Key.Y} -> {slope.Value}");
                if (minLine == -1 || slope.Value < slopes.ElementAt(minLine).Value)
                    minLine = i;
                product *= slope.Value;
            }
            Console.SetCursorPosition(0, map.Length + minLine);
            Console.Write("*");
            Console.SetCursorPosition(0, map.Length + slopes.Count);
            return $"Product of Trees: {product}";
        }

        private int TestSlope(int dx, int dy)
        {
            char result;
            int trees = 0;
            position.X = 0;
            position.Y = 0;
            do
            {
                result = Move(dx, dy);
                Console.SetCursorPosition(position.X, position.Y);
                if (result == '#')
                {
                    Console.Write('x');
                    trees++;
                }
                else Console.Write('O');

            } while (result != '\0');
            return trees;
        }

        private char Move(int dx, int dy)
        {
            position.Offset(dx, dy);
            if (position.Y >= map.Length) return '\0';
            if (position.X >= map[0].Length) position.Offset(-map[0].Length, 0);
            return map[position.Y][position.X];
        }
    }
}