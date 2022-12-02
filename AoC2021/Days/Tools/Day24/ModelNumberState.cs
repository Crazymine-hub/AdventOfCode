using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day24
{
    [DebuggerDisplay("State L:{Level} D:{Digit} Z:{ZResult}")]
    internal class ModelNumberState
    {
        public int Level { get; }
        public int Digit { get; }
        public int ZResult { get; }
        public ModelNumberState ParentState { get; }

        public ModelNumberState(int level, int digit, int score, ModelNumberState parentState)
        {
            Level = level;
            Digit = digit;
            ZResult = score;
            ParentState = parentState;
        }
    }
}
