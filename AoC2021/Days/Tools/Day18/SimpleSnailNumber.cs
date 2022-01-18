using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day18
{
    internal class SimpleSnailNumber : ISnailLiteral
    {
        public int Value { get; set; }
        public int Magnitude { get => Value; }

        public SimpleSnailNumber(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
