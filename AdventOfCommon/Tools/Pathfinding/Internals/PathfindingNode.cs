using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding.Internals
{
    public sealed class AStarNode
    {
        public BaseNode Node { get; }
        public AStarNode PreviousNode { get; internal set; }
        public double FullCost { get; internal set; } = double.PositiveInfinity;
        public double PathCost { get; internal set; } = double.PositiveInfinity;
        public double DistanceToTarget { get; internal set; } = double.PositiveInfinity;


        internal AStarNode(BaseNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }
    }
}
