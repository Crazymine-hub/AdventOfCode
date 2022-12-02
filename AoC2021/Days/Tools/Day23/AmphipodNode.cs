using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day23
{
    [DebuggerDisplay("Amphipod ({X}|{Y}) O:{OccupiedBy} H:{HomeTo}")]
    internal class AmphipodNode : AStarNode
    {
        public const byte UnoccupiedNodeValue = 0;

        private byte occupiedBy = 0;

        public bool CanOccupy { get; }
        public byte HomeTo { get; set; } = 0;
        public bool IsAtHome { get => HomeTo != UnoccupiedNodeValue && HomeTo == OccupiedBy; }
        public bool IsOccupied { get => OccupiedBy != UnoccupiedNodeValue; }
        public byte OccupiedBy
        {
            get => occupiedBy;
            set
            {
                if (!CanOccupy) throw new InvalidOperationException("Cannot occupy this node.");
                occupiedBy = value;
            }
        }

        public AmphipodNode(int x, int y, bool canOccupy, byte homeTo, byte occupiedBy) : base(x, y, 0)
        {
            CanOccupy = canOccupy;
            HomeTo = homeTo;
            if (CanOccupy) OccupiedBy = occupiedBy;
        }
    }
}
