using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.DynamicGrid
{
    public class DynamicGridValue<T>
    {
        public T Value { get; }
        public int X { get; }
        public int Y { get; }

        internal DynamicGridValue(int x, int y, T value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public override string ToString() => $"{Value} @({X},{Y})";

        public static implicit operator T(DynamicGridValue<T> gridValue)
        {
            return gridValue.Value;
        }
    }
}
