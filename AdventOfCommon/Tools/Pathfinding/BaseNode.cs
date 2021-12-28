using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class BaseNode: IEquatable<BaseNode>
    {
        public virtual int X { get; protected set; }
        public virtual int Y { get; protected set; }
        public virtual double Heat { get; set; }
        public virtual double ClientHeat { get; set; }

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
            return Math.Sqrt(Math.Pow(target.X - X, 2) + Math.Pow(target.Y - Y, 2));
        }

        public virtual bool Equals(BaseNode other)
        {
            return other != null && other.X == X && other.Y == Y;
        }
    }
}
