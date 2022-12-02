using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class BaseNode : IEquatable<BaseNode>
    {
        public virtual int X { get; protected set; }
        public virtual int Y { get; protected set; }

        public BaseNode(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"@{X}/{Y}";

        public virtual bool Equals(BaseNode other)
        {
            return other != null && other.X == X && other.Y == Y;
        }

        public virtual double GetDistanceTo(BaseNode target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return VectorAssist.GetLength(VectorAssist.PointDifference(this, target));
        }

        public static implicit operator Point(BaseNode operatedNode) => new Point(operatedNode.X, operatedNode.Y);
    }
}
