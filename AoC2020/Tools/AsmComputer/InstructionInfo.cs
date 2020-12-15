using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.AsmComputer
{
    public class InstructionInfo
    {
        public string Instruction { get; set; }
        public long Argument { get; set; }
        public long ExecutionTime { get; set; }

        public InstructionInfo(string instruction, long arg)
        {
            Instruction = instruction;
            Argument = arg;
            ExecutionTime = -1;
        }

        public override string ToString()
        {
            return Instruction.PadRight(5) + " " +
                Argument.ToString().PadRight(5) +
                " @" + ExecutionTime;
        }
    }
}
