using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day15
{
    internal class RiskyNode : BaseNode
    {
        public int Risk { get; }

        public RiskyNode(int x, int y, int risk) : base(x, y)
        {
            Risk = risk;
        }

        public override string ToString()
        {
            return $"{base.ToString()}({Risk})";
        }
    }
}
