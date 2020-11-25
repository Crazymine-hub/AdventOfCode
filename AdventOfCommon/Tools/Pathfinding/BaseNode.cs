using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class BaseNode
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public double DistanceToTarget;
        public int PathIndex { get; set; }
        public virtual char CharRepresentation => '\0';

        public BaseNode(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            string koords = $" @{X}/{Y}";
            return (CharRepresentation == '\0' ? '+' : CharRepresentation) + koords;
        }

        public void UpdateTargetDistance(BaseNode target)
        {
            DistanceToTarget = Math.Sqrt(Math.Pow(target.X - X, 2) + Math.Pow(target.Y - Y, 2));
        }
    }
}
