using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day18
{
    class NodeConnection: Tools.Pathfinding.BaseNodeConnection
    {
        public new Node NodeA { get => (Node)base.NodeA; protected set => base.NodeA = value; }
        public new Node NodeB { get => (Node)base.NodeB; protected set => base.NodeB = value; }
        public bool IsHorizontal { get; private set; }

        public new int Distance { get {
                if (NodeA.Lock != '\0' || NodeB.Lock != '\0')
                    return -1;
                else
                    return base.Distance;
            }
        }

        public NodeConnection(Node a, Node b): base(a, b)
        {
            IsHorizontal = NodeA.X != NodeB.X;
        }
    }
}
