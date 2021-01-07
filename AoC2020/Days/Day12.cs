using AdventOfCode.Tools;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day12 : DayBase
    {
        public override string Title => "Rain Risk";

        Point position = new Point();
        Direction direction = Direction.East;

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable.";
            foreach (string instruction in GetLines(input))
            {
                DoInstruction(instruction);
                Console.WriteLine("Changing Course: " + instruction);
            }
            return "Distance to Start: " + VectorAssist.ManhattanDistance(new Point(), position).ToString();
        }

        private void DoInstruction(string instruction)
        {
            int amount = int.Parse(instruction.Remove(0, 1));
            switch (instruction[0])
            {
                case 'N':
                    position.Y -= amount;
                    break;
                case 'W':
                    position.X -= amount;
                    break;
                case 'S':
                    position.Y += amount;
                    break;
                case 'E':
                    position.X += amount;
                    break;
                case 'R':
                    amount /= 90;
                    for (int i = 0; i < amount; i++)
                    {
                        direction += 1;
                        if (direction > Direction.West) direction = Direction.North;
                    }
                    break;
                case 'L':
                    amount /= 90;
                    for (int i = 0; i < amount; i++)
                    {
                        direction -= 1;
                        if (direction < Direction.North) direction = Direction.West;
                    }
                    break;
                case 'F':
                    DoInstruction(direction.ToString("G").ToUpper()[0] + amount.ToString());
                    break;
                default: throw new InvalidOperationException("Invalid Move: " + instruction);
            }
        }
    }
}
