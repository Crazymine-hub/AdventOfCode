using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day10 : DayBase
    {
        public override string Title => "Cathode-Ray Tube";
        int x = 1;
        int cycles = 0;
        int signalStrength = 0;
        Bitmap screen;
        private VisualFormHandler form;
        const int pixelSize = 5;

        public override string Solve(string input, bool part2)
        {
            screen = new Bitmap(40 * pixelSize, 6 * pixelSize);
            screen.FillRect(new Rectangle(new Point(0, 0), screen.Size), Color.FromArgb(10, 10, 10));
            form = VisualFormHandler.GetInstance();
            form.Show(screen);
            foreach (var instruction in GetLines(input))
            {
                var arguments = instruction.Split(' ');
                switch (arguments[0])
                {
                    case "noop":
                        HandleCycles(1);
                        break;
                    case "addx":
                        HandleCycles(2);
                        x += int.Parse(arguments[1]);
                        break;
                }
            }
            return $"Final Signal Strength is {signalStrength}";
        }

        private void HandleCycles(int cycleCount)
        {
            for (int i = 0; i < cycleCount; i++)
            {
                cycles++;
                if ((cycles - 20) % 40 == 0)
                    signalStrength += cycles * x;

                var column = (cycles - 1) % 40;
                var row = (cycles - 1) / 40 % 6;

                Console.WriteLine($"Cycle {cycles - 1} Position {column} X: {x}");

                if (column >= x - 1 && column <= x + 1)
                {
                    screen.FillRect(new Rectangle(column * pixelSize, row * pixelSize, pixelSize, pixelSize), Color.White);
                    form.Update(screen);
                }
            }
        }
    }
}
