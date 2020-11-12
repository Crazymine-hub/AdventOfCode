using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public abstract class DayBase
    {
        public string AdditionalContent { get; set; }
        public bool UsesAdditionalContent { get; protected set; } = false;
        public abstract string Solve(string input, bool part2);
    }
}
