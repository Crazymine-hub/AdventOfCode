using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day19
{
    public class SensorData
    {
        HashSet<Point3> points = new HashSet<Point3>();

        public IReadOnlyCollection<Point3> Points => points;
        

        public SensorData(IEnumerable<string> pointList)
        {
            foreach (string pointDescription in pointList)
            {
                var coords = pointDescription.Split(',');
                points.Add(new Point3(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2])));
            }
        }

        public IEnumerable<Point3> GetPointsWithRotation(int rotation)
        {
            foreach (var point in points)
                yield return point.GetRotatedPoint(rotation);
        }
    }
}