using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.IntComputer
{
    struct DumpLine
    {
        public string Line { get; }
        public int Instructions { get; }
        public int StartPos { get; }

        public DumpLine(string line, int instructions, int startPos)
        {
            Line = line;
            Instructions = instructions;
            StartPos = startPos;
        }
    }
}
