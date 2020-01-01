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
        private readonly int[] inputs;
        private int inputPos = 0;

        public IntComputer()
        {
            inputs = new int[0] { };
        }
        public IntComputer(int[] inputList)
        {
            inputs = inputList;
        }

        public int ReadMemory(string input)
        {
            //input = "0,1,1,1,0";
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
                if (stepSize == -2)
                {
                    stepSize = 0;
                    i = result;
                }
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
                    Console.SetCursorPosition(4, line + 2);
                    Console.Title = "AoCompute DEBUG >";
                    try
                    {
                        ExecuteAt(memDump[line].StartPos, false, out eval);
                        Console.Title += string.Format("{0} {1} -> {2}", memDump[line].StartPos, memDump[line].Line, eval);
                    }
                    catch
                    {
                        Console.Title += " Instruction Error!";
                    }
                    int prevLine = line;
                    switch (Console.ReadKey(false).Key)
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
                            if (memDump[line].HasCode)
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
                                Console.WriteLine("{0} = {1}", addresses[i], (address < Memory.Length && address >= 0) ? ReadAddress(address).ToString() : "OUTSIDE OF MEMORY");
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



        private int ExecuteAt(int address, bool Write, out int result, bool noEvaluate = false)
        {
            int instructionsUsed;
            result = 0;
            int instruction = Memory[address] % 100;
            ulong[] paramModes = NumberLists.MakeArray((ulong)Math.Abs(Memory[address] / 100)).Reverse().ToArray();
            switch (instruction)
            {
                case 1:
                    instructionsUsed = 4;
                    if (noEvaluate) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = WriteAddressP(address + 3, GetParamValue(address + 1, paramModes[0]) + GetParamValue(address + 2, paramModes[1]), Write);
                    break;
                case 2:
                    instructionsUsed = 4;
                    if (noEvaluate) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = WriteAddressP(address + 3, GetParamValue(address + 1, paramModes[0]) * GetParamValue(address + 2, paramModes[1]), Write);
                    break;
                case 3:
                    instructionsUsed = 2;
                    if (noEvaluate) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    string message = "";
                    if(inputPos < inputs.Length)
                    {
                        WriteAddressP(address + 1, inputs[inputPos], Write);
                        inputPos++;
                    }
                    while (Write)
                    {
                        Console.Clear();
                        if (message != "")
                            Console.WriteLine(message);
                        message = "";
                        Console.WriteLine("Enter a number:");
                        if (!int.TryParse(Console.ReadLine(), out int input))
                        {
                            message = "You didn't enter a number";
                            continue;
                        }
                        WriteAddressP(address + 1, input, Write);
                        break;
                    }
                    break;
                case 4:
                    instructionsUsed = 2;
                    if (noEvaluate) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = GetParamValue(address + 1, paramModes[0]);
                    if (Write)
                        Console.WriteLine("Output:" + result);
                    break;
                case 5:
                    instructionsUsed = -2;
                    if (!Write) instructionsUsed = 3;
                    paramModes = MakeValidModeList(paramModes, 3);
                    if (GetParamValue(address + 1, paramModes[0]) != 0)
                        result = GetParamValue(address + 2, paramModes[1]);
                    else
                        instructionsUsed = 3;
                    break;
                case 6:
                    instructionsUsed = -2;
                    if (!Write) instructionsUsed = 3;
                    paramModes = MakeValidModeList(paramModes, 3);
                    if (GetParamValue(address + 1, paramModes[0]) == 0)
                        result = GetParamValue(address + 2, paramModes[1]);
                    else
                        instructionsUsed = 3;
                    break;
                case 7:
                    instructionsUsed = 4;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = (GetParamValue(address + 1, paramModes[0]) < GetParamValue(address + 2, paramModes[1])) ? 1 : 0;
                    WriteAddressP(address + 3, result, Write);
                    break;
                case 8:
                    instructionsUsed = 4;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = (GetParamValue(address + 1, paramModes[0]) == GetParamValue(address + 2, paramModes[1])) ? 1 : 0;
                    WriteAddressP(address + 3, result, Write);
                    break;
                case 0:
                    instructionsUsed = 1;
                    break;
                case 99:
                    return -1;
                default:
                    if (noEvaluate) return -1;
                    throw new InvalidOperationException(string.Format("OPCode {0}@{1}", Memory[address], address));
            }
            return instructionsUsed;
        }

        private ulong[] MakeValidModeList(ulong[] modeList, int neededLength)
        {
            List<ulong> result = modeList.ToList();
            while (result.Count < neededLength)
                result.Add(0);
            return result.ToArray();
        }

        private int GetParamValue(int address, ulong mode)
        {
            switch (mode)
            {
                case 1: return ReadAddress(address);
                default: return ReadAddressP(address);
            }
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
            int maxInstructions = GetOpInstructionCount(0);
            if (maxInstructions == -1)
                maxInstructions = 1;
            int instructionsTotal = 0;
            string line = "";
            List<DumpLine> result = new List<DumpLine>();
            int maxLength = Memory.Max().ToString().Length;
            for (int i = 0; i < Memory.Length; i++)
            {
                if (instructionCount == maxInstructions)
                {
                    result.Add(new DumpLine(line.Trim('\t'), instructionCount, instructionsTotal, maxInstructions != -1));
                    line = "";
                    instructionsTotal += instructionCount;
                    instructionCount = 0;
                    maxInstructions = GetOpInstructionCount(i);
                    if (maxInstructions == -1)
                        maxInstructions = 1;
                }
                line += "│" + string.Format("{0}", Memory[i]).PadRight(maxLength);
                instructionCount++;
            }
            result.Add(new DumpLine(line.Trim('\t'), instructionCount, instructionsTotal, maxInstructions != -1));
            return result.ToArray();
        }

        private int GetOpInstructionCount(int address)
        {
            return ExecuteAt(address, false, out int ignoredValue, true);
        }
    }
}
