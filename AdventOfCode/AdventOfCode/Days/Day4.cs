using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class Day4 : IDay
    {
        public string Solve(string input, bool part2)
        {
            string[] range = input.Split('-');
            if (range.Length != 2)
                throw new ArgumentOutOfRangeException("input", "Only two Values allowed!");
            if (!ulong.TryParse(range[0], out ulong start) || !ulong.TryParse(range[1], out ulong end))
                throw new Exception("Input is not a Number!");
            if (range[0].Length != 6 || range[1].Length != 6)
                throw new ArgumentOutOfRangeException("The numbers must be exactly 6 digits long!");

            for(ulong i = start; i <= end; i++)
            {
                /* i / 1000000
                 * 
                 */
            }
        }
    }
}
