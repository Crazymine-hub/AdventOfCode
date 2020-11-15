using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class BaseNodeConnection
    {
        public BaseNode NodeA { get; protected set; }
        public BaseNode NodeB { get; protected set; }
        private int distance;
        public int Distance => distance;
        public double Rating { get; set; }

        public BaseNodeConnection(BaseNode a, BaseNode b)
        {
            NodeA = a;
            NodeB = b;
            if (NodeA.X == NodeB.X && NodeA.Y != NodeB.Y)
                distance = Math.Abs(NodeA.Y - NodeB.Y);
            else if (NodeA.Y == NodeB.Y && NodeA.X != NodeB.X)
                distance = Math.Abs(NodeA.X - NodeB.X);
            else
                throw new InvalidOperationException("Nodes are not on same X or Y axis.");
        }

        public bool HasConnectionTo(BaseNode target)
        {
            return (NodeA == target || NodeB == target);
        }

        public BaseNode GetOtherNode(BaseNode target)
        {
            if (NodeA == target)
                return NodeB;
            else if (NodeB == target)
                return NodeA;
            else
                throw new ArgumentException("Node not in this connection");
        }
    }
}
