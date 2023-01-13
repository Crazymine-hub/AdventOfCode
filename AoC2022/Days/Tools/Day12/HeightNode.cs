using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day12
{
    internal class HeightNode : AStarNode
    {
        public int Height { get; }

        public HeightNode(int x, int y, int height) : base(x, y)
        {
            Height = height;
        }

    }
}
