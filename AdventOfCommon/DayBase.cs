using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public abstract class DayBase
    {
        public string AdditionalContent { get; set; }
        public virtual bool UsesAdditionalContent { get; protected set; } = false;
        public bool TestMode { get; set; } = false;
        public CancellationToken CancellationToken { get; set; } = default;
        public abstract string Title { get; }
        public abstract string Solve(string input, bool part2);

        protected const string Part2UnavailableMessage = "Part 2 is unavailable";

        protected List<string> GetLines(string input)
        {
            return input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        protected List<string> GetGroupedLines(string input)
        {
            return input.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
