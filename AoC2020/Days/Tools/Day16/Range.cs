using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day16
{
    class Range
    {
        public int Lower { get; set; }
        public int Upper { get; set; }

        public Range(int low, int high)
        {
            Lower = low;
            Upper = high;
        }

        public bool ValueInRange(int value)
        {
            return value >= Lower && value <= Upper;
        }
    }
}
