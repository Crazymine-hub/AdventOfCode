using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.DynamicGrid
{
    public struct DynamicGridValue<T>
    {
        public T Value { get; }
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        internal DynamicGridValue(int x, int y, int z, T value)
        {
            X = x;
            Y = y;
            Z = z;
            Value = value;
        }

        public override string ToString() => $"{Value} @({X},{Y})";

        public static implicit operator T(DynamicGridValue<T> gridValue)
        {
            return gridValue.Value;
        }
    }
}
