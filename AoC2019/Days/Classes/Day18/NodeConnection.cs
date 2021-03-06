﻿using System;
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
        public bool AllowPassLockedDoor { get; set; } = false;

        public override double Distance { get {
                if (!AllowPassLockedDoor && !(NodeA.IsUnlocked && NodeB.IsUnlocked))
                    return -1;
                else
                    return base.Distance;
            }
        }

        public NodeConnection(Node a, Node b): base(a, b)
        {
        }
    }
}
