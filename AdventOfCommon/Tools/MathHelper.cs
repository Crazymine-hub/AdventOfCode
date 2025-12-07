using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    public static class MathHelper
    {
        public static long LeastCommonMultiple(params long[] numbers)
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

        public static long GreatestCommonDivisor(params long[] numbers)
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

        public static int Clamp(int value, int minimum, int maximum)
        {
            if (maximum < minimum) throw new ArgumentOutOfRangeException("maximum", "Maximum cannot be smaller than minium");
            return value < minimum ? minimum : (value > maximum ? maximum : value);
        }

        public static int IntegerFactorization(ulong number)
        {
            throw new NotImplementedException();
        }

        public static bool IsPrimeNumber(ulong number)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all operands, that are required to reach a desired number
        /// </summary>
        /// <param name="factorList">the list of all numbers to consider</param>
        /// <param name="operandCount">the amount of operands</param>
        /// <param name="desiredResult">the result, that must be reached</param>
        /// <param name="desiredIsProduct">Whether the desired result is a product or a sum.</param>
        /// <param name="otherValue">The Result of the other operation executed with the operands (Sum if <paramref name="desiredIsProduct"/> ist true. Product if false)</param>
        /// <returns>An Array with all required operands to get to the Result with the given operation. Or null, if no operands could be found.</returns>
        public static long[]? GetOperandsByResult(long[] factorList, int operandCount, long desiredResult, bool desiredIsProduct, out long otherValue)
        {
            bool numberMoved = true;
            int[] positions = new int[operandCount];
            long? product = null;
            long sum = ~desiredResult;
            //Initialize positions
            for (int pos = 0; pos < positions.Length; pos++)
                positions[pos] = pos;


            List<long> factors = new List<long>();
            while (desiredResult != (desiredIsProduct ? product : sum) && numberMoved)
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

        /// <summary>
        /// Gets the Product (and Sum) of the numbers at certain position
        /// </summary>
        /// <param name="numbers">a list of numbers</param>
        /// <param name="positions">the position of the numbers to multiply (and sum)</param>
        /// <param name="sum">the sum of the given numbers</param>
        /// <returns></returns>
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

        /// <summary>
        /// Increases the positions array at the given position
        /// <para>Uses Recursion</para>
        /// <para>Note: increases until the last first position reaches the Limit, given by <paramref name="numbers"/></para>
        /// </summary>
        /// <param name="position">the index of the position to increase</param>
        /// <param name="positions">the list of positions</param>
        /// <param name="numbers">the list of numbers, the positions refer to</param>
        /// <returns>True, if the position could be advanced, otherwise false.</returns>
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
