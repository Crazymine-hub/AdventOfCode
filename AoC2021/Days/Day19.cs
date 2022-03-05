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
        public override bool UsesAdditionalContent => true;

        List<SensorData> sensors = new List<SensorData>();

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;

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
            Task.WaitAll(compareTasks.ToArray(), CancellationToken);

            var intersections = compareTasks.Where(x => x.Result != null).Select(x => (IntersectResult)x.Result).ToList();
            var result = AssembleBeaconMap(intersections, null, new List<SensorData>());

            AdditionalContent = Point3.GetStanfordPly(result, "This is a PLY file of the final Points for AdventOfCode 2021 Day 19.\n" +
                "It can be opened with 3D Visualisation software, e.g. Blender");

            return $"Found {result.Count} beacons.";
        }

        private IntersectResult? CompareSensorData(SensorData rootSensor, SensorData intersectSensor)
        {
            List<IntersectResult> results = new List<IntersectResult>();
            foreach (Point3 rootBeacon in rootSensor.Points)
            {
                List<Point3> rootPattern = rootSensor.Points.Select(point => point - rootBeacon).ToList();
                foreach (Point3 targetBeacon in intersectSensor.Points)
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    for (int rotation = 0; rotation < 24; ++rotation)
                    {
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

        private List<Point3> AssembleBeaconMap(List<IntersectResult> intersects, SensorData currentRoot, List<SensorData> analyzed, Stack<bool> trace = null)
        {
            if (trace == null)
                trace = new Stack<bool>();

            List<IntersectResult> newAdditions = null;
            if (currentRoot != null)
                newAdditions = intersects.Where(x => (x.RootSensor == currentRoot || x.TargetSensor == currentRoot) && !analyzed.Contains(x.GetOther(currentRoot))).ToList();
            else
            {
                var start = intersects.First();
                analyzed.Add(start.RootSensor);
                var beaconMap = AssembleBeaconMap(intersects, start.RootSensor, analyzed, trace);
                beaconMap.AddRange(start.RootSensor.Points);
                beaconMap = beaconMap.Distinct().ToList();
                return beaconMap;
            }
            List<Point3> result = new List<Point3>();

            analyzed.AddRange(newAdditions.Select(x => x.GetOther(currentRoot)));
            //intersects.RemoveAll(x => newAdditions.Contains(x));
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

                IEnumerable<Point3> attachments = AssembleBeaconMap(intersects, realAddition.TargetSensor, analyzed, trace);
                result.AddRange(AssembleBeaconIntersection(realAddition, attachments));

                trace.Pop();
            }
            return result.Distinct().ToList();
        }

        private List<Point3> AssembleBeaconIntersection(IntersectResult intersectResult, IEnumerable<Point3> additionalPoints)
        {
            var reverse = false;
            var targetSensor = reverse ? intersectResult.RootSensor : intersectResult.TargetSensor;
            var targetReferencePoint = reverse ? intersectResult.RootPoint : intersectResult.TargetPoint;
            var rootAnchorPoint = reverse ? intersectResult.TargetPoint : intersectResult.RootPoint;
            var rotatedBeacons = targetSensor.Points.Select(point => point.Rotate(intersectResult.TargetRotation));
            var rotatedReference = targetReferencePoint.Rotate(intersectResult.TargetRotation);
            var resultBeacons = rotatedBeacons.Select(point => point - rotatedReference + rootAnchorPoint).ToList();

            var testBeacons = new List<Point3>();
            testBeacons.AddRange(resultBeacons);
            testBeacons.AddRange(intersectResult.RootSensor.Points);
            int fullCount = testBeacons.Count;
            var distinctBeacons = testBeacons.Distinct().ToList();
            int difference = fullCount - distinctBeacons.Count;

            if (additionalPoints != null)
                resultBeacons.AddRange(additionalPoints.Select(point => point.Rotate(intersectResult.TargetRotation) - rotatedReference + rootAnchorPoint));
            return resultBeacons;
        }
    }
}
