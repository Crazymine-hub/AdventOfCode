using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Tools.IntComputer;

namespace AdventOfCode.Days
{
    public class Day19 : DayBase
    {
        IntComputer computer = new IntComputer(true);
        List<Point> coords = new List<Point>();
        bool doPrint = true;
        int cntr = 0;
        string program;
        int result = 0;

        public override string Solve(string input, bool part2)
        {
            program = input;
            computer.OnOutput += Computer_OnOutput;
            if (!part2)
                return Part1();
            else
                return Part2();
        }

        private string Part2()
        {
            int dimension = 1;
            int start = 0;
            int end = 0;
            int width = 100;
            int height = 100;
            //doPrint = false;
            Dictionary<int, int> lengths = new Dictionary<int, int>();
            while ((lengths.Where(entry => entry.Value >= width && entry.Key >= start + height - 1).Count() == 0))
            {
                DrawVertical(dimension - 1, dimension, out Point StartP, out Point EndP);
                for (int h = StartP.Y; h < EndP.Y; h++)
                {
                    if (lengths.ContainsKey(h))
                        lengths[h]++;
                    else
                        lengths.Add(h, 1);
                }

                if (lengths.Count > 0)
                    for (int i = lengths.Keys.Min(); i < StartP.Y; i++)
                        lengths.Remove(i);
                start = StartP.Y;
                end = EndP.Y;

                if (DrawHorizontal(dimension - 1, dimension - 1) != 0) return "OOPS";
                dimension++;
            }
            int x = dimension - width - 1;
            int y = start;
            return $"({x}|{y})";
        }

        private int DrawVertical(int x, int height, out Point firstPull, out Point lastPull)
        {
            firstPull = new Point();
            lastPull = new Point();
            bool foundPoint = false;
            int cntr = this.cntr;
            for (int y = 0; y < height; y++)
            {
                Draw(x, y);
                if (result == 1 && !foundPoint)
                {
                    foundPoint = true;
                    firstPull = new Point(x, y);
                }
                if (result == 0 && foundPoint)
                {
                    foundPoint = false;
                    lastPull = new Point(x, y);
                }
            }
            return this.cntr - cntr;
        }

        private int DrawHorizontal(int width, int y)
        {
            bool foundPoint = false;
            int cntr = this.cntr;
            for (int x = 0; x < width; x++)
            {
                Draw(x, y);
                if (result == 1 && !foundPoint)
                {
                    foundPoint = true;
                }
            }
            return this.cntr - cntr;
        }

        private void Draw(int x, int y)
        {
            computer.Reset();
            computer.ReadMemory(program);
            computer.AddInput(x);
            computer.AddInput(y);
            if (doPrint)
                Console.SetCursorPosition(x, y);
            computer.Run();
        }

        private string Part1()
        {
            var outAssi = new Tools.ConsoleAssist();
            for (int y = 0; y < 50; y++)
                for (int x = 0; x < 50; x++)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = 0;
                    Console.Write(outAssi.GetNextProgressChar());
                    Draw(x, y);
                }
            return cntr.ToString();
        }

        private void Computer_OnOutput(long value)
        {
            if (doPrint)
                Console.Write(value == 0 ? "." : "#");
            result = value == 0 ? 0 : 1;
            cntr += result;
        }
    }
}
