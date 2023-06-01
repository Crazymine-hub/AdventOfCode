using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day17
{
    [DebuggerDisplay("{X}|{Height}: {ShapeIndex} ({PlacementCode})")]
    internal struct ShapePlacement
    {
        public int ListIndex { get; }
        public long Height { get; }
        public int ShapeIndex { get; }
        public int X { get; }
        public int PlacementCode { get; }
        public int JetIndex { get; }

        public ShapePlacement(long height, int shapeIndex, int x, int listIndex, int jetIndex)
        {
            Height = height;
            ShapeIndex = shapeIndex;
            X = x;
            PlacementCode = x << 4;
            PlacementCode |= shapeIndex;
            ListIndex = listIndex;
            JetIndex = jetIndex;
        }
    }
}
