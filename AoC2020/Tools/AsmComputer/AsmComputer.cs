using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.AsmComputer
{
    /// <summary>
    /// A Computer that seems to understand some kind of Assembler Language
    /// </summary>
    public class AsmComputer
    {
        //State of the Computer
        protected int position = 0;
        public long Accumulator { get; protected set; }
        public List<InstructionInfo> Instructions { get; set; } = new List<InstructionInfo>();

        //Settings for the Computer
        public bool PreventDoubleExecution { get; set; } = false;

        public AsmComputer()
        {

        }

        public void Reset()
        {
            Instructions = new List<InstructionInfo>();
            position = 0;
            Accumulator = 0;
        }

        public void LoadMemory(List<string> inputLines)
        {
            Reset();
            foreach(string instruction in inputLines)
            {//each line has to contain one instruction
                string[] segments = instruction.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                long arg = long.Parse(segments[1]);
                Instructions.Add(new InstructionInfo(segments[0], arg));
            }
        }

        public void Run()
        {
            Accumulator = 0;
            for(position = 0; position < Instructions.Count; ++position)
            {
                var currInst = Instructions.ElementAt(position);
                if (PreventDoubleExecution && currInst.WasExecuted) return;
                currInst.WasExecuted = true;
                //Execute the current instruction.
                switch (currInst.Instruction)
                {
                    //Change the accumulator value
                    case "acc":
                        Accumulator += currInst.Argument;
                        break;
                    //move the instruction pointer (gets increased by loop, thus decrease by 1)
                    case "jmp":
                        position += Convert.ToInt32(currInst.Argument - 1);
                        break;
                    //Do nothing
                    case "nop":
                        break;
                    //That's nothing we know now
                    default: throw new InvalidOperationException("Unknown Instruction: " + currInst.Instruction);
                }
            }
        }
    }
}
