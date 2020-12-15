using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public override string Title => "Report Repair";

        public override string Solve(string input, bool part2)
        {
            //Convert Input into List<int> (valid ints assumed)
            long[] factors = MathHelper.GetOperandsByResult(GetLines(input).Select(x => long.Parse(x)).ToArray(), part2 ? 3 : 2, 2020, false, out long result);
            if (factors == null)
                return "No match found!";
            else
            {
                string outp = "";
                foreach (int factor in factors)
                    outp += factor + " * ";
                outp += " 1 = " + result;
                return outp;
            }
        }
    }
}
