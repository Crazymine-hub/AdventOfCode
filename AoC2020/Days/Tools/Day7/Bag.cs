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
        public override string Name { get; set; }
        public Dictionary<string, int> ContainedBags { get; set; } = new Dictionary<string, int>();
        public override Dictionary<string, int> Dependencies { get => ContainedBags; set => ContainedBags = value; }

        public override string ToString()
        {
            return Name;
        }
    }
}
