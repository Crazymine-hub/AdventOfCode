using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day16
{
    //No playing games here
    internal class ValveInfo: AStarNode
    {
        public string Name { get; }
        public int PressureRelease { get; }
        public bool IsOpened { get; set; }

        public List<string> ConnectedValves { get; } = new List<string>();

        public override bool Equals(BaseNode other)
        {
            return Name == (other as ValveInfo)?.Name;
        }

        public ValveInfo(string name, int pressureRelease) : base(0, 0)
        {
            Name = name;
            PressureRelease = pressureRelease;
            IsOpened = false;
        }

        public override string ToString() => Name;
    }
}
