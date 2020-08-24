using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day12
{
    class MoonInfo3D
    {
        public Vector3D Position { get; set; }
        public Vector3D Velocity { get; set; }

        public int EPot => Position.CrossSum;
        public int EKin => Velocity.CrossSum;

        public int Energy => EPot * EKin;

        public MoonInfo3D()
        {
            Position = new Vector3D();
            Velocity = new Vector3D();
        }

        internal void ApplyVelocity()
        {
            Position.Add(Velocity);
        }

        public new string ToString()
        {
            return $"pos=<x={Position.X}, y={Position.Y}, z={Position.Z}>, vel=<x={Velocity.X}, y={Velocity.Y}, z={Velocity.Z}>";
        }

        public string EnergyString()
        {
            return $"pot:  {Math.Abs(Position.X)} + {Math.Abs(Position.Y)} +  {Math.Abs(Position.Z)} = {EPot};   " +
                $"kin: {Math.Abs(Velocity.X)} +  {Math.Abs(Velocity.Y)} + {Math.Abs(Velocity.Z)} = {EKin};   " +
                $"total: {Energy}";
        }

        public MoonInfo3D Clone()
        {
            MoonInfo3D moon = new MoonInfo3D();
            moon.Position = Position.Clone();
            moon.Velocity = Velocity.Clone();
            return moon;
        }

        public byte findEquality(MoonInfo3D compareMoon)
        {
            byte result = 0;
            if (compareMoon.Position.X == Position.X && compareMoon.Velocity.X == Velocity.X)
                result += 1;
            if (compareMoon.Position.Y == Position.Y && compareMoon.Velocity.Y == Velocity.Y)
                result += 2;
            if (compareMoon.Position.Z == Position.Z && compareMoon.Velocity.Z == Velocity.Z)
                result += 4;

            return result;
        }
    }
}