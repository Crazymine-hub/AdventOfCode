using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day18
{
    class NodeConnection
    {
        private Node nodeA;
        private Node nodeB;
        private int distance;
        public int Distance { get {
                if (nodeA.Lock != '\0' || nodeB.Lock != '\0')
                    return -1;
                else
                    return distance;
            }
        }

        public NodeConnection(Node a, Node b)
        {
            nodeA = a;
            nodeB = b;
            if (nodeA.X == nodeB.X && nodeA.Y != nodeB.Y)
                distance = Math.Abs(nodeA.Y - nodeB.Y);
            else if (nodeA.Y == nodeB.Y && nodeA.X != nodeB.X)
                distance = Math.Abs(nodeA.X - nodeB.X);
            else
                throw new InvalidOperationException("Nodes are not on same X or Y axis.");
        }

        public bool HasConnectionTo(Node target)
        {
            return (nodeA == target || nodeB == target);
        }

        public Node GetLinkFrom(Node target)
        {
            if (nodeA == target)
                return nodeB;
            else if (nodeB == target)
                return nodeA;
            else
                throw new ArgumentException("Node not in this connection");
        }
    }
}
