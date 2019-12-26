using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day3 : IDay
    {                                    //0    1    2    3    4    5    6    7    8    9
        private readonly char[] wires = { ' ', '│', '─', '┘', '└', '┐', '┌', '╫', '╪' };

        public string Solve(string input, bool part2)
        {
            List<int[][]> result = new List<int[][]>();
            foreach (string grid in input.Split('\n'))
            {
                result.Add(TraceWire(grid.Split(',')));
            }
            int[][] result1 = result[0];
            int[][] result2 = result[1];

            //int[][] result1 = TraceWire("R75, D30, R83, U83, L12, D49, R71, U7, L72".Split(','));
            //int[][] result2 = TraceWire("U62, R66, U55, R34, D71, R55, D58, R83".Split(','));

            //int[][] result1 = TraceWire("R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51".Split(','));
            //int[][] result2 = TraceWire("U98, R91, D20, R16, D67, R40, U7, R15, U6, R7".Split(','));

            //int[][] result1 = TraceWire("R8,U5,L5,D3".Split(','));
            //int[][] result2 = TraceWire("U7,R6,D4,L4".Split(','));


            int[][] intersected = Intersect(result1, result2);
            Point start = FindPointsOfValue(intersected, 128, 0x00F0)[0];
            int minDistance = -1;
            foreach (Point intersection in FindPointsOfValue(intersected, 32, 0x00F0))
            {
                Point offset = PointDifference(start, intersection);
                int distance = Math.Abs(offset.X) + Math.Abs(offset.Y);
                if (minDistance == -1 || distance < minDistance)
                    minDistance = distance;

            }

            if (part2)
            {
                int[] line1 = GetIntersectionStepsOf(result1, intersected);
                int[] line2 = GetIntersectionStepsOf(result2, intersected);
                if (line1.Length != line2.Length)
                    throw new Exception();
                if (line1.Length != 0)
                {
                    minDistance = 0;
                    for (int i = 0; i < line1.Length; i++)
                    {
                        int distance = line1[i] + line2[i];
                        if (minDistance == 0 || distance < minDistance)
                            minDistance = distance;
                    }
                }
                else
                    minDistance = -1;
            }
            return string.Format("{1}{0}{2}{0}{3}\r\nMinimale Distanz: {4}",
                "\r\n" + "".PadLeft(100, '=') + "\r\n",
                "",//GenerateOutput(result1),
                "",//GenerateOutput(result2),
                GenerateOutput(intersected),
                minDistance);

        }

        #region Part1
        private int[][] TraceWire(string[] path)
        {
            List<int[]> result = new List<int[]>();   //X-Axis
            result.Add(new int[0]);
            List<int> curLine = new List<int>();      //Y-Axis
            curLine.Add(128);

            int x = 0;
            int y = 0;
            bool lastHorizontal = false;
            bool lastPositive = false;
            foreach (string instruction in path)
            {
                int steps = ParseInstruction(instruction.Trim(), out bool horizontal);


                if (horizontal != lastHorizontal)
                {
                    int value = 3;
                    if ((lastHorizontal && !lastPositive) || (!lastHorizontal && steps > 0))
                        value += 2;
                    if ((lastHorizontal && steps > 0) || (!lastHorizontal && !lastPositive))
                        value += 1;
                    curLine[y] = curLine[y] & 0x00F0 | value;
                }
                if ((curLine[y] & 0x00F0) == 128)
                {
                    int value = 0;
                    value += horizontal ? 1 : 0;
                    value += (steps > 0) ? 2 : 0;
                    curLine[y] = curLine[y] & 0x00F0 | value;
                }

                for (int i = 0; i < Math.Abs(steps); i++)
                {
                    if (horizontal)
                    {
                        result[x] = curLine.ToArray();
                        x += steps < 0 ? -1 : 1;
                        if (result.Count > x)
                        {
                            if (x < 0)
                            {
                                result.Insert(0, (curLine = new List<int>()).ToArray());
                                x++;
                            }
                            else
                                curLine = result[x].ToList();
                        }
                        else
                            result.Add((curLine = new List<int>()).ToArray());
                    }
                    else
                        y += steps < 0 ? -1 : 1;
                    while (curLine.Count <= y || y < 0)
                    {
                        if (y < 0)
                        {
                            for (int j = 0; j < result.Count; j++)
                            {
                                List<int> current = result[j].ToList();
                                current.Insert(0, 0);
                                result[j] = current.ToArray();
                            }
                            curLine.Insert(0, 0);
                            y++;
                        }
                        else
                            curLine.Add(0);
                    }
                    int wireValue = horizontal ? 1 : 2;
                    switch (curLine[y])
                    {
                        case 0:
                            curLine[y] = wireValue;
                            break;
                        case 1:
                            if (!horizontal)
                                curLine[y] = 7;
                            else
                                throw new Exception("Pfad doppelt belegt");
                            break;
                        case 2:
                            if (horizontal)
                                curLine[y] = 8;
                            else
                                throw new Exception("Pfad doppelt belegt");
                            break;
                        default:
                            throw new Exception("ungültiger Pfadzustand");
                    }
                }
                lastHorizontal = horizontal;
                lastPositive = steps >= 0;
            }
            curLine[y] = 64;
            result[x] = curLine.ToArray();
            return result.ToArray();
        }

        private int ParseInstruction(string instruction, out bool isHorizontal)
        {
            isHorizontal = false;
            if (!int.TryParse(instruction.Substring(1), out int steps))
                throw new ArgumentException("Invalid trace operation (Steps): " + instruction, "instruction");
            steps = Math.Abs(steps);
            switch (instruction.ToLower()[0])
            {
                case 'u':
                    isHorizontal = false;
                    return -steps;
                case 'd':
                    isHorizontal = false;
                    return steps;
                case 'l':
                    isHorizontal = true;
                    return -steps;
                case 'r':
                    isHorizontal = true;
                    return steps;
                default:
                    throw new ArgumentException("Invalid trace operation (Direction): " + instruction, "instruction");
            }
        }

        private string GenerateOutput(int[][] trace)
        {
            string output = "";
            foreach (int[] line in trace)
            {
                foreach (int value in line)
                {
                    char appendVal = ' ';
                    switch (value & 0x00F0)
                    {
                        case 128:
                            appendVal = 'O';
                            break;
                        case 64:
                            appendVal = '■';
                            break;
                        case 32:
                            appendVal = '#';
                            break;
                        default:
                            appendVal = wires[value & 0x000F];
                            break;
                    }
                    output += appendVal;
                }
                output += "\r\n";
            }
            return output;
        }

        private int[][] Intersect(int[][] grid1, int[][] grid2)
        {
            if (grid1.Length == 1) return grid1;

            EqualizeGrids(ref grid1, ref grid2);

            short intersectionCount = 0;
            List<int[]> intersected = grid1.ToList();
            for (int x = 0; x < grid2.Length; x++)
            {
                if (x >= intersected.Count)
                {
                    intersected.Add(new int[0]);
                    continue;
                }
                List<int> readLine = grid2[x].ToList();
                List<int> writeLine = intersected[x].ToList();
                for (int y = 0; y < readLine.Count; y++)
                {
                    if (y >= writeLine.Count)
                        writeLine.Add(0);
                    if (readLine[y] != 0)
                    {
                        if (writeLine[y] != 0 && writeLine[y] <= 0x000F)
                        {
                            writeLine[y] = writeLine[y] & 0x00FF | (intersectionCount << 8) | 32 | (readLine[y] & 0x000F);
                            intersectionCount++;
                        }
                        else
                            writeLine[y] = readLine[y];
                    }
                }
                intersected[x] = writeLine.ToArray();
            }

            return intersected.ToArray();
        }

        private void EqualizeGrids(ref int[][] grid1, ref int[][] grid2)
        {
            Point offset = PointDifference(FindPointsOfValue(grid1, 128, 0x00F0)[0], FindPointsOfValue(grid2, 128, 0x00F0)[0]);

            List<int[]> template = grid1.ToList();
            List<int[]> overlap = grid2.ToList();


            int difference = template.Count - overlap.Count;

            for (int i = 0; i < Math.Abs(difference); i++)
            {
                if (difference < 0)
                    template.Add(new int[0]);
                else
                    overlap.Add(new int[0]);
            }

            for (int i = 0; i < Math.Abs(offset.X); i++)
            {
                if (offset.X < 0)
                {
                    overlap.Insert(0, new int[0]);
                    template.Add(new int[0]);
                }
                else
                {
                    template.Insert(0, new int[0]);
                    overlap.Add(new int[0]);
                }
            }


            for (int i = 0; i < template.Count; i++)
            {
                List<int> tempLine = template[i].ToList();
                List<int> overLine = overlap[i].ToList();
                int space = offset.Y;
                while (Math.Abs(space) != 0)
                {
                    if (space < 0)
                    {
                        space++;
                        overLine.Insert(0, 0);
                        tempLine.Add(0);
                    }
                    else
                    {
                        space--;
                        tempLine.Insert(0, 0);
                        overLine.Add(0);
                    }
                }
                template[i] = tempLine.ToArray();
                overlap[i] = overLine.ToArray();
            }

            grid1 = template.ToArray();
            grid2 = overlap.ToArray();
        }

        private Point[] FindPointsOfValue(int[][] grid, int value, int mask)
        {
            List<Point> result = new List<Point>();
            for (int x = 0; x < grid.Length; x++)
            {
                for (int y = 0; y < grid[x].Length; y++)
                {
                    if ((grid[x][y] & mask) == value)
                        result.Add(new Point(x, y));
                }
            }
            return result.ToArray();
        }

        private Point PointDifference(Point start, Point end)
        {
            return new Point(end.X - start.X, end.Y - start.Y);
        }
        #endregion

        #region Part2
        private int[] GetIntersectionSteps(int[][] grid, bool startHorizontal, bool startPositive)
        {
            Point start = FindPointsOfValue(grid, 128, 0x00F0)[0];
            int x = start.X;
            int y = start.Y;
            int fieldValue = 16;
            bool horizontal = startHorizontal;
            bool positive = startPositive;
            int stepCount = 0;
            List<int> result = new List<int>();
            grid[x][y] = grid[x][y] & 0x00F0;

            do
            {
                if ((fieldValue & 0x000F) >= 3 && (fieldValue & 0x000F) <= 6 && ((fieldValue & 0x00F0) == 0))
                {
                    horizontal = !horizontal;
                    if (fieldValue == 3)
                        positive = false;
                    if (fieldValue == 6)
                        positive = true;
                }
                if (horizontal)
                    x += positive ? 1 : -1;
                else
                    y += positive ? 1 : -1;

                fieldValue = grid[x][y];
                stepCount++;
                if ((fieldValue & 32) != 0)
                {
                    int index = (fieldValue & 0xFF00) >> 8;
                    while (result.Count <= index)
                        result.Add(-1);
                    result[index] = stepCount;
                }
            } while ((fieldValue & 0x000F) >= 1 && (fieldValue & 0x000F) <= 8);
            if ((fieldValue & 0x00F0) != 64)
                throw new Exception();

            return result.ToArray();
        }

        private int[] GetIntersectionStepsOf(int[][] grid, int[][] intersected)
        {
            Point start = FindPointsOfValue(grid, 128, 0x00F0)[0];
            int startValue = grid[start.X][start.Y];
            bool isHorizontal = (startValue & 1) != 0;
            bool isPositive = (startValue & 2) != 0;
            return GetIntersectionSteps(intersected, isHorizontal, isPositive);
        }
        #endregion
    }
}
