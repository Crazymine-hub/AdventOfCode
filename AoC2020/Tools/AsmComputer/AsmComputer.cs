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
        long executiontimer = 0;
        public long Accumulator { get; protected set; }
        public List<InstructionInfo> Instructions { get; set; } = new List<InstructionInfo>();

        //Settings for the Computer
        public bool PreventDoubleExecution { get; set; } = false;

        public AsmComputer()
        {

        }

        public void Reset(bool wipe)
        {
            if (wipe)
                Instructions = new List<InstructionInfo>();
            foreach (var instr in Instructions)
                instr.ExecutionTime = -1;
            position = 0;
            Accumulator = 0;
            executiontimer = 0;
        }

        public void LoadMemory(List<string> inputLines)
        {
            Reset(true);
            foreach (string instruction in inputLines)
            {//each line has to contain one instruction
                string[] segments = instruction.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                long arg = long.Parse(segments[1]);
                Instructions.Add(new InstructionInfo(segments[0], arg));
            }
        }

        public int Run()
        {
            Reset(false);
            for (position = 0; position < Instructions.Count; ++position)
            {
                var currInst = Instructions.ElementAt(position);
                if (PreventDoubleExecution && currInst.ExecutionTime >= 0) return -1;
                currInst.ExecutionTime = executiontimer++;
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
            return 0;
        }

        private void PrintDebugHead()
        {
            Console.WriteLine("AsmComputer v1.0");
            Console.WriteLine("Accumulator: " + Accumulator);
            Console.WriteLine("ExecutionTime: " + executiontimer);
        }

        private void PrintInstructionLine(int lineNumber)
        {
            int linePad = Instructions.Count.ToString().Count();
            Console.Write(lineNumber == position ? ">" : " ");
            Console.Write(lineNumber.ToString().PadLeft(linePad) + " | ");
            Console.WriteLine(Instructions.ElementAt(lineNumber).ToString());
        }

        public void PrintState()
        {
            PrintDebugHead();
            for (int i = 0; i < Instructions.Count; i++)
                PrintInstructionLine(i);
        }

        public void PrintTrace()
        {
            if (!PreventDoubleExecution) Console.WriteLine("Double Execution must be prevented");
            PrintDebugHead();
            for (int i = 0; i < executiontimer; i++)
                PrintInstructionLine(Instructions.IndexOf(Instructions.Single(x => x.ExecutionTime == i)));
            PrintInstructionLine(position);
        }
    }
}
