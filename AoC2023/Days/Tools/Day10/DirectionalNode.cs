using AdventOfCode.Tools;
using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day10;
public class DirectionalNode: BaseNode
{
    public TraceChars.Direction Direction { get; set; }
    public int StepsFromStart { get; set; } = -1;

    public TraceChars.Direction SideA { get; set; } = TraceChars.Direction.None;
    public TraceChars.Direction SideB { get; set; } = TraceChars.Direction.None;

    public bool IsInside { get; set; }

    public DirectionalNode(int x, int y, TraceChars.Direction direction) : base(x, y)
    {
        Direction = direction;
    }

}
