using AdventOfCode.Days.Tools.Day19;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Graphics;
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
        public override bool UsesAdditionalContent => true;

        List<SensorData> sensors = new List<SensorData>();

        private ConsoleAssist consoleAssist = new ConsoleAssist();

        public override string Solve(string input, bool part2)
        {
            List<Task<IntersectResult?>> compareTasks = new List<Task<IntersectResult?>>();

            foreach (var lineGroup in GetGroupedLines(input))
                sensors.Add(new SensorData(GetLines(lineGroup)));

            for (int i = 0; i < sensors.Count - 1; ++i)
                for (int j = i + 1; j < sensors.Count; ++j)
                {
                    var task = new Task<IntersectResult?>((object state) =>
                    {
                        var rootSensor = ((SensorData[])state)[0];
                        var targetSensor = ((SensorData[])state)[1];
                        return CompareSensorData(rootSensor, targetSensor);
                    }, new SensorData[] { sensors[i], sensors[j] }, CancellationToken);
                    task.Start();
                    compareTasks.Add(task);
                }
            Console.WriteLine("Checking Matches....");
            consoleAssist.WaitForAllTasks(compareTasks, 100);
            CancellationToken.ThrowIfCancellationRequested();

            var intersections = compareTasks.Where(x => x.Result != null).Select(x => (IntersectResult)x.Result).ToList();
            var sensorLocations = new List<(SensorData, Point3)>();
            var result = AssembleBeaconMap(intersections, null, ref sensorLocations);

            AdditionalContent = Point3.GetStanfordPly(result, "This is a PLY file of the final Points for AdventOfCode 2021 Day 19.\n" +
                "It can be opened with 3D Visualisation software, e.g. Blender");


            int maxDistance = 0;
            SensorData sensorA = null;
            SensorData sensorB = null;
            for (int i = 0; i < sensorLocations.Count - 1; ++i)
                for (int j = i + 1; j < sensorLocations.Count; ++j)
                {
                    int distance = Point3.ManhattanDistance(sensorLocations[i].Item2, sensorLocations[j].Item2);
                    if(distance > maxDistance)
                    {
                        maxDistance = distance;
                        sensorA = sensorLocations[i].Item1;
                        sensorB = sensorLocations[j].Item1;
                    }
                }

                return $"Found {result.Count} beacons.\r\n"+
                $"The maximum manhattan distance between sensors {sensorA} and {sensorB} is {maxDistance}.";
        }

        private IntersectResult? CompareSensorData(SensorData rootSensor, SensorData intersectSensor)
        {
            List<IntersectResult> results = new List<IntersectResult>();
            foreach (Point3 rootBeacon in rootSensor.Points)
            {
                List<Point3> rootPattern = rootSensor.Points.Select(point => point - rootBeacon).ToList();
                foreach (Point3 targetBeacon in intersectSensor.Points)
                {
                    for (int rotation = 0; rotation < 24; ++rotation)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                        Point3 rotatedTarget = targetBeacon.Rotate(rotation);
                        IEnumerable<Point3> rotatedPoints = intersectSensor.Points.Select(point => point.Rotate(rotation));
                        IEnumerable<Point3> rotatedPattern = rotatedPoints.Select(point => point - rotatedTarget);
                        var intersectPattern = rotatedPattern.Intersect(rootPattern);
                        if (intersectPattern.Count() >= 12)
                            return new IntersectResult(rootSensor, rootBeacon, intersectSensor, targetBeacon, rotation);
                    }
                }
            }
            return null;
        }

        private List<Point3> AssembleBeaconMap(List<IntersectResult> intersects, SensorData currentRoot, ref List<(SensorData, Point3)> sensorLocations,
            List<SensorData> analyzed = null, Stack<bool> trace = null)
        {
            if (trace == null)
                trace = new Stack<bool>();
            if (analyzed == null)
                analyzed = new List<SensorData>();

            List<IntersectResult> newAdditions = null;
            if (currentRoot != null)
                newAdditions = intersects.Where(x => (x.RootSensor == currentRoot || x.TargetSensor == currentRoot) && !analyzed.Contains(x.GetOther(currentRoot))).ToList();
            else
            {
                var start = intersects.First();
                analyzed.Add(start.RootSensor);
                var beaconMap = AssembleBeaconMap(intersects, start.RootSensor, ref sensorLocations, analyzed, trace);
                beaconMap.AddRange(start.RootSensor.Points);
                beaconMap = beaconMap.Distinct().ToList();
                return beaconMap;
            }
            List<Point3> result = new List<Point3>();

            analyzed.AddRange(newAdditions.Select(x => x.GetOther(currentRoot)));
            foreach (var addition in newAdditions)
            {
                var realAddition = addition;
                if (addition.TargetSensor == currentRoot)
                    realAddition = CompareSensorData(realAddition.TargetSensor, realAddition.RootSensor).Value;

                bool isLast = addition.Equals(newAdditions.Last());
                foreach (bool traces in trace.Reverse()) Console.Write(traces ? " │" : "  ");
                Console.Write(isLast ? " └" : " ├");
                Console.WriteLine(realAddition.TargetSensor.ToString());
                trace.Push(!isLast);
                List<(SensorData, Point3)> activeSensorPoints = new List<(SensorData, Point3)>();
                IEnumerable<Point3> attachments = AssembleBeaconMap(intersects, realAddition.TargetSensor, ref activeSensorPoints, analyzed, trace);

                result.AddRange(AssembleBeaconIntersection(realAddition, attachments, ref activeSensorPoints));

                sensorLocations.AddRange(activeSensorPoints);
                trace.Pop();
            }
            sensorLocations.Add((currentRoot, new Point3(0, 0, 0)));
            return result.Distinct().ToList();
        }

        private List<Point3> AssembleBeaconIntersection(IntersectResult intersectResult, IEnumerable<Point3> additionalPoints, ref List<(SensorData, Point3)> sensorPoints)
        {
            var rotatedBeacons = intersectResult.TargetSensor.Points.Select(point => point.Rotate(intersectResult.TargetRotation));
            var rotatedReference = intersectResult.TargetPoint.Rotate(intersectResult.TargetRotation);
            var resultBeacons = rotatedBeacons.Select(point => point - rotatedReference + intersectResult.RootPoint).ToList();

            if (additionalPoints != null)
                resultBeacons.AddRange(additionalPoints.Select(point => point.Rotate(intersectResult.TargetRotation) - rotatedReference + intersectResult.RootPoint));
            
            sensorPoints = sensorPoints.Select(point => (point.Item1, point.Item2.Rotate(intersectResult.TargetRotation) - rotatedReference + intersectResult.RootPoint)).ToList();
            return resultBeacons;
        }
    }
}
