using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day19
{
    internal struct IntersectResult
    {
        public Point3 RootPoint { get; }
        public Point3 TargetPoint { get; }
        public int TargetRotation { get; }

        public IntersectResult(Point3 rootPoint, Point3 targetPoint, int targetRotation)
        {
            RootPoint = rootPoint;
            TargetPoint = targetPoint;
            TargetRotation = targetRotation;
        }
    }
}
