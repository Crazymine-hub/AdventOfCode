using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.IntComputer
{
    public class IntComputer
    {
        public int[] Memory { get; private set; }
        private readonly byte operationMask = 0;

        public int ReadMemory(string input)
        {
            List<int> codes = new List<int>();
            string[] codeList = input.Split(',');
            for (int i = 0; i < codeList.Length; i++)
            {
                if (!int.TryParse(codeList[i].Trim('\r', ','), out int opCode)) return -i;
                codes.Add(opCode);
            }
            Memory = codes.ToArray();
            return 0;
        }

        public void Reset()
        {
            Memory = new int[0];
        }

        public void Run()
        {
            int stepSize = 0;
            for (int i = 0; i < Memory.Length; i += stepSize)
            {
                stepSize = ExecuteAt(i, true, out int result);
                if (stepSize == -1) return;
            }
        }

        public void Debug()
        {
            int line = 0;
            int evalLine = 0;

            while (true)
            {
                bool fresh = true;
                Console.Clear();
                DumpLine[] memDump = DumpMemory();
                Console.WriteLine("DEBUG MODE");
                Console.WriteLine("==========");
                int eval = 0;
                for (int i = 0; i < memDump.Length; i++)
                {
                    Console.WriteLine("{0} {1}", (memDump[i].StartPos).ToString().PadRight(5), memDump[i].Line);
                    if (i == line)
                    {
                        Console.CursorTop--;
                        Console.CursorLeft = 5;
                        Console.WriteLine(">");
                    }
                }

                evalLine = Console.CursorTop;
                while (fresh)
                {
                    Console.SetCursorPosition(0, evalLine);
                    Console.Write("".PadLeft(Console.WindowWidth));
                    ExecuteAt(memDump[line].StartPos, false, out eval);
                    Console.SetCursorPosition(0, evalLine);
                    Console.WriteLine("Evaluation = " + eval);
                    int prevLine = line;
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (--line < 0) line = memDump.Length - 1;
                            break;
                        case ConsoleKey.DownArrow:
                            if (++line >= memDump.Length) line = 0;
                            break;
                        case ConsoleKey.Escape:
                            return;
                        case ConsoleKey.Enter:
                            ExecuteAt(memDump[line].StartPos, true, out eval);
                            fresh = false;
                            break;
                        case ConsoleKey.Spacebar:
                            Console.SetCursorPosition(0, evalLine + 1);
                            Console.WriteLine("Enter adresses to be evaluated separated by ','");
                            string[] addresses = Console.ReadLine().Split(',');
                            for (int i = 0; i < addresses.Length; i++)
                            {
                                if (!int.TryParse(addresses[i], out int address))
                                {
                                    Console.WriteLine("Invalid Address" + addresses[i]);
                                    break;
                                }
                                Console.WriteLine("{0} = {1}", addresses[i], (address < Memory.Length && address >= 0)? ReadAddress(address).ToString():"OUTSIDE OF MEMORY");
                            }
                            Console.WriteLine("Done! Press any key to return...");
                            Console.ReadKey(false);
                            int lowest = Console.CursorTop;
                            for (int i = evalLine + 1; i <= lowest; i++)
                            {
                                Console.CursorTop = i;
                                Console.WriteLine("".PadLeft(Console.WindowWidth));
                            }
                            break;
                    }
                    Console.SetCursorPosition(5, prevLine + 2);
                    Console.Write(" │");
                    Console.SetCursorPosition(5, line + 2);
                    Console.Write(">│");
                }
            }

        }



        private int ExecuteAt(int address, bool Write, out int result)
        {
            int instructionsUsed = 0;
            result = 0;
            switch (Memory[address])
            {
                case 1:
                    instructionsUsed = 4;
                    result = WriteAddressP(address + 3, ReadAddressP(address + 1) + ReadAddressP(address + 2), Write);
                    break;
                case 2:
                    instructionsUsed = 4;
                    result = WriteAddressP(address + 3, ReadAddressP(address + 1) * ReadAddressP(address + 2), Write);
                    break;
                case 99:
                    return -1;
                default:
                    throw new InvalidOperationException(string.Format("OPCode {0}@{1}", Memory[address], address));
            }
            return instructionsUsed;
        }

        private int ReadAddress(int Address)
        {
            if (Address < Memory.Length)
                return Memory[Address];
            else
                throw new ArgumentOutOfRangeException("Address", Address, "Address unavailable (Outside of Memory)");
        }

        private int WriteAddress(int Address, int value, bool doWrite)
        {
            if (doWrite)
            {
                if (Address < Memory.Length)
                    Memory[Address] = value;
                else
                    throw new ArgumentOutOfRangeException("Address", Address, "Address unavailable (Outside of Memory)");
            }
            return value;
        }

        private int ReadAddressP(int Address)
        {
            return ReadAddress(ReadAddress(Address));
        }

        private int WriteAddressP(int Address, int value, bool doWrite)
        {
            return WriteAddress(ReadAddress(Address), value, doWrite);
        }

        public string PrintMemory()
        {
            DumpLine[] dump = DumpMemory();
            string result = "";
            foreach (DumpLine line in dump)
                result += line.Line + "\r\n";
            return result;
        }

        private DumpLine[] DumpMemory()
        {
            int instructionCount = 0;
            int maxInstructions = 4;
            int instructionsTotal = 0;
            string line = "";
            List<DumpLine> result = new List<DumpLine>();
            int maxLength = Memory.Max().ToString().Length;
            for (int i = 0; i < Memory.Length; i++)
            {
                if (instructionCount == maxInstructions && maxInstructions >= 0)
                {
                    result.Add(new DumpLine(line.Trim('\t'), instructionCount, instructionsTotal));
                    line = "";
                    instructionsTotal += instructionCount;
                    instructionCount = 0;
                    maxInstructions = GetOpInstructionCount(Memory[i]);
                }
                line += "│" + string.Format("{0}", Memory[i]).PadRight(maxLength);
                instructionCount++;
            }
            result.Add(new DumpLine(line.Trim('\t'), instructionCount, instructionsTotal));
            return result.ToArray();
        }

        private int GetOpInstructionCount(int instruction)
        {
            if (instruction == 99) return -1;
            else return 4;
        }
    }
}
