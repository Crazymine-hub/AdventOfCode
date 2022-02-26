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
                    }, new SensorData[] { sensors[i], sensors[j] });
                    task.Start();
                    compareTasks.Add(task);
                }
            Task.WaitAll(compareTasks.ToArray());

            var result = AssembleBeaconMap(compareTasks.Where(x => x.Result != null).Select(x => (IntersectResult)x.Result).ToList(), null, new Stack<int>());

            return "";
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
                        Point3 rotatedTarget = targetBeacon.Rotate(rotation);
                        IEnumerable<Point3> rotatedPoints = intersectSensor.Points.Select(point => point.Rotate(rotation));
                        IEnumerable<Point3> rotatedPattern = rotatedPoints.Select(point => point - rotatedTarget);
                        var intersectPattern = rotatedPattern.Intersect(rootPattern);
                        if (intersectPattern.Count() == 12)
                            return new IntersectResult(rootSensor, rootBeacon, intersectSensor, targetBeacon, rotation);
                    }
                }
            }
            return null;
        }

        private List<Point3> AssembleBeaconMap(List<IntersectResult> intersects, SensorData currentRoot, Stack<int> rotations)
        {
            List<IntersectResult> newAdditions = null;
            if(currentRoot != null)
                newAdditions = intersects.Where(x => x.RootSensor == currentRoot).ToList();
            else
                newAdditions = new List<IntersectResult>(){ intersects.First()};
            List<Point3> result = new List<Point3>();

            var newIntersects = intersects.Except(newAdditions).ToList();
            foreach (var addition in newAdditions)
            {
                var adjustedAddition = addition;
                rotations.Push(addition.TargetRotation);
                IEnumerable<Point3> attachments = AssembleBeaconMap(newIntersects, addition.TargetSensor, rotations);
                rotations.Pop();
                foreach (int rotation in rotations)
                {
                    adjustedAddition = adjustedAddition.Rotate(rotation);
                    attachments = attachments.Select(x => x.Rotate(rotation));
                }
                result.AddRange(AssembleBeaconIntersection(adjustedAddition, attachments));
            }
            return result.Distinct().ToList();
        }

        private List<Point3> AssembleBeaconIntersection(IntersectResult intersectResult, IEnumerable<Point3> additionalPoints)
        {
            var rotatedTarget = intersectResult.TargetSensor.Points.Select(point => point.Rotate(intersectResult.TargetRotation));
            var rotatedAnchor = intersectResult.TargetPoint.Rotate(intersectResult.TargetRotation);
            var resultBeacons = rotatedTarget.Select(point => point - rotatedAnchor + intersectResult.RootPoint).ToList();
            if (additionalPoints != null)
                resultBeacons.AddRange(additionalPoints.Select(point => point.Rotate(intersectResult.TargetRotation) - rotatedAnchor + intersectResult.RootPoint));
            return resultBeacons;
        }
    }
}
