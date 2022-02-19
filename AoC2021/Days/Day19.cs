using AdventOfCode.Days.Tools.Day19;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day19 : DayBase
    {
        public override string Title => "Beacon Scanner";

        List<SensorData> sensors = new List<SensorData>();

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;

            List<Task<List<IntersectResult>>> compareTasks = new List<Task<List<IntersectResult>>>();

            foreach (var lineGroup in GetGroupedLines(input))
                sensors.Add(new SensorData(GetLines(lineGroup)));

            for (int i = 0; i < sensors.Count - 1; ++i)
                for (int j = i + 1; j < sensors.Count; ++j)
                {
                    var task = new Task<List<IntersectResult>>((object state) =>
                    {
                        var rootSensor = ((SensorData[])state)[0];
                        var targetSensor = ((SensorData[])state)[1];
                        return CompareSensorData(rootSensor, targetSensor);
                    }, new SensorData[] { sensors[i], sensors[j] });
                    task.Start();
                    compareTasks.Add(task);
                }
            Task.WaitAll(compareTasks.ToArray());

            return "";
        }

        private List<IntersectResult> CompareSensorData(SensorData rootSensor, SensorData intersectSensor)
        {
            List<IntersectResult> results = new List<IntersectResult>();
            foreach (Point3 rootBeacon in rootSensor.Points)
            {
                List<Point3> rootPattern = rootSensor.Points.Select(point => point - rootBeacon).ToList();
                foreach (Point3 targetBeacon in intersectSensor.Points)
                {
                    for (int rotation = 0; rotation < 24; ++rotation)
                    {
                        Point3 rotatedTarget = targetBeacon.GetRotatedPoint(rotation);
                        IEnumerable<Point3> rotatedPoints = intersectSensor.Points.Select(point => point.GetRotatedPoint(rotation));
                        IEnumerable<Point3> rotatedPattern = rotatedPoints.Select(point => (point - rotatedTarget));
                        //rotatedPattern = rotatedPattern.Select(point => point.Invert());
                        var intersectPattern = rotatedPattern.Intersect(rootPattern);
                        if (intersectPattern.Count() == 12)
                            results.Add(new IntersectResult(rootBeacon, targetBeacon, rotation));
                    }
                }
            }
            return results;
        }
    }
}
