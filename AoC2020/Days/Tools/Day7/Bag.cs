using AdventOfCode.Tools.TopologicalOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day7
{
    class Bag: TopoItem
    {
        public override string Name { get; init; }
        public Dictionary<string, int> ContainedBags { get; set; } = new Dictionary<string, int>();
        public override bool IsDependantOn(string dependencyName) =>
            ContainedBags.ContainsKey(dependencyName);

        public override string ToString()
        {
            return Name;
        }
    }
}
