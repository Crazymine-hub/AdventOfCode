using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding.AStar
{
    public class AStarNode : BaseNode
    {
        public AStarNode PreviousNode { get; internal set; }
        public double ExpansionPriority => PathLength + NodeHeuristic;
        public double PathLength { get; internal set; } = double.PositiveInfinity;
        public double NodeHeuristic { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} (H:{NodeHeuristic} L:{PathLength} P:{ExpansionPriority})";
        }

        public AStarNode(int x, int y, double heuristic = double.PositiveInfinity) : base(x, y)
        {
            NodeHeuristic = heuristic;
        }
    }
}
