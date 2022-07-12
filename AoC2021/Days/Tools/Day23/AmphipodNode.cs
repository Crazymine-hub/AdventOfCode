using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day23
{
    internal class AmphipodNode : AStarNode
    {
        private char occupiedBy = '\0';

        public bool CanOccupy { get; }
        public char HomeTo { get; set; } = '\0';
        public char OccupiedBy
        {
            get => occupiedBy;
            set
            {
                if (!CanOccupy) throw new InvalidOperationException("Cannot occupy this node.");
                occupiedBy = value;
            }
        }

        public AmphipodNode(int x, int y, bool canOccupy, char homeTo, char occupiedBy) : base(x, y, 0)
        {
            CanOccupy = canOccupy;
            HomeTo = homeTo;
            if(CanOccupy) OccupiedBy = occupiedBy;
        }
    }
}
