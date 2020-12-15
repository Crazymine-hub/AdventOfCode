using AdventOfCode.Tools.AsmComputer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day8 : DayBase
    {
        public override string Title => "Handheld Halting";

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            AsmComputer comp = new AsmComputer();
            comp.LoadMemory(GetLines(input));
            comp.PreventDoubleExecution = true;
            comp.Run();
            return "Computer Accumulator Value = " + comp.Accumulator;
        }
    }
}
