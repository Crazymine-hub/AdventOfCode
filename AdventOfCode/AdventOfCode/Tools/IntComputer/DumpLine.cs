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
        public bool HasCode { get; }

        public DumpLine(string line, int instructions, int startPos, bool hasCode)
        {
            Line = line;
            Instructions = instructions;
            StartPos = startPos;
            HasCode = hasCode;
        }
    }
}
