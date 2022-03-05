using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day19
{
    public class SensorData
    {
        public string Name { get; }
        HashSet<Point3> points = new HashSet<Point3>();

        public IReadOnlyCollection<Point3> Points => points;
        

        public SensorData(IEnumerable<string> pointList)
        {
            Name = pointList.First();
            foreach (string pointDescription in pointList.Skip(1))
            {
                var coords = pointDescription.Split(',');
                points.Add(new Point3(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2])));
            }
        }

        private SensorData(string name, HashSet<Point3> points)
        {
            this.Name = name;
            this.points = points;
        }

        public SensorData Rotate(int rotation) => 
            new SensorData(Name, points.Select(x => x.Rotate(rotation)).ToHashSet());

        public override string ToString() => Name;

        public string GetStanfordPly() => Point3.GetStanfordPly(Points, $"AoC 2021 Day19 Sensor {Name}");
    }
}