﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    public static class Bitwise
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
        public static long SetBit(long data, int bitNr, bool value)
        {
            long mask = 1L << bitNr;
            data = data & ~mask;
            if (value)
                data = data | mask;
            return data;
        }

        public static bool IsBitSet(long data, int bitNr)
        {
            return (data & (1L << bitNr)) != 0;
        }

        public static long GetBitMask(int length)
        {
            long result = 1;
            for (int i = 1; i < length; i++)
                result = (result << 1) | 1;
            return result;
        }
    }
}