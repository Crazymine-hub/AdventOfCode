using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AdventOfCode.Tools;

namespace AdventOfCode.Days.Classes.Day10
{
    class AsteroidInfo
    {
        public Point Position;  //Absolute Position value
        public Point Direction; //Shortened Vector for Direction
        public int Detections;  //Amount of Asteroids Detected
        public double Distance; //Distance to Base Station

        public AsteroidInfo(Point position, Point direction, int detections, int distance)
        {
            Position = position;
            Direction = direction;
            Detections = detections;
            Distance = distance;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(AsteroidInfo)) throw new ArgumentException("Can only compare against AsteroidInfo instances");
            Point startDir = new Point(0, -1);
            AsteroidInfo compare = (AsteroidInfo)obj;
            if (compare.Direction == Direction)
                return Math.Sign(Distance - compare.Distance);
            else
            {
                double angle1 = VectorAssist.GetAngleBetween(Direction, startDir);
                double angle2 = VectorAssist.GetAngleBetween(compare.Direction, startDir);
                if (double.IsNaN(angle1)) angle1 = 0;
                if (double.IsNaN(angle2)) angle2 = 0;
                if (Direction.X < 0)
                    angle1 += 180;
                if (compare.Direction.X < 0)
                    angle2 += 180;
                return Math.Sign(angle1-angle2);
            }
        }

        public new string ToString()
        {
            Point direction = Direction == null ? new Point(0, -1) : Direction;
            double angle = VectorAssist.GetAngleBetween(direction, new Point(0, -1));
            if (direction.X < 0) angle = 360 - angle;

            return string.Format("Base @:({0}|{1} Detects: {2} Angle: {3}°)", Position.X, Position.Y, Detections, angle);
        }
    }
}
