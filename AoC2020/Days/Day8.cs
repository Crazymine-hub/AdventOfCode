﻿using AdventOfCode.Tools.AsmComputer;
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
            // Set Up Computer and execute
            AsmComputer comp = new AsmComputer();
            comp.LoadMemory(GetLines(input));
            comp.PreventDoubleExecution = true;
            int execResult = comp.Run();
            if (!part2) return "Computer Accumulator Value = " + comp.Accumulator;

            //Part 2 only
            //Get all executed jmp and nop instructions in reversed execution order
            List<InstructionInfo> trace = comp.Instructions.Where(x => x.ExecutionTime >= 0 && (x.Instruction == "jmp" || x.Instruction == "nop"))
                .OrderBy(x => x.ExecutionTime).Reverse().ToList();

            //Try Executing with each instruction inverted
            int listPos = 0;
            Console.WriteLine("Changing Instruction: ");
            while (listPos < trace.Count && execResult == -1)
            {
                Console.Write(comp.Instructions.IndexOf(trace[listPos]));
                Console.Write(" | ");
                Console.Write(trace[listPos].ToString().PadRight(20));

                InvertInstruction(trace[listPos]);
                execResult = comp.Run();
                Console.WriteLine("-> " + execResult + " Acc: " + comp.Accumulator);
                if (execResult == -1)
                    //Revert Changes and increase to next item
                    InvertInstruction(trace[listPos++]);
            }

            if (execResult == 0)
                return "Computer Accumulator Value = " + comp.Accumulator;
            else return "No instruction ran successfull.";
        }

        /// <summary>
        /// Inverts a JMP or NOP Instruction to the other.
        /// </summary>
        private void InvertInstruction(InstructionInfo instruction)
        {
            if (instruction.Instruction == "nop")
                instruction.Instruction = "jmp";
            else if (instruction.Instruction == "jmp")
                instruction.Instruction = "nop";
        }
    }
}
