using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.IntComputer
{
    abstract class ComputerManager: IDay
    {
        public string Solve(string input, bool part2)
        {
            IntComputer computer = new IntComputer();

            computer.ReadMemory(input);
            if (part2)
            {
                computer.Debug();
                return "";
            }
            computer.Run();
            return computer.PrintMemory();
        }
    }
}
