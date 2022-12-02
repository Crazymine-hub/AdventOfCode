using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day17
{
    internal class ThrowResult
    {
        public List<Point> Trajectory { get; }
        public int MaxHeight { get;}
        public bool TargetHit { get; }
        public bool Overshot { get; }

        public ThrowResult(List<Point> trajectory, int maxHeight, bool targetHit, bool overshot)
        {
            Trajectory = trajectory;
            MaxHeight = maxHeight;
            TargetHit = targetHit;
            Overshot = overshot;
        }
    }
}
