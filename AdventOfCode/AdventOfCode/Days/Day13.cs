using AdventOfCode.Tools.IntComputer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day13: IDay
    {
        private IntComputer computer = new IntComputer(true);
        private int[] values = new int[3];
        private int pos = 0;
        private readonly char[] tiles = new char[] { ' ', '█', '▒', '=', '■' };
        private long drawCount = 0;

        public string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            computer.ReadMemory(input);
            computer.OnOutput += GameRenderer;
            computer.Run();
            return "Tiles drawn: " + drawCount.ToString();
        }

        private void GameRenderer(long value)
        {
            values[pos] = (int)value;
            pos++;
            if(pos >= 3)
            {
                pos = 0;
                Console.SetCursorPosition(values[0], values[1]);
                Console.Write(tiles[values[2]]);
                if(values[2] == 2)
                    drawCount++;
            }
        }
    }
}
