using System;

namespace AdventOfCode.Tools.Pathfinding
{
    [Flags]
    public enum ConnectionDirection
    {
        None = 0,
        AToB = 1,
        BToA = 2,
        Both = 3,
    }
}