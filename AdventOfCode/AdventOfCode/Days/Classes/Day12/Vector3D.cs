using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day12
{
    class Vector3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int CrossSum => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public Vector3D(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Add(Vector3D vector3D)
        {
            X += vector3D.X;
            Y += vector3D.Y;
            Z += vector3D.Z;
        }

        public Vector3D Clone()
        {
            return new Vector3D(X, Y, Z);
        }
    }
}
