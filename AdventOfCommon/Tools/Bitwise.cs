using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    static class Bitwise
    {
        public static int SetBit(int data, int bitNr, bool value)
        {
            int mask = 1 << bitNr;
            data = data & ~mask;
            if (value)
                data = data | mask;
            return data;
        }

        public static bool IsBitSet(int data, int bitNr)
        {
            return (data & (1 << bitNr)) != 0;
        }
    }
}
