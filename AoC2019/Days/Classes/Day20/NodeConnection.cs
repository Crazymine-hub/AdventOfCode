using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day20
{
    class NodeConnection: BaseNodeConnection
    {
        public new Node NodeA { get => (Node)base.NodeA; protected set => base.NodeA = value; }
        public new Node NodeB { get => (Node)base.NodeB; protected set => base.NodeB = value; }

        public NodeConnection(Node a, Node b) : base(a, b)
        {
        }

        public NodeConnection(Node a, Node b, double dist) : base(a, b)
        {
            distance = dist;
        }
    }
}
