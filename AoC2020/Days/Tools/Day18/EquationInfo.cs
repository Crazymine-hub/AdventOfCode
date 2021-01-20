using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day18
{
    class EquationInfo
    {
        public long Value { get; set; }
        public char CurrOperator { get; set; }
        public int BracketLevel { get; set; }

        public EquationInfo()
        {
            Value = 0;
            CurrOperator = '+';
        }

        public EquationInfo(int value, char startOperator)
        {
            Value = value;
            CurrOperator = startOperator;
        }
    }
}
