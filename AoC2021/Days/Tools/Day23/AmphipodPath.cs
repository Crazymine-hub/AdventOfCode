using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day23
{
    [DebuggerDisplay("AmphipodPath {From} -> {To}")]
    internal struct AmphipodPath
    {
        public AmphipodNode From { get; }
        public AmphipodNode To { get; }
        public long PathLength { get; }

        public AmphipodPath(AmphipodNode from, AmphipodNode to, long pathLength)
        {
            From = from;
            To = to;
            PathLength = pathLength;
        }
    }
}
