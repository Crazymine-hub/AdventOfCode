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
        public abstract string Title { get; }
        public abstract string Solve(string input, bool part2);

        protected List<string> GetLines(string input)
        {
            return input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
