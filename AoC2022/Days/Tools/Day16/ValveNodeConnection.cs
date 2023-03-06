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
        public override double Distance => 1;

        public ValveNodeConnection(AStarNode a, AStarNode b, ConnectionDirection direction = ConnectionDirection.Both) : base(a, b, direction)
        {
        }

        public override void RefreshDistance() { }
    }
}
