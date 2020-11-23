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
        int pos = 0;
        int cntr = 0;
        string program;

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            program = input;
            computer.OnOutput += Computer_OnOutput;
            var outAssi = new Tools.ConsoleAssist();
            foreach (int i in NextStep())
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.Write(outAssi.GetNextProgressChar());
            }
            return cntr.ToString();
        }

        private IEnumerable<int> NextStep()
        {
            for (int y = 0; y < 50; y++)
                for (int x = 0; x < 50; x++)
                {
                    computer.Reset();
                    computer.ReadMemory(program);
                    computer.AddInput(x);
                    computer.AddInput(y);
                    coords.Add(new Point(x, y));
                    computer.Run();
                    yield return 1;
                }
        }

        private void Computer_OnOutput(long value)
        {
            Console.SetCursorPosition(coords[pos].X, coords[pos++].Y);
            Console.Write(value == 0 ? " " : "#");
            cntr += value == 0 ? 0 : 1;
        }
    }
}
