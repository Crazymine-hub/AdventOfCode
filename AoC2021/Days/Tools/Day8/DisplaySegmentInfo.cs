using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day8
{
    class DisplaySegmentInfo
    {
        public int BitCount { get; set; }
        public int Digit { get; }

        public DisplaySegmentInfo(int bitCount, int digit)
        {
            BitCount = bitCount;
            Digit = digit;
        }
    }
}
