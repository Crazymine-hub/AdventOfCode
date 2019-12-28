using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    static class NumberLists
    {
        public static ulong[] MakeArray(ulong number)
        {
            ulong fac = 10;
            List<ulong> result = new List<ulong>();
            do
            {
                result.Insert(0, ((number % fac) - (number % (fac / 10))) / (fac / 10));
                fac *= 10;
            } while (fac <= number);

            return result.ToArray();
        }

        public static ulong MakeNumber(ulong[] number)
        {
            ulong result = 0;
            foreach (ulong digit in number)
            {
                result += digit;
                result *= 10;
            }
            return result / 10;
        }
    }
}
