using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day15
{
    //HACK: This bodge could be solved in another way right? It works though.
    public class TwoWayNodeConnection : AStarNodeConnection
    {
        private AStarNode lastOtherNode;

        public override double Distance => lastOtherNode.NodeHeuristic;

        public TwoWayNodeConnection(AStarNode a, AStarNode b) : base(a, b)
        {
            lastOtherNode = a;
        }

        public override AStarNode GetOtherNode(AStarNode node)
        {
            lastOtherNode = base.GetOtherNode(node);
            return lastOtherNode;
        }
    }
}
