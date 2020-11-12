using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    public static class MathHelper
    {
        public static long LeastCommonMultiple(long[] numbers)
        {
            if (numbers.Length == 0) throw new ArgumentException("numbers has to contain at least one element", "numbers");
            if (numbers.Length == 1) return numbers[0];
            long result = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                result = LeastCommonMultiple(result, numbers[i]);
            }
            return result;
        }


        public static long LeastCommonMultiple(long a, long b)
        {
            if (a == 0 || b == 0) return 0;
            return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
        }

        public static long GreatestCommonDivisor(long[] numbers)
        {
            if (numbers.Length == 0) throw new ArgumentException("numbers has to contain at least one element", "numbers");
            if (numbers.Length == 1) return numbers[0];
            long result = numbers[0];

            for(int i = 1; i < numbers.Length; i++)
            {
                result = GreatestCommonDivisor(result, numbers[i]);
            }

            return result;
        }

        public static long GreatestCommonDivisor(long a, long b)
        {

            if (a == 0) return Math.Abs(b);
            if (b == 0) return Math.Abs(a);

            do
            {
                var rest = a % b;
                a = b;
                b = rest;
            } while (b != 0);

            return Math.Abs(a);
        }

        public static int IntegerFactorization(ulong number)
        {
            throw new NotImplementedException();
        }

        public static bool IsPrimeNumber(ulong number)
        {
            throw new NotImplementedException();
        }
    }
}
