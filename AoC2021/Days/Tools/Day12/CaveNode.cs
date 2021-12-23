using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day12
{
    internal class CaveNode : BaseNode
    {
        public string Name { get; }
        public bool Revisitable { get; }

        public CaveNode(string name, bool revisitable) : base(0,0)
        {
            Name = name;
            Revisitable = revisitable;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
