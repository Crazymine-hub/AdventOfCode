using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.SpecificBitwise
{
    //Bitwise operations for ulong
    [BitwiseHandler(typeof(ulong))]
    public static class ULongBitwise
    {
        public static ulong SetBit(ulong data, int bitNr, bool value)
        {
            ulong mask = 1UL << bitNr;
            data = data & ~mask;
            if (value)
                data = data | mask;
            return data;
        }

        public static bool IsBitSet(ulong data, int bitNr)
        {
            return (data & (1UL << bitNr)) != 0;
        }

        public static ulong GetValue(IEnumerable<bool> values)
        {
            ulong result = 0;
            for (int i = 0; i < values.Count(); ++i)
                result = SetBit(result, i, values.ElementAt(i));
            return result;
        }

        public static int CountSetBits(ulong data)
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
