using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day18
{
    internal class SnailNumber: ISnailLiteral
    {
        public ISnailLiteral Left { get; set; }
        public ISnailLiteral Right { get; set; }

        public int Magnitude => Left.Magnitude * 3 + Right.Magnitude * 2;

        public SnailNumber(ISnailLiteral left, ISnailLiteral right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString()
        {
            return $"[{Left},{Right}]";
        }
    }
}
