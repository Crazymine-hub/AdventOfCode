using AdventOfCode.Tools.Extensions;
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
        public string? AdditionalContent { get; set; }
        public virtual bool UsesAdditionalContent { get; protected set; } = false;
        public bool TestMode { get; set; } = false;
        public CancellationToken CancellationToken { get; set; } = default;
        public abstract string Title { get; }
        public abstract string Solve(string input, bool part2);

        protected const string Part2UnavailableMessage = "Part 2 is unavailable";

        [Obsolete("The GetLines Method is available as string extension. (AdventOfCode.Tools.Extensions.StringExtensions)")]
        protected static List<string> GetLines(string input)
        {
            return input.GetLines();
        }

        [Obsolete("The GetGroupedLines Method is available as string extension. (AdventOfCode.Tools.Extensions.StringExtensions)")]
        protected static List<string> GetGroupedLines(string input)
        {
            return input.GetGroupedLines();
        }
    }
}
