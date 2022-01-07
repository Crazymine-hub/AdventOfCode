using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.SpecificBitwise
{
    //Bitwise operations for ints
    [BitwiseHandler(typeof(int))]
    public static class IntBitwise
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
            return (data & 1 << bitNr) != 0;
        }

        public static long GetBitMask(int length)
        {
            long result = 1;
            for (int i = 1; i < length; i++)
                result = result << 1 | 1;
            return result;
        }

        public static int GetBitMask(IEnumerable<bool> values)
        {
            int result = 0;
            for (int i = 0; i < values.Count(); ++i)
                result = SetBit(result, i, values.ElementAt(i));
            return result;
        }

        public static int CountSetBits(int data)
        {
            int cntr = 0;
            while (data != 0)
            {
                if ((data & 1) == 1)
                    cntr++;
                data >>= 1;
            }
            return cntr;
        }
    }
}
