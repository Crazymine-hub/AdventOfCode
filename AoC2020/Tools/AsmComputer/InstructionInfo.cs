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
        public bool WasExecuted { get; set; }

        public InstructionInfo(string instruction, long arg)
        {
            Instruction = instruction;
            Argument = arg;
            WasExecuted = false;
        }

        public override string ToString()
        {
            return Instruction + " " + Argument;
        }
    }
}
