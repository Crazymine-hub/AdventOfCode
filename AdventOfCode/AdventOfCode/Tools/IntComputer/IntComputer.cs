using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.IntComputer
{
    /*------------------------------------
     * Complte! Undocumented But Complete
     *------------------------------------*/
    public class IntComputer
    {
        public delegate void OutputPushDelegate(long value);
        public delegate long InputRequestDelegate();

        public long[] Memory { get; private set; }
        private int inputPos = 0;
        private List<long> output;
        private bool autoMode;
        private long addressOffset = 0;

        public long[] Inputs { get; set; }
        public long[] Output { get { return output.ToArray(); } }
        public event OutputPushDelegate OnOutput;
        public event InputRequestDelegate InputRequested;
        public Task ExecutingTask { get; private set; }
        public string Name { get; set; }

        public IntComputer(bool useAutoMode = false, long[] inputList = null)
        {
            if (inputList == null)
                Inputs = new long[0];
            else
                Inputs = inputList;
            autoMode = useAutoMode;
            Name = "Computer";
        }

        public int ReadMemory(string input)
        {
            //input = "0,1,1,1,0";
            List<long> codes = new List<long>();
            string[] codeList = input.Split(',');
            for (int i = 0; i < codeList.Length; i++)
            {
                if (!long.TryParse(codeList[i].Trim('\r', ','), out long opCode)) return -i;
                codes.Add(opCode);
            }
            Memory = codes.ToArray();
            return 0;
        }

        public void Reset()
        {
            Memory = new long[0];
            Inputs = new long[0];
            output = new List<long>();
            inputPos = 0;
        }

        public void Run()
        {

            output = new List<long>();
            int stepSize = 0;
            for (long i = 0; i < Memory.Length; i += stepSize)
            {
                stepSize = ExecuteAt(i, true, out long result);
                if (stepSize == -1) return;
                if (stepSize == -2)
                {
                    stepSize = 0;
                    i = result;
                }
            }
            return;
        }

        public Task RunAsync()
        {
            if (!autoMode)
                throw new InvalidOperationException("The Computer needs to be Run in AutoMode");
            ExecutingTask = Task.Run(new Action(Run));
            return ExecutingTask;
        }

        public void AddInput(long input)
        {
            List<long> inputList = Inputs.ToList();
            inputList.Add(input);
            Inputs = inputList.ToArray();
        }

        public void Debug()
        {
            output = new List<long>();
            int line = 0;
            int evalLine = 0;

            while (true)
            {
                bool fresh = true;
                Console.Clear();
                DumpLine[] memDump = DumpMemory();
                Console.WriteLine("DEBUG MODE");
                Console.WriteLine("==========");
                long eval = 0;
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


        private int ExecuteAt(long address, bool Write, out long result, bool noEvaluate = false)
        {
            int instructionsUsed;
            result = 0;
            long instruction = Memory[address] % 100;
            ulong[] paramModes = NumberLists.MakeArray((ulong)Math.Abs(Memory[address] / 100)).Reverse().ToArray();
            switch (instruction)
            {
                case 1:
                    instructionsUsed = 4;
                    if (noEvaluate) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = WriteAddressP(address + 3, GetParamValue(address + 1, paramModes[0]) + GetParamValue(address + 2, paramModes[1]), paramModes[2], Write);
                    break;
                case 2:
                    instructionsUsed = 4;
                    if (noEvaluate) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = WriteAddressP(address + 3, GetParamValue(address + 1, paramModes[0]) * GetParamValue(address + 2, paramModes[1]), paramModes[2], Write);
                    break;
                case 3:
                    instructionsUsed = 2;
                    if (noEvaluate || !Write) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    WriteAddressP(address + 1, ReadInput(), paramModes[0], Write);
                    break;
                case 4:
                    instructionsUsed = 2;
                    if (noEvaluate) break;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = GetParamValue(address + 1, paramModes[0]);
                    if (Write)
                    {
                        if (autoMode)
                            OnOutput?.Invoke(result);
                        else
                            Console.WriteLine("Output:" + result);
                        output.Add(result);
                    }
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
                    WriteAddressP(address + 3, result, paramModes[2], Write);
                    break;
                case 8:
                    instructionsUsed = 4;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = (GetParamValue(address + 1, paramModes[0]) == GetParamValue(address + 2, paramModes[1])) ? 1 : 0;
                    WriteAddressP(address + 3, result, paramModes[2], Write);
                    break;
                case 9:
                    instructionsUsed = 2;
                    paramModes = MakeValidModeList(paramModes, instructionsUsed);
                    result = addressOffset += GetParamValue(address + 1, paramModes[0]);
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

        private long ReadInput()
        {
            if (inputPos < Inputs.Length || autoMode)
            {
                if (InputRequested != null)
                {
                    long input = InputRequested.Invoke();
                    AddInput(input);
                }

                while (Inputs.Length <= inputPos) { }
                return Inputs[inputPos++];
            }
            return ConsoleAssist.GetUserInput("Enter a number...");
        }

        private long GetParamValue(long address, ulong mode)
        {
            switch (mode)
            {
                case 1: return ReadAddress(address);
                case 2: return ReadAddress(addressOffset + ReadAddress(address));
                case 0: return ReadAddressP(address);
                default: throw new InvalidOperationException($"Unknown Parametermode: {mode}");
            }
        }

        private long ReadAddress(long Address)
        {
            if (Address >= Memory.Length)
            {
                long[] newMem = new long[Address + 1];
                Memory.CopyTo(newMem, 0);
                Memory = newMem;
            }
            return Memory[Address];
        }

        private long WriteAddress(long Address, long value, bool doWrite)
        {
            if (doWrite)
            {
                if (Address >= Memory.Length)
                {
                    long[] newMem = new long[Address + 1];
                    Memory.CopyTo(newMem, 0);
                    Memory = newMem;
                }
                Memory[Address] = value;

            }
            return value;
        }

        private long ReadAddressP(long Address)
        {
            return ReadAddress(ReadAddress(Address));
        }

        private long WriteAddressP(long address, long value, ulong paramMode, bool doWrite)
        {
            switch (paramMode)
            {
                case 1: throw new InvalidOperationException("Immediate Parameter mode not allowed for write operations.");
                case 2:
                    address = addressOffset + ReadAddress(address);
                    break;
                case 0:
                    address = ReadAddress(address);
                    break;
                default: throw new InvalidOperationException($"Unknown Parametermode: {paramMode}");
            }
            return WriteAddress(address, value, doWrite);
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
            return ExecuteAt(address, false, out long ignoredValue, true);
        }
    }
}
