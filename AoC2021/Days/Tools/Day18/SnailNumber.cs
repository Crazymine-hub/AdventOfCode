using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day18
{
    internal class SnailNumber : ISnailLiteral
    {
        private ISnailLiteral left;
        private ISnailLiteral right;

        public ISnailLiteral Left
        {
            get => left; set
            {
                left = value;
                left.Parent = this;
            }
        }
        public ISnailLiteral Right
        {
            get => right; set
            {
                right = value;
                right.Parent = this;
            }
        }
        public SnailNumber Parent { get; set; }

        public int Depth => (Parent?.Depth ?? -1) + 1;

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
