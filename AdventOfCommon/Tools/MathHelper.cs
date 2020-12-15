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

            for (int i = 1; i < numbers.Length; i++)
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


        public static long[] GetFactorsByResult(long[] factorList, int factorNumber, long desired, bool desiredIsProduct, out long otherValue)
        {
            bool numberMoved = true;
            int[] positions = new int[factorNumber];
            long? product = null;
            long sum = ~desired;
            //Initialize positions
            for (int pos = 0; pos < positions.Length; pos++)
                positions[pos] = pos;


            List<long> factors = new List<long>();
            while (desired != (desiredIsProduct ? product : sum) && numberMoved)
            {//repeat until found or done
                product = GetProduct(factorList, positions, out sum);
                factors.Clear();
                if (product != null)
                    foreach (int pos in positions)
                        factors.Add(factorList[pos]);
                numberMoved = MoveNextNumber(positions.Length - 1, positions, factorList);
            }
            otherValue = (desiredIsProduct ? sum : product) ?? 0;
            if (!numberMoved)
                return null;
            return factors.ToArray();
        }

        private static long? GetProduct(long[] numbers, int[] positions, out long sum)
        {
            sum = 0;
            long product = 1;
            string formula = "";
            foreach (int pos in positions)
            {//build sum and product of numbers at given positions
                if (pos >= numbers.Length) return null;
                sum += numbers[pos];
                product *= numbers[pos];
                formula += numbers[pos] + " x ";
            }
            return product;
        }

        private static bool MoveNextNumber(int position, int[] positions, long[] numbers)
        {
            //tried to change position below range. We're done.
            if (position == -1) return false;
            //increase the number at the given position.
            if (++positions[position] >= numbers.Length)
            {//increased above range. increase previous digit and reset given digit (RECURSION)
                if (!MoveNextNumber(position - 1, positions, numbers)) return false; //Previous digit could not be changed. abort.
                positions[position] = positions[position - 1] + 1;
            }
            return true;
        }
    }
}
