using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day16
{
    internal class ValveNodeConnection: AStarNodeConnection
    {
        public override double Distance { get; }
        public List<ValveInfo> PassedNodes { get; }

        public ValveNodeConnection(AStarNode a, AStarNode b, int distance, List<ValveInfo> passedNodes) : base(a, b, ConnectionDirection.Both)
        {
            Distance = distance;
            PassedNodes = passedNodes;
        }

        public override void RefreshDistance() { }
    }
}
