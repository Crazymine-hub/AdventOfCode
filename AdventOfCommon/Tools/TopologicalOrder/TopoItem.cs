using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.TopologicalOrder
{
    public abstract class TopoItem
    {
        public abstract string Name { get; }
        public abstract bool IsDependantOn(string dependencyName);
    }
}
