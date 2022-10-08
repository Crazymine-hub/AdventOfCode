using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    /// <summary>
    /// Tool for analyzing digits of a number
    /// </summary>
    public static class NumberLists
    {
        /// <summary>
        /// Stores each digit of a number in an array. Highest digit first
        /// </summary>
        /// <param name="number">The number to separate</param>
        /// <returns>An array with each digit  of the number</returns>
        public static ulong[] MakeArray(ulong number)
        {
            ulong fac = 10;
            List<ulong> result = new List<ulong>();
            do
            {
                result.Insert(0, ((number % fac) - (number % (fac / 10))) / (fac / 10));
                fac *= 10;
            } while (fac <= number * 10);

            return result.ToArray();
        }

        /// <summary>
        /// Creates a number from an array of digits
        /// </summary>
        /// <param name="number">The number in array form. each position being a single digit.</param>
        /// <returns>The Number the digits represent.</returns>
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

        /// <summary>
        /// Sets all digits of the number to the same value.
        /// </summary>
        /// <param name="number">The number in digitized form</param>
        /// <param name="digitValue">the value to set each digit to.</param>
        /// <returns></returns>
        public static ulong[] SetAll(ulong[] number, ulong digitValue)
        {
            for (int i = 0; i < number.Length; i++)
                number[i] = digitValue;
            return number;
        }

        public static ulong CrossSum(ulong[] number)
        {
            ulong result = 0;
            foreach (ulong digit in number)
                result += digit;
            return result;
        }

        public static ulong CrossSum(ulong number)
        {
            return CrossSum(MakeArray(number));
        }

        public static ulong[] SetLength(ulong[] number, uint wantedLength, bool atEnd)
        {
            if (number.Length == wantedLength) return number;
            List<ulong> editNr = number.ToList();
            while (editNr.Count != wantedLength)
            {
                if (editNr.Count > wantedLength)
                {
                    if (atEnd)
                        editNr.RemoveAt(editNr.Count - 1);
                    else
                        editNr.RemoveAt(0);
                }
                else
                {
                    if (atEnd)
                        editNr.Add(0);
                    else
                        editNr.Insert(0, 0);
                }
            }
            return editNr.ToArray();
        }
        
        public static bool UniqueDigits(ulong[] number)
        {
            List<ulong> usedDigits = new List<ulong>();
            foreach(ulong digit in number)
            {
                if (usedDigits.Contains(digit))
                    return false;
                else
                    usedDigits.Add(digit);
            }
            return true;
        }
    }
}
